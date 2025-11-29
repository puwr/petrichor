import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { SnackbarService } from './snackbar.service';
import { ValidationError } from '@angular/forms/signals';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const snackbar = inject(SnackbarService);

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status === 0) {
        snackbar.error('Network error. Please check your connection.');
      }

      if (err.status === 400) {
        if (err.error.errors) {
          const validationErrors: ValidationError[] = Object.values(err.error.errors).flatMap(
            (messages) =>
              (messages as string[]).map((message) => ({
                kind: 'server',
                message: message.trim(),
              })),
          );

          if (validationErrors.length > 0) {
            return throwError(() => validationErrors);
          }
        }

        if (err.error?.title) {
          return throwError(() => [{ kind: 'server', message: err.error.title }]);
        }

        snackbar.error(err.error?.title || err.error);
      }

      if (err.status === 403) {
        snackbar.error('Access denied.');
      }

      if (err.status === 404) {
        router.navigateByUrl('/not-found');
      }

      if (err.status === 409) {
        if (err.error?.title) {
          return throwError(() => [{ kind: 'server', message: err.error.title }]);
        }

        snackbar.error('Conflict occured.');
      }

      if (err.status >= 500) {
        snackbar.error('Server error. Please try again later.');
      }

      return throwError(() => err);
    }),
  );
};
