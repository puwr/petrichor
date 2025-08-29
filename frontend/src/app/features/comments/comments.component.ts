import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  inject,
  input,
  OnInit,
  signal,
} from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommentService } from '../../core/services/comment.service';
import { Comment, CreateCommentRequest } from '../../shared/models/comment';
import {
  BehaviorSubject,
  catchError,
  combineLatest,
  EMPTY,
  exhaustMap,
  of,
  tap,
} from 'rxjs';
import { CursorPagedResponse } from '../../shared/models/pagination';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { AuthFacade } from '../../core/stores/auth/auth.facade';
import { CommentComponent } from './comment/comment.component';
import { ValidationErrorsComponent } from '../../shared/components/validation-errors/validation-errors.component';

@Component({
  selector: 'app-comments',
  imports: [ReactiveFormsModule, CommentComponent, ValidationErrorsComponent],
  templateUrl: './comments.component.html',
  styleUrl: './comments.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommentsComponent implements OnInit {
  private fb = inject(FormBuilder);
  private authFacade = inject(AuthFacade);
  private commentService = inject(CommentService);
  private destroyRef = inject(DestroyRef);

  private currentUser = this.authFacade.currentUser;
  isAuthenticated = this.authFacade.isAuthenticated;

  resourceId = input.required<string>();
  resourceId$ = toObservable(this.resourceId);

  comments = signal<Comment[]>([]);
  nextCursor = signal<string | null>(null);
  hasMore = signal<boolean>(true);
  validationErrors: string[] | null = null;

  private cursorSubject = new BehaviorSubject<string | null>(null);

  commentForm = this.fb.group({
    comment: ['', Validators.required],
  });

  ngOnInit(): void {
    combineLatest([this.resourceId$, this.cursorSubject.asObservable()])
      .pipe(
        exhaustMap(([resourceId, cursor]) => {
          if (!this.hasMore()) return of(null);

          return this.commentService.getComments(resourceId, cursor).pipe(
            tap((response: CursorPagedResponse<Comment>) => {
              if (response) {
                this.comments.update((prev) => [...prev, ...response.items]);
                this.nextCursor.set(response.nextCursor);
                this.hasMore.set(response.hasMore);
              }
            }),
            takeUntilDestroyed(this.destroyRef)
          );
        })
      )
      .subscribe();
  }

  loadMore(): void {
    this.cursorSubject.next(this.nextCursor());
  }

  onSubmit(): void {
    var commentData: CreateCommentRequest = {
      resourceId: this.resourceId(),
      message: this.commentForm.value.comment!.trim(),
    };

    this.commentService
      .createComment(commentData)
      .pipe(
        tap((commentId) => {
          var newComment: Comment = {
            id: commentId,
            resourceId: this.resourceId(),
            authorId: this.currentUser()!.id,
            authorUserName: this.currentUser()!.userName,
            message: this.commentForm.value.comment!.trim(),
            createdAtUtc: new Date(),
          };

          this.comments.update((prev) => [newComment, ...prev]);
          this.commentForm.reset();
          this.validationErrors = null;
        }),
        catchError((errors) => {
          this.validationErrors = errors;
          return EMPTY;
        }),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe();
  }

  onCommentDelete(commentId: string): void {
    this.commentService
      .deleteComment(commentId)
      .pipe(
        tap(() => {
          this.comments.update((prev) =>
            prev.filter((c) => c.id !== commentId)
          );
        }),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe();
  }
}
