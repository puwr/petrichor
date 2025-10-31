import { Injectable, inject } from '@angular/core';
import { catchError, EMPTY } from 'rxjs';
import { AuthStore } from './auth';

@Injectable({
  providedIn: 'root',
})
export class InitService {
  private authStore = inject(AuthStore);

  initialize() {
    return this.authStore.refreshToken().pipe(catchError(() => EMPTY));
  }
}
