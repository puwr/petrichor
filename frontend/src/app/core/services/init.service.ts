import { inject, Injectable } from '@angular/core';
import { AuthFacade } from '../store/auth/auth.facade';
import { catchError, EMPTY } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class InitService {
  private authFacade = inject(AuthFacade);

  initialize() {
    return this.authFacade.refreshToken().pipe(catchError(() => EMPTY));
  }
}
