import { CurrentUser } from '../app/core/account/account.models';

export const mockCurrentUser: CurrentUser = {
  id: 'testId',
  email: 'test@test.test',
  userName: 'test',
  roles: [],
} as const;
