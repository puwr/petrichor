import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { identity, delay, finalize } from 'rxjs';
import { environment } from 'src/environments/environment';
import { SKIP_GLOBAL_LOADING } from '../http-tokens';
import { LoadingService } from './loading.service';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const loadingService = inject(LoadingService);

  if (req.context.get(SKIP_GLOBAL_LOADING)) {
    return next(req);
  }

  loadingService.show();

  return next(req).pipe(
    environment.production ? identity : delay(200),
    finalize(() => loadingService.hide()),
  );
};
