import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { AuthStatusResponse, LoginRequest, RegisterRequest } from '../../shared/models/auth';
import { Observable } from 'rxjs';

@Injectable({
	providedIn: 'root',
})
export class AuthService {
	private http = inject(HttpClient);

	private apiUrl = environment.apiUrl;

	register(userData: RegisterRequest): Observable<void> {
		return this.http.post<void>(`${this.apiUrl}/auth/register`, userData);
	}

	login(credentials: LoginRequest): Observable<void> {
		return this.http.post<void>(`${this.apiUrl}/auth/login`, credentials);
	}

	refreshToken(): Observable<void> {
		return this.http.post<void>(`${this.apiUrl}/auth/refresh-token`, {});
	}

	logout(): Observable<void> {
		return this.http.post<void>(`${this.apiUrl}/auth/logout`, {});
	}

	getAuthStatus(): Observable<AuthStatusResponse> {
		return this.http.get<AuthStatusResponse>(`${this.apiUrl}/auth/status`);
	}
}
