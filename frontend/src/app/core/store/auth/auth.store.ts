import {
	patchState,
	signalStore,
	withComputed,
	withMethods,
	withProps,
	withState,
} from '@ngrx/signals';
import { initialAuthSlice } from './auth.slice';
import { computed, inject } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { AccountService } from '../../services/account.service';
import { withDevtools } from '@angular-architects/ngrx-toolkit';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { LoginRequest, RegisterRequest } from '../../../shared/models/auth';
import { exhaustMap, finalize, pipe, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import { ActivatedRoute, Router } from '@angular/router';
import { SnackbarService } from '../../services/snackbar.service';
import { authGuard } from '../../guards/auth.guard';
import { setCurrentUser } from './auth.updaters';

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
							next: (user) => patchState(store, setCurrentUser(user)),
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
							patchState(store, setCurrentUser(null));

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
					next: (user) => patchState(store, setCurrentUser(user)),
					error: () => logout(),
				}),
			);
		};

		const isResourceOwnerOrAdmin = (userId?: string): boolean => {
			const currentUser = store.currentUser();

			return currentUser?.roles.includes('Admin') || currentUser?.id == userId;
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
