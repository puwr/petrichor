import { PartialStateUpdater } from '@ngrx/signals';
import { CommentSlice } from './comment.slice';

export function setValidationErrors(
  validationErrors: string[] | null,
): PartialStateUpdater<CommentSlice> {
  return (_) => ({
    validationErrors,
  });
}

export function setNextCursor(nextCursor: string | null): PartialStateUpdater<CommentSlice> {
  return (_) => ({
    nextCursor,
  });
}
