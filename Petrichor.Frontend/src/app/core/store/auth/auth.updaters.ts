import { PartialStateUpdater } from '@ngrx/signals';
import { User } from '../../../shared/models/user';
import { AuthSlice } from './auth.slice';

export function setCurrentUser(currentUser: User | null): PartialStateUpdater<AuthSlice> {
	return (_) => ({
		currentUser,
	});
}
