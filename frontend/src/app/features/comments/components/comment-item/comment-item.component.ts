import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, input, output } from '@angular/core';
import { Comment } from '../../../../shared/models/comment';
import { AuthFacade } from '../../../../core/store/auth/auth.facade';

@Component({
	selector: 'app-comment-item',
	imports: [DatePipe],
	templateUrl: './comment-item.component.html',
	styleUrl: './comment-item.component.scss',
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommentItemComponent {
	private authFacade = inject(AuthFacade);

	comment = input.required<Comment>();
	commentDeleted = output<string>();

	isAuthorOrAdmin = computed(() => this.authFacade.isResourceOwnerOrAdmin(this.comment().authorId));
}
