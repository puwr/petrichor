import { CurrentUser } from '@app/core/account/account.models';
import { PartialStateUpdater } from '@ngrx/signals';
import { AuthSlice } from './auth.slice';

export function setCurrentUser(currentUser: CurrentUser | null): PartialStateUpdater<AuthSlice> {
  return () => ({
    currentUser,
  });
}
