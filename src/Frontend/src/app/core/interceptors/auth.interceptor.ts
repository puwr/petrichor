import {
  HttpErrorResponse,
  HttpInterceptorFn,
  HttpRequest,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, EMPTY, switchMap, throwError } from 'rxjs';
import { AuthFacade } from '../stores/auth/auth.facade';
import { Router } from '@angular/router';
import { SnackbarService } from '../services/snackbar.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const authFacade = inject(AuthFacade);
  const snackbar = inject(SnackbarService);

  const request = req.clone({
    withCredentials: true,
  });

  return next(request).pipe(
    catchError((error) => {
      if (is401Error(error, request)) {
        return authFacade.refreshToken().pipe(
          switchMap(() => next(request)),
          catchError(() => {
            snackbar.error('Something went wrong, please log in again.');

            router.navigate(['/login'], {
              queryParams: { returnUrl: router.url },
            });

            return EMPTY;
          })
        );
      }

      return throwError(() => error);
    })
  );
};

function is401Error(error: unknown, request: HttpRequest<unknown>) {
  return (
    error instanceof HttpErrorResponse &&
    error.status === 401 &&
    !request.url.includes('/login') &&
    !request.url.includes('/register') &&
    !request.url.includes('/refresh-token')
  );
}
