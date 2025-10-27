import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { CommentStore } from '../../store/comment.store';
import { CommentItemComponent } from '../comment-item/comment-item.component';
import { ButtonComponent } from '../../../../shared/components/button/button.component';

@Component({
  selector: 'app-comment-list',
  imports: [CommentItemComponent, ButtonComponent],
  templateUrl: './comment-list.component.html',
  styleUrl: './comment-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommentListComponent {
  readonly commentStore = inject(CommentStore);
}
