import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { AccountService } from '../services/account.service';
import { map, Observable, of } from 'rxjs';

export const authGuard: CanActivateFn = (route, state): Observable<boolean> => {
  const router = inject(Router);
  const accountService = inject(AccountService);
  const authService = inject(AuthService);

  if (accountService.currentUser()) {
    return of(true);
  }

  return authService.getAuthStatus().pipe(
    map((authStatus) => {
      if (authStatus.isAuthenticated) {
        return true;
      } else {
        router.navigate(['/login'], {
          queryParams: { returnUrl: state.url },
        });

        return false;
      }
    })
  );
};
