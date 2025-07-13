import { LoginRequest, RegisterRequest } from '../app/shared/models/auth';
import { User } from '../app/shared/models/user';

export const mockRegisterRequest: RegisterRequest = {
  email: 'test@test.test',
  userName: 'test',
  password: 'Pa$$w0rd',
} as const;

export const mockLoginRequest: LoginRequest = {
  email: 'test.test',
  password: 'Pa$$w0rd',
} as const;

export const mockUser: User = {
  id: 'testId',
  email: 'test@test.test',
  userName: 'test',
} as const;
