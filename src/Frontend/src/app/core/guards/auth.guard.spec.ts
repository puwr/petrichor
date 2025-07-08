import { TestBed } from '@angular/core/testing';
import { AccountService } from '../services/account.service';
import {
  ActivatedRouteSnapshot,
  CanActivateFn,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { signal, WritableSignal } from '@angular/core';
import { User } from '../../shared/models/user';
import { AuthService } from '../services/auth.service';
import { authGuard } from './auth.guard';
import { mockUser } from '../../../testing/fixtures';
import { firstValueFrom, Observable, of } from 'rxjs';
import { Mock } from 'vitest';

describe('authGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) =>
    TestBed.runInInjectionContext(() => authGuard(...guardParameters));

  let authService: { getAuthStatus: Mock };
  let accountService: { currentUser: WritableSignal<User | null> };
  let router: { navigate: Mock };

  const mockRoute = {} as ActivatedRouteSnapshot;
  const mockState = { url: '/upload' } as RouterStateSnapshot;

  beforeEach(() => {
    authService = { getAuthStatus: vi.fn() };
    accountService = { currentUser: signal<User | null>(null) };
    router = { navigate: vi.fn() };

    TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: authService },
        { provide: AccountService, useValue: accountService },
        { provide: Router, useValue: router },
      ],
    });
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });

  it('should grant access when current user exists', async () => {
    accountService.currentUser.set(mockUser);

    const result = await firstValueFrom(
      executeGuard(mockRoute, mockState) as Observable<boolean>
    );

    expect(result).toBe(true);
    expect(router.navigate).not.toHaveBeenCalled();
  });

  it('should grant access if user is authenticated', async () => {
    vi.spyOn(authService, 'getAuthStatus').mockReturnValue(
      of({ isAuthenticated: true })
    );

    const result = await firstValueFrom(
      executeGuard(mockRoute, mockState) as Observable<boolean>
    );

    expect(result).toBe(true);
    expect(router.navigate).not.toHaveBeenCalled();
  });

  it('should deny access and redirect to login page if user is not authenticated', async () => {
    vi.spyOn(authService, 'getAuthStatus').mockReturnValue(
      of({ isAuthenticated: false })
    );

    const result = await firstValueFrom(
      executeGuard(mockRoute, mockState) as Observable<boolean>
    );

    expect(result).toBe(false);
    expect(router.navigate).toHaveBeenCalledWith(['/login'], {
      queryParams: { returnUrl: mockState.url },
    });
  });
});
