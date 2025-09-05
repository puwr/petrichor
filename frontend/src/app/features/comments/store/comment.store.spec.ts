import { TestBed } from '@angular/core/testing';
import { CommentStore } from './comment.store';
import { AuthFacade } from '../../../core/store/auth/auth.facade';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { BehaviorSubject, of, throwError } from 'rxjs';
import { CommentService } from '../../../core/services/comment.service';
import { signal } from '@angular/core';
import { mockUser } from '../../../../testing/test-data';
import { makeComment } from '../../../shared/models/comment';

describe('CommentStore', () => {
	it('should be created', () => {
		const { store } = setup();

		expect(store).toBeTruthy();
	});

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
					items: [makeComment('id1', '1', 'Comment 1', mockUser)],
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
							items: [makeComment('id2', '1', 'Comment 2', mockUser)],
							hasMore: true,
							nextCursor: 'test-cursor',
						}),
					),
					createComment: vi.fn().mockReturnValue(of('id3')),
					deleteComment: vi.fn().mockReturnValue(of(null)),
				},
			},
			{
				provide: AuthFacade,
				useValue: {
					currentUser: signal(mockUser),
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
