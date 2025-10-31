import { Component, ChangeDetectionStrategy, inject } from '@angular/core';
import { AuthStore } from '@app/core/auth';
import { CommentFormComponent } from './comment-form/comment-form.component';
import { CommentListComponent } from './comment-list/comment-list.component';
import { CommentStore } from './store/comment.store';

@Component({
  selector: 'app-comments',
  imports: [CommentListComponent, CommentFormComponent],
  templateUrl: './comments.component.html',
  styleUrl: './comments.component.scss',
  providers: [CommentStore],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommentsComponent {
  readonly authStore = inject(AuthStore);
}
