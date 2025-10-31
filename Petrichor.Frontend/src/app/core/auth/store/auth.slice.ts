import { CurrentUser } from '@app/core/account/account.models';

export interface AuthSlice {
  currentUser: CurrentUser | null;
}

export const initialAuthSlice: AuthSlice = {
  currentUser: null,
};
