import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthFacade } from '../store/auth/auth.facade';

export const authGuard: CanActivateFn = (route, state): boolean => {
  const router = inject(Router);
  const authFacade = inject(AuthFacade);

  if (authFacade.isAuthenticated()) {
    return true;
  }

  router.navigate(['/login'], {
    queryParams: { returnUrl: state.url },
  });

  return false;
};
