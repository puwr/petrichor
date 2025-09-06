import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { User } from '../../shared/models/user';
import { Observable } from 'rxjs';

@Injectable({
	providedIn: 'root',
})
export class AccountService {
	private http = inject(HttpClient);

	private apiUrl = environment.apiUrl;

	updateCurrentUser(): Observable<User> {
		return this.http.get<User>(`${this.apiUrl}/account/me`);
	}
}
