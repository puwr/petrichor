import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { LoadingService } from '../services/loading.service';
import { delay, finalize, identity } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SKIP_GLOBAL_LOADING } from '../../shared/constants';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const loadingService = inject(LoadingService);

  if (req.context.get(SKIP_GLOBAL_LOADING)) {
    return next(req);
  }

  loadingService.show();

  return next(req).pipe(
    environment.production ? identity : delay(200),
    finalize(() => loadingService.hide())
  );
};
