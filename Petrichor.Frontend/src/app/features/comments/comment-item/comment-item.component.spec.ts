import { CommentItemComponent } from './comment-item.component';
import { mockCurrentUser } from '../../../../test/account.mocks';
import { AuthStore } from '../../../core/auth/auth.store';
import { makeComment } from '@app/features/comments/comment.models';
import { inputBinding, outputBinding } from '@angular/core';
import { render, screen } from '@testing-library/angular';
import userEvent from '@testing-library/user-event';

describe('CommentItemComponent', () => {
  it('displays comment message and details', async () => {
    await setup();

    expect(screen.getByText(mockCurrentUser.userName)).toBeInTheDocument();
    expect(screen.getByText(/comment message/i)).toBeInTheDocument();
  });

  it('renders delete button when isAuthorOrAdmin is true', async () => {
    await setup({ isAuthorOrAdmin: true });

    expect(screen.getByRole('button', { name: /delete comment/i })).toBeInTheDocument();
  });

  it('does not render delete button when isAuthorOrAdmin is false', async () => {
    await setup();

    expect(screen.queryByRole('button', { name: /delete comment/i })).not.toBeInTheDocument();
  });

  it('emits commentDeleted event with commentId', async () => {
    const { user, commentDeletedSpy } = await setup({ isAuthorOrAdmin: true });

    await user.click(screen.getByRole('button', { name: /delete comment/i }));

    expect(commentDeletedSpy).toHaveBeenCalledWith('id1');
  });
});

async function setup(config: { isAuthorOrAdmin?: boolean } = {}) {
  const commentDeletedSpy = vi.fn();
  const user = userEvent.setup();

  await render(CommentItemComponent, {
    providers: [
      {
        provide: AuthStore,
        useValue: {
          isResourceOwnerOrAdmin: vi.fn().mockReturnValue(config.isAuthorOrAdmin ?? false),
        },
      },
    ],
    bindings: [
      inputBinding('comment', () => makeComment('id1', '1', 'comment message', mockCurrentUser)),
      outputBinding('commentDeleted', commentDeletedSpy),
    ],
  });

  return { user, commentDeletedSpy };
}
