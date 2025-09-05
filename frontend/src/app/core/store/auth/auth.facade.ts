import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { environment } from '../../../../environments/environment';
import { LoginRequest, RegisterRequest } from '../../../shared/models/auth';
import {
  catchError,
  EMPTY,
  exhaustMap,
  finalize,
  map,
  Observable,
  tap,
  throwError,
} from 'rxjs';
import { User } from '../../../shared/models/user';
import { createStore, select, setProp, withProps } from '@ngneat/elf';
import { toSignal } from '@angular/core/rxjs-interop';
import {
  clearRequestsStatus,
  createRequestsStatusOperator,
  selectRequestStatus,
  updateRequestStatus,
  withRequestsStatus,
} from '@ngneat/elf-requests';
import { EffectFn } from '@ngneat/effects-ng';
import { SnackbarService } from '../../services/snackbar.service';
import { authGuard } from '../../guards/auth.guard';

type AuthState = {
  currentUser: User | null;
  isAuthenticated: boolean;
};

type AuthRequestKeys = 'login' | 'register';

@Injectable({ providedIn: 'root' })
export class AuthFacade extends EffectFn {
  private http = inject(HttpClient);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private snackbar = inject(SnackbarService);

  private apiUrl = environment.apiUrl;

  private readonly store = createStore(
    { name: 'auth' },
    withProps<AuthState>({ currentUser: null, isAuthenticated: false }),
    withRequestsStatus<AuthRequestKeys>()
  );
  private readonly trackAuthRequestsStatus = createRequestsStatusOperator(
    this.store
  );

  readonly currentUser = toSignal(
    this.store.pipe(select((state) => state.currentUser)),
    { initialValue: null }
  );
  readonly isAuthenticated = toSignal(
    this.store.pipe(select((state) => state.isAuthenticated)),
    { initialValue: false }
  );

  isResourceOwnerOrAdmin(userId: string): boolean {
    const currentUser = this.currentUser();

    return currentUser?.roles.includes('Admin') || currentUser?.id == userId;
  }

  setAuthState(currentUser: User | null, isAuthenticated: boolean): void {
    this.store.update(
      setProp('currentUser', currentUser),
      setProp('isAuthenticated', isAuthenticated)
    );
  }

  readonly registerStatus = toSignal(
    this.store.pipe(selectRequestStatus('register'))
  );
  readonly registerEffect = this.createEffectFn(
    (userData$: Observable<RegisterRequest>) => {
      return userData$.pipe(
        exhaustMap((userData) => {
          return this.http
            .post<void>(`${this.apiUrl}/auth/register`, userData)
            .pipe(
              this.trackAuthRequestsStatus('register'),
              tap(() => {
                this.store.update(updateRequestStatus('register', 'success'));
                this.snackbar.success(
                  'Registration complete! You may now log in.'
                );
                this.router.navigateByUrl('/login');
              }),
              catchError(() => EMPTY)
            );
        })
      );
    }
  );

  readonly loginStatus = toSignal(
    this.store.pipe(selectRequestStatus('login'))
  );
  readonly loginEffect = this.createEffectFn(
    (credentials$: Observable<LoginRequest>) => {
      return credentials$.pipe(
        exhaustMap((credentials) => {
          return this.http
            .post<void>(`${this.apiUrl}/auth/login`, credentials)
            .pipe(
              this.trackAuthRequestsStatus('login'),
              exhaustMap(() =>
                this.updateCurrentUser().pipe(
                  catchError((error) => {
                    this.store.update(updateRequestStatus('login', 'idle'));
                    return throwError(() => error);
                  })
                )
              ),
              tap({
                complete: () => {
                  this.store.update(updateRequestStatus('login', 'success'));

                  const returnUrl =
                    this.route.snapshot.queryParams['returnUrl'] ?? '/';
                  this.router.navigateByUrl(returnUrl);
                },
              }),
              catchError(() => EMPTY)
            );
        })
      );
    }
  );

  readonly logoutEffect = this.createEffectFn(($: Observable<void>) => {
    return $.pipe(
      exhaustMap(() => {
        return this.http.post<void>(`${this.apiUrl}/auth/logout`, {}).pipe(
          finalize(() => {
            this.setAuthState(null, false);

            if (this.isRouteProtected()) {
              this.router.navigateByUrl('/');
            }
          }),
          catchError(() => EMPTY)
        );
      })
    );
  });

  refreshToken(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/auth/refresh-token`, {}).pipe(
      exhaustMap(() => this.updateCurrentUser()),
      map(() => undefined)
    );
  }

  readonly clearRequestStatus = () => this.store.update(clearRequestsStatus());

  private updateCurrentUser(): Observable<void> {
    return this.http.get<User>(`${this.apiUrl}/account/me`).pipe(
      tap((user) => {
        this.setAuthState(user, true);
      }),
      catchError(() => {
        this.logoutEffect();
        return throwError(() => EMPTY);
      }),
      map(() => undefined)
    );
  }

  private isRouteProtected(): boolean {
    return (
      this.route.snapshot.root.firstChild?.routeConfig?.canActivate?.includes(
        authGuard
      ) ?? false
    );
  }
}
