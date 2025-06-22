import {
  HttpErrorResponse,
  HttpInterceptorFn,
  HttpRequest,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { AccountService } from '../services/account.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const accountService = inject(AccountService);

  const request = req.clone({
    withCredentials: true,
  });

  return next(request).pipe(
    catchError((error) => {
      if (is401Error(error, request)) {
        return authService.refreshToken().pipe(
          switchMap(() => accountService.updateCurrentUser()),
          switchMap(() => next(request)),
          catchError((error) => {
            accountService.currentUser.set(null);
            return throwError(() => error);
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
