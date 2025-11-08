import { signal } from '@angular/core';
import { AuthStore } from '@app/core/auth';
import { mockCurrentUser } from 'src/test/account.mocks';
import { Comment, makeComment } from '../comment.models';
import { CommentStore } from '../comment.store';
import { CommentListComponent } from './comment-list.component';
import { render, waitFor, screen } from '@testing-library/angular';
import { CommentItemComponent } from '../comment-item/comment-item.component';
import userEvent from '@testing-library/user-event';

describe('CommentListComponent', () => {
  it('renders comment-item component for each comment', async () => {
    const { commentStore } = await setup();

    commentStore.comments.set([
      makeComment('id1', '1', 'comment message 1', mockCurrentUser),
      makeComment('id2', '1', 'comment message 2', mockCurrentUser),
    ]);

    await waitFor(() => {
      expect(screen.getByText(/comment message 1/i)).toBeInTheDocument();
      expect(screen.getByText(/comment message 2/i)).toBeInTheDocument();
    });
  });

  it('calls commentStore.deleteComment when commentDeleted is emitted', async () => {
    const { user, commentStore } = await setup();

    commentStore.comments.set([makeComment('id1', '1', 'comment message 1', mockCurrentUser)]);

    await waitFor(() => expect(screen.getByText(/comment message 1/i)).toBeInTheDocument());

    await user.click(screen.getByRole('button', { name: /delete comment/i }));

    expect(commentStore.deleteComment).toHaveBeenCalledWith({ commentId: 'id1' });
  });

  it('renders "Load more" button when hasMore is true', async () => {
    const { commentStore } = await setup();

    commentStore.hasMore.set(true);

    await waitFor(() =>
      expect(screen.getByRole('button', { name: /load more/i })).toBeInTheDocument(),
    );
  });

  it('does not render "Load more" button when hasMore is false', async () => {
    const { commentStore } = await setup();

    commentStore.hasMore.set(false);

    await waitFor(() =>
      expect(screen.queryByRole('button', { name: /load more/i })).not.toBeInTheDocument(),
    );
  });

  it('calls commentStore.loadMore when "Load more" button is clicked', async () => {
    const { user, commentStore } = await setup();

    commentStore.hasMore.set(true);
    await waitFor(() =>
      expect(screen.getByRole('button', { name: /load more/i })).toBeInTheDocument(),
    );

    await user.click(screen.getByRole('button', { name: /load more/i }));

    expect(commentStore.loadMore).toHaveBeenCalled();
  });
});

async function setup() {
  const user = userEvent.setup();
  const commentStore = {
    comments: signal<Comment[]>([]),
    hasMore: signal(false),
    deleteComment: vi.fn(),
    loadMore: vi.fn(),
  };

  await render(CommentListComponent, {
    providers: [
      {
        provide: CommentStore,
        useValue: commentStore,
      },
    ],
    childComponentOverrides: [
      {
        component: CommentItemComponent,
        providers: [
          {
            provide: AuthStore,
            useValue: { isResourceOwnerOrAdmin: vi.fn().mockReturnValue(true) },
          },
        ],
      },
    ],
  });

  return { user, commentStore };
}
