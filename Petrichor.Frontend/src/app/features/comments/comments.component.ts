import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { CommentStore } from './store/comment.store';
import { CommentListComponent } from './components/comment-list/comment-list.component';
import { CommentFormComponent } from './components/comment-form/comment-form.component';
import { AuthStore } from '../../core/store/auth/auth.store';

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
