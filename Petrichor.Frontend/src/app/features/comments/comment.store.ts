import { withDevtools } from '@angular-architects/ngrx-toolkit';
import { computed, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthStore } from '@app/core/auth';
import { tapResponse } from '@ngrx/operators';
import {
  signalStore,
  type,
  withState,
  withComputed,
  withProps,
  withMethods,
  patchState,
  withHooks,
} from '@ngrx/signals';
import { withEntities, addEntities, prependEntity, removeEntity } from '@ngrx/signals/entities';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, exhaustMap, of, map, tap } from 'rxjs';
import { Comment, CreateCommentRequest, makeComment } from './comment.models';
import { CommentService } from './comment.service';

interface CommentSlice {
  resourceId: string | null;
  nextCursor: string | null;
}

const initialCommentSlice: CommentSlice = {
  resourceId: null,
  nextCursor: null,
};

export const CommentStore = signalStore(
  withEntities({ entity: type<Comment>(), collection: '_comment' }),
  withState(initialCommentSlice),
  withComputed(({ _commentEntities, nextCursor }) => ({
    comments: _commentEntities,
    hasMore: computed(() => (nextCursor() ? true : false)),
  })),
  withProps((_) => ({
    _commentsService: inject(CommentService),
    _authStore: inject(AuthStore),
  })),
  withMethods((store) => {
    const _getComments = rxMethod<void>(
      pipe(
        switchMap(() =>
          store._commentsService.getComments(store.resourceId()!, store.nextCursor()).pipe(
            tapResponse({
              next: (response) => {
                patchState(
                  store,
                  addEntities([...response.items], {
                    collection: '_comment',
                  }),
                  { nextCursor: response.nextCursor },
                );
              },
              error: console.error,
            }),
          ),
        ),
      ),
    );

    const createComment = (message: string) =>
      store._commentsService
        .createComment({ resourceId: store.resourceId()!, message } as CreateCommentRequest)
        .pipe(
          tap((commentId) => {
            var newComment = makeComment(
              commentId,
              store.resourceId()!,
              message,
              store._authStore.currentUser()!,
            );

            patchState(store, prependEntity(newComment, { collection: '_comment' }));
          }),
        );

    const deleteComment = rxMethod<{ commentId: string }>(
      pipe(
        exhaustMap(({ commentId }) =>
          store._commentsService.deleteComment(commentId).pipe(
            tapResponse({
              next: () => patchState(store, removeEntity(commentId, { collection: '_comment' })),
              error: console.error,
            }),
          ),
        ),
      ),
    );

    return {
      _getComments,
      createComment,
      deleteComment,
      loadMore: () => _getComments(),
    };
  }),
  withHooks({
    onInit(store, route = inject(ActivatedRoute)) {
      of(1)
        .pipe(
          switchMap(() => {
            return route.paramMap.pipe(
              map((params) => params.get('id')),
              tap((id) => {
                if (id) {
                  patchState(store, { resourceId: id });
                  store._getComments();
                } else {
                  console.error('Failed to get resourceId from route.');
                }
              }),
            );
          }),
        )
        .subscribe();
    },
  }),
  withDevtools('comments'),
);
