import { TestBed } from '@angular/core/testing';
import { CommentListComponent } from './comment-list.component';
import { Component, input, output, signal } from '@angular/core';
import { Comment, makeComment } from '../../../../shared/models/comment';
import { mockUser } from '../../../../../testing/test-data';
import { CommentStore } from '../../store/comment.store';
import { By } from '@angular/platform-browser';
import { AuthStore } from '../../../../core/store/auth/auth.store';

describe('CommentListComponent', () => {
	it('should create', () => {
		const { commentListcomponent } = setup();

		expect(commentListcomponent).toBeTruthy();
	});

	it('renders comment-item component for each comment', () => {
		const { fixture, commentStore } = setup();

		const comments: Comment[] = [
			makeComment('id1', '1', 'Comment 1', mockUser),
			makeComment('id2', '1', 'Comment 2', mockUser),
		];

		commentStore.comments.set(comments);
		fixture.detectChanges();

		const commentItems = fixture.debugElement.queryAll(By.directive(MockCommentItemComponent));

		expect(commentItems.length).toBe(2);
		expect(commentItems[0].componentInstance.comment()).toEqual(comments[0]);
		expect(commentItems[1].componentInstance.comment()).toEqual(comments[1]);
	});

	it('calls commentStore.deleteComment when commentDeleted is emitted', () => {
		const { fixture, commentStore } = setup();

		commentStore.comments.set([makeComment('id1', '1', 'Comment 1', mockUser)]);
		fixture.detectChanges();

		const commentItem = fixture.debugElement.query(By.directive(MockCommentItemComponent));
		commentItem.componentInstance.commentDeleted.emit('id1');
		fixture.detectChanges();

		expect(commentStore.deleteComment).toHaveBeenCalledWith({ commentId: 'id1' });
	});

	it('renders "Load more" button when hasMore is true', () => {
		const { fixture, commentStore } = setup();

		commentStore.hasMore.set(true);
		fixture.detectChanges();

		const btn = fixture.debugElement.query(By.css('.comment-list__load-more'));
		expect(btn.nativeElement.textContent).toContain('Load more');
	});

	it('does not render "Load more" button when hasMore is false', () => {
		const { fixture, commentStore } = setup();

		commentStore.hasMore.set(false);
		fixture.detectChanges();

		const btn = fixture.debugElement.query(By.css('.comment-list__load-more'));
		expect(btn).toBeNull();
	});

	it('calls commentStore.loadMore when "Load more" button is clicked', () => {
		const { fixture, commentStore } = setup();

		commentStore.hasMore.set(true);
		fixture.detectChanges();

		const btn = fixture.debugElement.query(By.css('.comment-list__load-more'));

		btn.triggerEventHandler('click');
		fixture.detectChanges();

		expect(commentStore.loadMore).toHaveBeenCalled();
	});
});

function setup() {
	const commentStore = {
		comments: signal<Comment[]>([]),
		hasMore: signal(false),
		deleteComment: vi.fn(),
		loadMore: vi.fn(),
	};

	TestBed.configureTestingModule({
		imports: [CommentListComponent],
		providers: [
			{
				provide: AuthStore,
				useValue: {
					isResourceOwnerOrAdmin: vi.fn().mockReturnValue(false),
				},
			},
			{
				provide: CommentStore,
				useValue: commentStore,
			},
		],
	})
		.overrideComponent(CommentListComponent, {
			set: {
				imports: [MockCommentItemComponent],
			},
		})
		.compileComponents();

	const fixture = TestBed.createComponent(CommentListComponent);
	const commentListcomponent = fixture.componentInstance;
	fixture.detectChanges();

	return {
		fixture,
		commentListcomponent,
		commentStore,
	};
}

@Component({
	selector: 'app-comment-item',
	template: '',
})
export class MockCommentItemComponent {
	comment = input.required<Comment>();
	commentDeleted = output<void>();
}
