import { Component } from '@angular/core';
import { signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { AuthStore } from '@app/core/auth';
import { CommentsComponent } from './comments.component';

describe('Comments', () => {
  it('should create', () => {
    const { commentsComponent } = setup();

    expect(commentsComponent).toBeTruthy();
  });

  it('renders comment-form component when isAuthenticated is true', () => {
    const { fixture, authStore } = setup();

    authStore.isAuthenticated.set(true);
    fixture.detectChanges();

    const commentFormComponent = fixture.debugElement.query(By.directive(MockCommentFormComponent));

    expect(commentFormComponent).toBeTruthy();
  });

  it('does not render comment-form component when isAuthenticated is false', () => {
    const { fixture, authStore } = setup();

    authStore.isAuthenticated.set(false);
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
  const authStore = {
    isAuthenticated: signal(false),
  };

  TestBed.configureTestingModule({
    imports: [CommentsComponent],
    providers: [
      {
        provide: AuthStore,
        useValue: authStore,
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
    authStore,
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
