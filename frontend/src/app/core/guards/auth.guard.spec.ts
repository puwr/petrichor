import { TestBed } from '@angular/core/testing';
import {
	ActivatedRouteSnapshot,
	CanActivateFn,
	Router,
	RouterStateSnapshot,
} from '@angular/router';
import { signal } from '@angular/core';
import { authGuard } from './auth.guard';
import { AuthStore } from '../store/auth/auth.store';

describe('authGuard', () => {
	it('should be created', () => {
		const { executeGuard } = setup();

		expect(executeGuard).toBeTruthy();
	});

	it('should grant access if authenticated', () => {
		const { executeGuard, authStore, router, route, routerState } = setup();

		authStore.isAuthenticated.set(true);

		const result = executeGuard(route, routerState);

		expect(result).toBe(true);
		expect(router.navigate).not.toHaveBeenCalled();
	});

	it('should deny access and redirect to login page if not authenticated', () => {
		const { executeGuard, authStore, router, route, routerState } = setup();

		authStore.isAuthenticated.set(false);

		const result = executeGuard(route, routerState);

		expect(result).toBe(false);
		expect(router.navigate).toHaveBeenCalledWith(['/login'], {
			queryParams: { returnUrl: routerState.url },
		});
	});
});

function setup() {
	const executeGuard: CanActivateFn = (...guardParameters) =>
		TestBed.runInInjectionContext(() => authGuard(...guardParameters));

	const authStore = { isAuthenticated: signal(false) };
	const route = {} as ActivatedRouteSnapshot;
	const routerState = { url: '/upload' } as RouterStateSnapshot;

	TestBed.configureTestingModule({
		providers: [{ provide: AuthStore, useValue: authStore }],
	});

	const router = TestBed.inject(Router);
	vi.spyOn(router, 'navigate');

	return {
		executeGuard,
		authStore,
		router,
		route,
		routerState,
	};
}
