import { signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { ButtonComponent, ValidationErrorsComponent } from '@app/shared/components';
import { CommentStore } from '../store/comment.store';
import { CommentFormComponent } from './comment-form.component';

describe('CommentFormComponent', () => {
  it('should create', () => {
    const { commentFormComponent } = setup();

    expect(commentFormComponent).toBeTruthy();
  });

  it('enables submit button when form is valid', () => {
    const { fixture, commentFormComponent } = setup();

    commentFormComponent.commentForm.patchValue({ comment: 'Comment' });
    fixture.detectChanges();

    const btn = fixture.debugElement.query(By.directive(ButtonComponent));

    expect(btn.componentInstance.disabled()).toBe(false);
  });

  it('disables submit button when form is invalid', () => {
    const { fixture, commentFormComponent } = setup();

    commentFormComponent.commentForm.patchValue({ comment: '' });
    fixture.detectChanges();

    const btn = fixture.debugElement.query(By.directive(ButtonComponent));

    expect(btn.componentInstance.disabled()).toBe(true);
  });

  it('renders validation-errors component when errors are present', () => {
    const { fixture, commentStore } = setup();

    commentStore.validationErrors.set(['Error 1']);
    fixture.detectChanges();

    var validationErrorsComponent = fixture.debugElement.query(
      By.directive(ValidationErrorsComponent),
    );

    expect(validationErrorsComponent).toBeTruthy();
  });

  it('does not render validation-errors component when errors are absent', () => {
    const { fixture, commentStore } = setup();

    commentStore.validationErrors.set(null);
    fixture.detectChanges();

    var validationErrorsComponent = fixture.debugElement.query(
      By.directive(ValidationErrorsComponent),
    );

    expect(validationErrorsComponent).toBeNull();
  });

  describe('onSubmit', () => {
    it('resets form on successful submission', () => {
      const { fixture, commentFormComponent } = setup();

      commentFormComponent.commentForm.patchValue({ comment: 'Comment' });
      fixture.detectChanges();

      commentFormComponent.onSubmit();

      expect(commentFormComponent.commentForm.value.comment).toBeNull();
    });
  });
});

function setup() {
  const commentStore = {
    validationErrors: signal<string[] | null>(null),
    createComment: vi.fn().mockImplementation(({ _, onSuccess }) => {
      onSuccess();
    }),
  };

  TestBed.configureTestingModule({
    imports: [CommentFormComponent],
    providers: [
      {
        provide: CommentStore,
        useValue: commentStore,
      },
    ],
  }).compileComponents();

  const fixture = TestBed.createComponent(CommentFormComponent);
  const commentFormComponent = fixture.componentInstance;
  fixture.detectChanges();

  return {
    fixture,
    commentFormComponent,
    commentStore,
  };
}
