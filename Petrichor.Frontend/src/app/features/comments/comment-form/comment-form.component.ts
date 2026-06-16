import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ValidationErrorsComponent, ButtonComponent } from '@app/shared/components';
import { CommentStore } from '../comment.store';
import { form, FormField, required, submit } from '@angular/forms/signals';
import { catchError, firstValueFrom, map, of } from 'rxjs';

@Component({
  selector: 'app-comment-form',
  imports: [ValidationErrorsComponent, ButtonComponent, FormField],
  templateUrl: './comment-form.component.html',
  styleUrl: './comment-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommentFormComponent {
  readonly commentStore = inject(CommentStore);

  commentForm = form(signal({ comment: '' }), (schema) => {
    required(schema.comment, { message: '' });
  });

  async submitCommentForm(): Promise<void> {
    await submit(this.commentForm, async (form) => {
      if (form().invalid()) return;

      return await firstValueFrom(
        this.commentStore.createComment(form().value().comment).pipe(
          map(() => {
            form().reset();
            form().value.set({ comment: '' });
            return null;
          }),
          catchError((errors) => of(errors)),
        ),
      );
    });
  }

  async onKeyDown(event: KeyboardEvent): Promise<void> {
    if ((event.ctrlKey || event.metaKey) && event.key === 'Enter') {
      event.preventDefault();

      await this.submitCommentForm();
    }
  }

  async onSubmit(event: SubmitEvent): Promise<void> {
    event?.preventDefault();

    await this.submitCommentForm();
  }
}
