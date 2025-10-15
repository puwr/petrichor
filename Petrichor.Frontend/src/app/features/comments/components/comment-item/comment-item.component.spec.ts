import { TestBed } from '@angular/core/testing';
import { CommentItemComponent } from './comment-item.component';
import { makeComment } from '../../../../shared/models/comment';
import { mockUser } from '../../../../../testing/test-data';
import { By } from '@angular/platform-browser';
import { AuthStore } from '../../../../core/store/auth/auth.store';

describe('CommentItemComponent', () => {
	it('should create', () => {
		const { commentItemComponent } = setup();

		expect(commentItemComponent).toBeTruthy();
	});

	it('displays comment details', () => {
		const { fixture } = setup();

		const commentDetails = fixture.debugElement.query(By.css('.comment__details'));

		expect(commentDetails.nativeElement.textContent).toContain(mockUser.userName);
	});

	it('displays comment message', () => {
		const { fixture } = setup();

		const commentDetails = fixture.debugElement.query(By.css('.comment__message'));

		expect(commentDetails.nativeElement.textContent).toEqual('Comment 1');
	});

	it('renders delete button when isAuthorOrAdmin is true', () => {
		const { fixture, authStore } = setup();

		vi.spyOn(authStore, 'isResourceOwnerOrAdmin').mockReturnValue(true);

		fixture.componentRef.setInput('comment', makeComment('id1', '1', 'Comment 1', mockUser));
		fixture.detectChanges();

		const btn = fixture.debugElement.query(By.css('.comment__delete'));

		expect(btn).toBeTruthy();
	});

	it('does not render delete button when isAuthorOrAdmin is false', () => {
		const { fixture, authStore } = setup();

		vi.spyOn(authStore, 'isResourceOwnerOrAdmin').mockReturnValue(false);

		fixture.componentRef.setInput('comment', makeComment('id1', '1', 'Comment 1', mockUser));
		fixture.detectChanges();

		const btn = fixture.debugElement.query(By.css('.comment__delete'));

		expect(btn).toBeNull();
	});
});

function setup() {
	const authStore = {
		isResourceOwnerOrAdmin: vi.fn().mockReturnValue(false),
	};

	TestBed.configureTestingModule({
		imports: [CommentItemComponent],
		providers: [
			{
				provide: AuthStore,
				useValue: authStore,
			},
		],
	}).compileComponents();

	const fixture = TestBed.createComponent(CommentItemComponent);
	const commentItemComponent = fixture.componentInstance;

	fixture.componentRef.setInput('comment', makeComment('id1', '1', 'Comment 1', mockUser));
	fixture.detectChanges();

	return {
		fixture,
		commentItemComponent,
		authStore,
	};
}
