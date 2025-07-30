import { TestBed } from '@angular/core/testing';
import {
  ActivatedRouteSnapshot,
  CanActivateFn,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { signal, WritableSignal } from '@angular/core';
import { authGuard } from './auth.guard';
import { AuthFacade } from '../stores/auth/auth.facade';

describe('authGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) =>
    TestBed.runInInjectionContext(() => authGuard(...guardParameters));

  let authFacade: { isAuthenticated: WritableSignal<boolean> };
  let router: Router;

  const mockRoute = {} as ActivatedRouteSnapshot;
  const mockState = { url: '/upload' } as RouterStateSnapshot;

  beforeEach(() => {
    authFacade = { isAuthenticated: signal<boolean>(false) };

    TestBed.configureTestingModule({
      providers: [{ provide: AuthFacade, useValue: authFacade }],
    });

    router = TestBed.inject(Router);
    vi.spyOn(router, 'navigate');
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });

  it('should grant access if authenticated', () => {
    authFacade.isAuthenticated.set(true);

    const result = executeGuard(mockRoute, mockState);

    expect(result).toBe(true);
    expect(router.navigate).not.toHaveBeenCalled();
  });

  it('should deny access and redirect to login page if not authenticated', () => {
    const result = executeGuard(mockRoute, mockState);

    expect(result).toBe(false);
    expect(router.navigate).toHaveBeenCalledWith(['/login'], {
      queryParams: { returnUrl: mockState.url },
    });
  });
});
