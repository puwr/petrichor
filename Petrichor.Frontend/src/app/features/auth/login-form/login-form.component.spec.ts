import { ValidationError } from '@angular/forms/signals';
import { LoginFormComponent } from './login-form.component';
import { AuthStore } from '@app/core/auth';
import { render, screen, waitFor } from '@testing-library/angular';
import { userEvent } from '@testing-library/user-event';
import { of, throwError } from 'rxjs';

describe('LoginFormComponent', () => {
  it('enables login button when form is valid', async () => {
    const { user } = await setup();

    await user.type(screen.getByLabelText(/email/i), 'test@test.test');
    await user.type(screen.getByLabelText(/password/i), 'test');

    const btn = screen.getByRole('button', { name: /login/i });

    expect(btn).not.toBeDisabled();
  });

  it('disables login button when form is invalid', async () => {
    await setup();

    const btn = screen.getByRole('button', { name: /login/i });

    expect(btn).toBeDisabled();
  });

  it('renders validation-errors component when errors are present', async () => {
    const { user, authStore } = await setup();
    const mockErrors: ValidationError[] = [
      {
        kind: 'server',
        message: 'validation error 1',
      },
    ];

    authStore.login.mockReturnValue(throwError(() => mockErrors));

    await user.type(screen.getByLabelText(/email/i), 'test@test.test');
    await user.type(screen.getByLabelText(/password/i), 'test');

    await user.click(screen.getByRole('button', { name: /login/i }));

    await waitFor(() => {
      expect(screen.getByRole('alert')).toBeInTheDocument();
      expect(screen.getByText(/validation error 1/i)).toBeInTheDocument();
    });
  });

  it('does not render validation-errors component when errors are absent', async () => {
    const { user, authStore } = await setup();

    await user.type(screen.getByLabelText(/email/i), 'test@test.test');
    await user.type(screen.getByLabelText(/password/i), 'test');

    await user.click(screen.getByRole('button', { name: /login/i }));

    await waitFor(() => {
      expect(authStore.login).toHaveBeenCalledWith({ email: 'test@test.test', password: 'test' });
      expect(screen.queryByRole('alert')).not.toBeInTheDocument();
    });
  });
});

async function setup() {
  const user = userEvent.setup();

  const authStore = {
    login: vi.fn().mockReturnValue(of(undefined)),
  };

  await render(LoginFormComponent, {
    providers: [
      {
        provide: AuthStore,
        useValue: authStore,
      },
    ],
  });

  return {
    user,
    authStore,
  };
}
