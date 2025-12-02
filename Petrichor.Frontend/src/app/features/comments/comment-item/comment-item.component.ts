import { DatePipe } from '@angular/common';
import { Component, ChangeDetectionStrategy, inject, input, output, computed } from '@angular/core';
import { AuthStore } from '@app/core/auth';
import { ButtonComponent, IconComponent } from '@app/shared/components';
import { Comment } from '../comment.models';

@Component({
  selector: 'app-comment-item',
  imports: [DatePipe, ButtonComponent, IconComponent],
  templateUrl: './comment-item.component.html',
  styleUrl: './comment-item.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommentItemComponent {
  readonly authStore = inject(AuthStore);

  comment = input.required<Comment>();
  commentDeleted = output<string>();

  isAuthorOrAdmin = computed(() => this.authStore.isResourceOwnerOrAdmin(this.comment().author));
}
