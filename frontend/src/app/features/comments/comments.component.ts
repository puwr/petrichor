import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { AuthFacade } from '../../core/store/auth/auth.facade';
import { CommentStore } from './store/comment.store';
import { CommentListComponent } from './components/comment-list/comment-list.component';
import { CommentFormComponent } from './components/comment-form/comment-form.component';

@Component({
	selector: 'app-comments',
	imports: [CommentListComponent, CommentFormComponent],
	templateUrl: './comments.component.html',
	styleUrl: './comments.component.scss',
	providers: [CommentStore],
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommentsComponent {
	readonly authFacade = inject(AuthFacade);
}
