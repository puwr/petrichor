import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ValidationErrorsComponent, ButtonComponent } from '@app/shared/components';
import { CommentStore } from '../comment.store';

@Component({
  selector: 'app-comment-form',
  imports: [ReactiveFormsModule, ValidationErrorsComponent, ButtonComponent],
  templateUrl: './comment-form.component.html',
  styleUrl: './comment-form.component.scss',
})
export class CommentFormComponent {
  private fb = inject(FormBuilder);
  readonly commentStore = inject(CommentStore);

  commentForm = this.fb.group({
    comment: ['', Validators.required],
  });

  onSubmit(): void {
    this.commentStore.createComment({
      message: this.commentForm.value.comment!.trim(),
      onSuccess: () => this.commentForm.reset(),
    });
  }
}
