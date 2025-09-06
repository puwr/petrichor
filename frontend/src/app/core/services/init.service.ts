import { inject, Injectable } from '@angular/core';
import { AuthStore } from '../store/auth/auth.store';
import { catchError, EMPTY } from 'rxjs';

@Injectable({
	providedIn: 'root',
})
export class InitService {
	private authStore = inject(AuthStore);

	initialize() {
		return this.authStore.refreshToken().pipe(catchError(() => EMPTY));
	}
}
