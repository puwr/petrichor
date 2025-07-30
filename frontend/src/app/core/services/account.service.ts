import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { User } from '../../shared/models/user';
import {
  catchError,
  ignoreElements,
  Observable,
  of,
  tap,
  throwError,
} from 'rxjs';

/**
 * @deprecated This service is deprecated.
 * Please use AuthFacade instead.
 */
@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private http = inject(HttpClient);

  private apiUrl = environment.apiUrl;

  currentUser = signal<User | null>(null);

  updateCurrentUser(): Observable<void> {
    return this.http.get<User>(`${this.apiUrl}/account/me`).pipe(
      tap((user) => {
        this.currentUser.set(user);
      }),
      catchError((error) => {
        if (
          error instanceof HttpErrorResponse &&
          (error.status === 401 || error.status === 403)
        ) {
          this.currentUser.set(null);
          return of(null);
        }

        return throwError(() => error);
      }),
      ignoreElements()
    );
  }
}
