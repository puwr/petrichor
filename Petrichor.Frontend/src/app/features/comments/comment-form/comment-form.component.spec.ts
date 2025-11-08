import { signal } from '@angular/core';
import { CommentStore } from '../comment.store';
import { CommentFormComponent } from './comment-form.component';
import { render, screen, waitFor } from '@testing-library/angular';
import userEvent from '@testing-library/user-event';

describe('CommentFormComponent', () => {
  it('enables submit button when form is valid', async () => {
    const { user } = await setup();

    await user.type(screen.getByPlaceholderText(/enter your/i), 'comment message');

    const btn = screen.getByRole('button', { name: /add comment/i });

    expect(btn).not.toBeDisabled();
  });

  it('disables submit button when form is invalid', async () => {
    await setup();

    const btn = screen.getByRole('button', { name: /add comment/i });

    expect(btn).toBeDisabled();
  });

  it('renders validation-errors component when errors are present', async () => {
    const { commentStore } = await setup();

    commentStore.validationErrors.set(['error 1']);

    await waitFor(() => {
      expect(screen.getByRole('alert')).toBeInTheDocument();
      expect(screen.getByText(/error 1/i)).toBeInTheDocument();
    });
  });

  it('does not render validation-errors component when errors are absent', async () => {
    const { commentStore } = await setup();

    commentStore.validationErrors.set(null);

    await waitFor(() => {
      expect(screen.queryByRole('alert')).not.toBeInTheDocument();
    });
  });

  it('resets form on successful submission', async () => {
    const { user } = await setup();

    await user.type(screen.getByPlaceholderText(/enter your/i), 'comment message');

    await user.click(screen.getByRole('button', { name: /add comment/i }));

    expect(screen.queryByText(/comment message/i)).not.toBeInTheDocument();
  });
});

async function setup() {
  const user = userEvent.setup();

  const commentStore = {
    validationErrors: signal<string[] | null>(null),
    createComment: vi.fn().mockImplementation(({ _, onSuccess }) => {
      onSuccess();
    }),
  };

  await render(CommentFormComponent, {
    providers: [
      {
        provide: CommentStore,
        useValue: commentStore,
      },
    ],
  });

  return {
    user,
    commentStore,
  };
}
