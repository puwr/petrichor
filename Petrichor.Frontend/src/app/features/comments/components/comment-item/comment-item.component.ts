import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, input, output } from '@angular/core';
import { Comment } from '../../../../shared/models/comment';
import { AuthStore } from '../../../../core/store/auth/auth.store';
import { ButtonComponent } from '../../../../shared/components/button/button.component';

@Component({
  selector: 'app-comment-item',
  imports: [DatePipe, ButtonComponent],
  templateUrl: './comment-item.component.html',
  styleUrl: './comment-item.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommentItemComponent {
  readonly authStore = inject(AuthStore);

  comment = input.required<Comment>();
  commentDeleted = output<string>();

  isAuthorOrAdmin = computed(() => this.authStore.isResourceOwnerOrAdmin(this.comment().authorId));
}
