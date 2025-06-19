import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { SnackbarService } from '../services/snackbar.service';
import { Router } from '@angular/router';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const snackbar = inject(SnackbarService);

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status === 400) {
        if (err.error?.errors) {
          const validationErrors = Object.values(err.error.errors).flat();

          if (validationErrors.length) {
            return throwError(() => validationErrors);
          }
        }

        snackbar.error(err.error?.title || err.error);
        return throwError(() => err);
      }

      if (err.status === 401) {
        // Handled in auth interceptor
        return throwError(() => err);
      }

      if (err.status === 403) {
        snackbar.error('Access denied.');
        return throwError(() => err);
      }

      if (err.status === 404) {
        router.navigateByUrl('/not-found');
        return throwError(() => err);
      }

      if (err.status === 409) {
        return throwError(() => [err.error?.title || 'Conflict occured.']);
      }

      if (err.status >= 500) {
        snackbar.error('Server error. Please try again later.');
        return throwError(() => err);
      }

      snackbar.error(err.error?.title || 'Something went wrong.');
      return throwError(() => err);
    })
  );
};
