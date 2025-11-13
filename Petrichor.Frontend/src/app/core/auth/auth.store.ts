import { withDevtools } from '@angular-architects/ngrx-toolkit';
import { computed, inject } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AccountService } from '@app/core/account/account.service';
import { SnackbarService } from '@app/core/snackbar.service';
import { tapResponse } from '@ngrx/operators';
import {
  signalStore,
  withState,
  withComputed,
  withProps,
  withMethods,
  patchState,
} from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, exhaustMap, finalize, tap } from 'rxjs';
import { authGuard } from './auth.guard';
import { RegisterRequest, LoginRequest } from './auth.models';
import { AuthService } from './auth.service';
import { CurrentUser } from '@app/core/account/account.models';

interface AuthSlice {
  currentUser: CurrentUser | null;
}

const initialAuthSlice: AuthSlice = {
  currentUser: null,
};

export const AuthStore = signalStore(
  { providedIn: 'root' },
  withState(initialAuthSlice),
  withComputed(({ currentUser }) => ({
    isAuthenticated: computed(() => (currentUser() ? true : false)),
  })),
  withProps((_) => ({
    _authService: inject(AuthService),
    _accountService: inject(AccountService),
    _router: inject(Router),
    _route: inject(ActivatedRoute),
    _snackbar: inject(SnackbarService),
  })),
  withMethods((store) => {
    const register = rxMethod<{
      userData: RegisterRequest;
      onError: (errors: string[] | null) => void;
    }>(
      pipe(
        exhaustMap(({ userData, onError }) => {
          return store._authService.register(userData).pipe(
            tapResponse({
              next: () => {
                store._snackbar.success('Registration complete! You may now log in.');
                store._router.navigateByUrl('/login');
              },
              error: (errors: string[] | null) => onError(errors),
            }),
          );
        }),
      ),
    );

    const login = rxMethod<{
      credentials: LoginRequest;
      onError: (errors: string[] | null) => void;
    }>(
      pipe(
        exhaustMap(({ credentials, onError }) => {
          return store._authService.login(credentials).pipe(
            exhaustMap(() => store._accountService.updateCurrentUser()),
            tapResponse({
              next: (user) => patchState(store, { currentUser: user }),
              error: (errors: string[] | null) => onError(errors),
              complete: () => {
                const returnUrl = store._route.snapshot.queryParams['returnUrl'] ?? '/';
                store._router.navigateByUrl(returnUrl);
              },
            }),
          );
        }),
      ),
    );

    const logout = rxMethod<void>(
      pipe(
        exhaustMap(() => {
          const isRouteProtected = () => {
            return (
              store._route.snapshot.root.firstChild?.routeConfig?.canActivate?.includes(
                authGuard,
              ) ?? false
            );
          };

          return store._authService.logout().pipe(
            finalize(() => {
              patchState(store, { currentUser: null });

              if (isRouteProtected()) {
                store._router.navigateByUrl('/');
              }
            }),
          );
        }),
      ),
    );

    const refreshToken = () => {
      return store._authService.refreshToken().pipe(
        exhaustMap(() => store._accountService.updateCurrentUser()),
        tap({
          next: (user) => patchState(store, { currentUser: user }),
          error: () => logout(),
        }),
      );
    };

    const isResourceOwnerOrAdmin = (userName?: string): boolean => {
      const currentUser = store.currentUser();

      return currentUser?.roles.includes('Admin') || currentUser?.userName == userName;
    };

    return {
      register,
      login,
      logout,
      refreshToken,
      isResourceOwnerOrAdmin,
    };
  }),
  withDevtools('auth'),
);
