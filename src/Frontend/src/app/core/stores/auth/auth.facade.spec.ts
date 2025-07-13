import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { AuthFacade } from './auth.facade';
import { ActivatedRoute, Router } from '@angular/router';
import { environment } from '../../../../environments/environment';
import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { SnackbarService } from '../../services/snackbar.service';
import {
  mockLoginRequest,
  mockRegisterRequest,
  mockUser,
} from '../../../../testing/test-data';
import { authGuard } from '../../guards/auth.guard';

describe('AuthFacade', () => {
  const apiUrl = environment.apiUrl;

  it('should be created', () => {
    const { authFacade } = setup();

    expect(authFacade).toBeTruthy();
  });

  describe('registerEffect', () => {
    it('updates status to "success", shows success notification, redirects to login page on success', () => {
      const { authFacade, httpMock, router, snackbar } = setup();

      authFacade.registerEffect(mockRegisterRequest);
      httpMock.expectOne(`${apiUrl}/auth/register`).flush(null);

      expect(authFacade.registerStatus()?.value).toBe('success');
      expect(snackbar.success).toHaveBeenCalledTimes(1);
      expect(router.navigateByUrl).toHaveBeenCalledExactlyOnceWith('/login');

      httpMock.verify();
    });

    it('updates status to "error", avoids success side effects on failure', () => {
      const { authFacade, httpMock, router, snackbar } = setup();

      authFacade.registerEffect(mockRegisterRequest);
      httpMock
        .expectOne(`${apiUrl}/auth/register`)
        .flush(null, { status: 400, statusText: 'Bad Request' });

      expect(authFacade.registerStatus()?.value).toBe('error');
      expect(snackbar.success).not.toHaveBeenCalled();
      expect(router.navigateByUrl).not.toHaveBeenCalled();

      httpMock.verify();
    });
  });

  describe('loginEffect', () => {
    it('updates status to "success", updates auth state and redirects to return url on login and user update success', () => {
      const { authFacade, httpMock, router, route } = setup();

      authFacade.loginEffect(mockLoginRequest);
      httpMock.expectOne(`${apiUrl}/auth/login`).flush(null);
      httpMock.expectOne(`${apiUrl}/account/me`).flush(mockUser);

      expect(authFacade.loginStatus()?.value).toBe('success');
      expect(authFacade.isAuthenticated()).toBe(true);
      expect(authFacade.currentUser()).toBe(mockUser);
      expect(router.navigateByUrl).toHaveBeenCalledExactlyOnceWith(
        route.snapshot.queryParams['returnUrl']
      );

      httpMock.verify();
    });

    it('updates status to "error", avoids success side effects on login failure', () => {
      const { authFacade, httpMock, router } = setup();

      authFacade.loginEffect(mockLoginRequest);
      httpMock
        .expectOne(`${apiUrl}/auth/login`)
        .flush(null, { status: 400, statusText: 'Bad Request' });

      expect(authFacade.loginStatus()?.value).toBe('error');
      expect(router.navigateByUrl).not.toHaveBeenCalled();

      httpMock.verify();
    });

    it('updates status to "idle", triggers logout, avoids success side effects on login success and user update failure', () => {
      const { authFacade, httpMock, router, route } = setup();

      authFacade.loginEffect(mockLoginRequest);
      httpMock.expectOne(`${apiUrl}/auth/login`).flush(null);
      httpMock
        .expectOne(`${apiUrl}/account/me`)
        .flush(null, { status: 403, statusText: 'Forbidden' });
      httpMock.expectOne(`${apiUrl}/auth/logout`).flush(null);

      expect(authFacade.loginStatus()?.value).toBe('idle');
      expect(authFacade.logoutEffect).toHaveBeenCalledTimes(1);
      expect(router.navigateByUrl).not.toHaveBeenCalledWith(
        route.snapshot.queryParams['returnUrl']
      );

      httpMock.verify();
    });
  });

  describe('logoutEffect', () => {
    test.each([['success'], ['failure']])(
      'clears auth state, avoids redirection if route is public on %s',
      (responseType) => {
        const { authFacade, httpMock, router } = setup();

        authFacade.setAuthState(mockUser, true);
        authFacade.logoutEffect();
        httpMock
          .expectOne(`${apiUrl}/auth/logout`)
          .flush(responseType === 'success' ? null : null, {
            status: 500,
            statusText: 'Server Error',
          });

        expect(authFacade.isAuthenticated()).toBe(false);
        expect(authFacade.currentUser()).toBeNull();
        expect(router.navigateByUrl).not.toHaveBeenCalled();

        httpMock.verify();
      }
    );

    test.each([['success'], ['failure']])(
      'clears auth state, redirects to home page if route is protected on %s',
      (responseType) => {
        const { authFacade, httpMock, router } = setup({
          isCurrentRouteProtected: true,
        });

        authFacade.setAuthState(mockUser, true);
        authFacade.logoutEffect();
        httpMock
          .expectOne(`${apiUrl}/auth/logout`)
          .flush(responseType === 'success' ? null : null, {
            status: 500,
            statusText: 'Server Error',
          });

        expect(authFacade.isAuthenticated()).toBe(false);
        expect(authFacade.currentUser()).toBeNull();
        expect(router.navigateByUrl).toHaveBeenCalledExactlyOnceWith('/');

        httpMock.verify();
      }
    );
  });

  describe('refreshToken', () => {
    it('updates current user on refresh token and user update success', () => {
      const { authFacade, httpMock } = setup();

      authFacade.refreshToken().subscribe();
      httpMock.expectOne(`${apiUrl}/auth/refresh-token`).flush(null);
      httpMock.expectOne(`${apiUrl}/account/me`).flush(mockUser);

      expect(authFacade.isAuthenticated()).toBe(true);
      expect(authFacade.currentUser()).toBe(mockUser);

      httpMock.verify();
    });

    it('rethrows error on refresh token failure', () => {
      const { authFacade, httpMock } = setup();

      authFacade.refreshToken().subscribe({
        error: (error) => {
          expect(error.status).toBe(500);
        },
      });

      httpMock
        .expectOne(`${apiUrl}/auth/refresh-token`)
        .flush(null, { status: 500, statusText: 'Server Error' });

      httpMock.verify();
    });

    it('triggers logout and rethrows EMPTY error on user update failure', () => {
      const { authFacade, httpMock } = setup();

      let recievedError = false;

      authFacade.refreshToken().subscribe({
        error: () => (recievedError = true),
      });

      httpMock.expectOne(`${apiUrl}/auth/refresh-token`).flush(null);
      httpMock
        .expectOne(`${apiUrl}/account/me`)
        .flush(null, { status: 500, statusText: 'Server Error' });
      httpMock.expectOne(`${apiUrl}/auth/logout`).flush(null);

      expect(authFacade.logoutEffect).toHaveBeenCalledTimes(1);
      expect(recievedError).toBe(true);

      httpMock.verify();
    });
  });
});

function setup(options?: { isCurrentRouteProtected?: boolean }) {
  TestBed.configureTestingModule({
    providers: [
      provideHttpClient(),
      provideHttpClientTesting(),
      {
        provide: ActivatedRoute,
        useValue: {
          snapshot: {
            queryParams: { returnUrl: '/upload' },
            root: {
              firstChild: {
                routeConfig: {
                  canActivate: options?.isCurrentRouteProtected
                    ? [authGuard]
                    : [],
                },
              },
            },
          },
        },
      },
      {
        provide: SnackbarService,
        useValue: { success: vi.fn(), error: vi.fn() },
      },
    ],
  });

  const authFacade = TestBed.inject(AuthFacade);
  const httpMock = TestBed.inject(HttpTestingController);
  const router = TestBed.inject(Router);
  const route = TestBed.inject(ActivatedRoute);
  const snackbar = TestBed.inject(SnackbarService);

  vi.spyOn(router, 'navigateByUrl');
  vi.spyOn(authFacade, 'logoutEffect');

  return { authFacade, httpMock, router, route, snackbar };
}
