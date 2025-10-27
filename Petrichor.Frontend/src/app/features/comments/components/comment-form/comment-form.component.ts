import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommentStore } from '../../store/comment.store';
import { ValidationErrorsComponent } from '../../../../shared/components/validation-errors/validation-errors.component';
import { ButtonComponent } from '../../../../shared/components/button/button.component';

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
