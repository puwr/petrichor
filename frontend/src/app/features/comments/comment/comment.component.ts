import { DatePipe } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  input,
  output,
} from '@angular/core';
import { Comment } from '../../../shared/models/comment';
import { AuthFacade } from '../../../core/stores/auth/auth.facade';

@Component({
  selector: 'app-comment',
  imports: [DatePipe],
  templateUrl: './comment.component.html',
  styleUrl: './comment.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommentComponent {
  private authFacade = inject(AuthFacade);

  comment = input.required<Comment>();
  commentDeleted = output<string>();

  isAuthorOrAdmin = computed(() =>
    this.authFacade.isResourceOwnerOrAdmin(this.comment().authorId)
  );
}
