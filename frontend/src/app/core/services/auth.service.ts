import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import {
  AuthStatusResponse,
  LoginRequest,
  RegisterRequest,
} from '../../shared/models/auth';
import { AccountService } from './account.service';
import { finalize, Observable } from 'rxjs';
import { Router } from '@angular/router';

/**
 * @deprecated This service is deprecated.
 * Please use AuthFacade instead.
 */
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private accountService = inject(AccountService);

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
    return this.http.post<void>(`${this.apiUrl}/auth/logout`, {}).pipe(
      finalize(() => {
        this.accountService.currentUser.set(null);
        this.router.navigateByUrl('/');
      })
    );
  }

  getAuthStatus(): Observable<AuthStatusResponse> {
    return this.http.get<AuthStatusResponse>(`${this.apiUrl}/auth/status`);
  }
}
