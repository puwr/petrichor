import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CommentsComponent } from './comments.component';
import { Component, signal } from '@angular/core';
import { AuthFacade } from '../../core/store/auth/auth.facade';
import { By } from '@angular/platform-browser';

describe('Comments', () => {
	it('should create', () => {
		const { commentsComponent } = setup();

		expect(commentsComponent).toBeTruthy();
	});

	it('renders comment-form component when isAuthenticated is true', () => {
		const { fixture, authFacade } = setup();

		authFacade.isAuthenticated.set(true);
		fixture.detectChanges();

		const commentFormComponent = fixture.debugElement.query(By.directive(MockCommentFormComponent));

		expect(commentFormComponent).toBeTruthy();
	});

	it('does not render comment-form component when isAuthenticated is false', () => {
		const { fixture, authFacade } = setup();

		authFacade.isAuthenticated.set(false);
		fixture.detectChanges();

		const commentFormComponent = fixture.debugElement.query(By.directive(MockCommentFormComponent));

		expect(commentFormComponent).toBeNull();
	});

	it('renders comment-list component', () => {
		const { fixture } = setup();

		const commentListComponent = fixture.debugElement.query(By.directive(MockCommentListComponent));

		expect(commentListComponent).toBeTruthy();
	});
});

function setup() {
	const authFacade = {
		isAuthenticated: signal(false),
	};

	TestBed.configureTestingModule({
		imports: [CommentsComponent],
		providers: [
			{
				provide: AuthFacade,
				useValue: authFacade,
			},
		],
	})
		.overrideComponent(CommentsComponent, {
			set: {
				imports: [MockCommentFormComponent, MockCommentListComponent],
			},
		})
		.compileComponents();

	const fixture = TestBed.createComponent(CommentsComponent);
	const commentsComponent = fixture.componentInstance;

	fixture.detectChanges();

	return {
		fixture,
		commentsComponent,
		authFacade,
	};
}

@Component({
	selector: 'app-comment-form',
	template: '',
})
export class MockCommentFormComponent {}

@Component({
	selector: 'app-comment-list',
	template: '',
})
export class MockCommentListComponent {}
