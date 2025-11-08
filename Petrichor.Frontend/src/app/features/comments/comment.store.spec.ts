import { signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { AuthStore } from '@app/core/auth';
import { throwError, of, BehaviorSubject } from 'rxjs';
import { mockCurrentUser } from 'src/test/account.mocks';
import { makeComment } from './comment.models';
import { CommentService } from './comment.service';
import { CommentStore } from './comment.store';

describe('CommentStore', () => {
  it('initializes with correct resourceId and loads comments', () => {
    const { store } = setup();

    expect(store.resourceId()).toBe('1');
    expect(store.hasMore()).toBe(true);
    expect(store.nextCursor()).toEqual('test-cursor');
    expect(store.comments().length).toBe(1);
  });

  describe('createComment', () => {
    it('creates and prepends new comment, calls onSuccess callback', () => {
      const { store } = setup();

      const mockOnSuccess = vi.fn();

      store.createComment({
        message: 'Comment 3',
        onSuccess: () => mockOnSuccess(),
      });

      expect(store.comments().length).toBe(2);
      expect(store.comments()[0].message).toEqual('Comment 3');
      expect(mockOnSuccess).toHaveBeenCalled();
    });

    it('updates validation errors on error', () => {
      const { store, commentService } = setup();

      const mockOnSuccess = vi.fn();
      vi.spyOn(commentService, 'createComment').mockReturnValue(
        throwError(() => ['Error 1', 'Error 2']),
      );

      store.createComment({
        message: 'Comment 3',
        onSuccess: () => mockOnSuccess(),
      });

      expect(store.validationErrors()).toEqual(['Error 1', 'Error 2']);
      expect(store.comments().length).toBe(1);
      expect(mockOnSuccess).not.toHaveBeenCalled();
    });
  });

  describe('deleteComment', () => {
    it('deletes comment', () => {
      const { store } = setup();

      store.deleteComment({ commentId: 'id2' });

      expect(store.comments().length).toBe(0);
    });
  });

  describe('loadMore', () => {
    it('loads and appends additional comments', () => {
      const { store, commentService } = setup();

      vi.spyOn(commentService, 'getComments').mockReturnValue(
        of({
          items: [makeComment('id1', '1', 'Comment 1', mockCurrentUser)],
          nextCursor: null,
          hasMore: false,
        }),
      );

      store.loadMore();

      expect(store.comments().length).toBe(2);
      expect(store.comments().at(-1)?.message).toEqual('Comment 1');
    });
  });
});

function setup() {
  TestBed.configureTestingModule({
    providers: [
      CommentStore,
      {
        provide: CommentService,
        useValue: {
          getComments: vi.fn().mockReturnValue(
            of({
              items: [makeComment('id2', '1', 'Comment 2', mockCurrentUser)],
              hasMore: true,
              nextCursor: 'test-cursor',
            }),
          ),
          createComment: vi.fn().mockReturnValue(of('id3')),
          deleteComment: vi.fn().mockReturnValue(of(null)),
        },
      },
      {
        provide: AuthStore,
        useValue: {
          currentUser: signal(mockCurrentUser),
        },
      },
      {
        provide: ActivatedRoute,
        useValue: {
          paramMap: new BehaviorSubject(convertToParamMap({ id: '1' })).asObservable(),
        },
      },
    ],
  });

  const store = TestBed.inject(CommentStore);
  const commentService = TestBed.inject(CommentService);

  return { store, commentService };
}
