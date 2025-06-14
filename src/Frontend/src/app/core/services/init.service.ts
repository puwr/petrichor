import { inject, Injectable } from '@angular/core';
import { AccountService } from './account.service';
import {
  catchError,
  ignoreElements,
  map,
  Observable,
  of,
  startWith,
  take,
  throwError,
} from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class InitService {
  private accountService = inject(AccountService);

  initialize(): Observable<void> {
    return this.accountService.updateCurrentUser();
  }
}
