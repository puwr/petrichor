import { CommentStore } from '../comment.store';
import { CommentFormComponent } from './comment-form.component';
import { render, screen, waitFor } from '@testing-library/angular';
import userEvent from '@testing-library/user-event';
import { of, throwError } from 'rxjs';
import { ValidationError } from '@angular/forms/signals';

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
    const { commentStore, user } = await setup();

    const mockValidationErrors: ValidationError[] = [{ kind: 'server', message: 'error 1' }];
    commentStore.createComment.mockReturnValue(throwError(() => mockValidationErrors));

    await user.type(screen.getByPlaceholderText(/enter your/i), 'comment message');
    await user.click(screen.getByRole('button', { name: /add comment/i }));

    await waitFor(() => {
      expect(screen.getByRole('alert')).toBeInTheDocument();
      expect(screen.getByText(/error 1/i)).toBeInTheDocument();
    });
  });

  it('does not render validation-errors component when errors are absent', async () => {
    const { user } = await setup();

    await user.type(screen.getByPlaceholderText(/enter your/i), 'comment message');
    await user.click(screen.getByRole('button', { name: /add comment/i }));

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
    createComment: vi.fn().mockReturnValue(of(undefined)),
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
