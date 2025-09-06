import { User } from '../../../shared/models/user';

export interface AuthSlice {
	currentUser: User | null;
}

export const initialAuthSlice: AuthSlice = {
	currentUser: null,
};
