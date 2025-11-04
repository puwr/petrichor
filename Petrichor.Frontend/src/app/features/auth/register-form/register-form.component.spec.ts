import { render, screen } from '@testing-library/angular';
import userEvent from '@testing-library/user-event';
import { RegisterFormComponent } from './register-form.component';
import { AuthStore, RegisterRequest } from '@app/core/auth';

describe('RegisterFormComponent', () => {
  it('enables register button when form is valid', async () => {
    const { user, userData } = await setup();

    await user.type(screen.getByLabelText(/email/i), userData.email);
    await user.type(screen.getByLabelText(/userName/i), userData.userName);
    await user.type(screen.getByLabelText(/password/i), userData.password);

    const btn = screen.getByRole('button', { name: /register/i });

    expect(btn).not.toBeDisabled();
  });

  it('disables register button when form is invalid', async () => {
    await setup();

    const btn = screen.getByRole('button', { name: /register/i });

    expect(btn).toBeDisabled();
  });

  it('renders validation-errors component when errors are present', async () => {
    const { user, userData, authStore } = await setup();
    const mockErrors = ['validation error 1'];

    authStore.register.mockImplementation((args) => args.onError(mockErrors));

    await user.type(screen.getByLabelText(/email/i), userData.email);
    await user.type(screen.getByLabelText(/userName/i), userData.userName);
    await user.type(screen.getByLabelText(/password/i), userData.password);
    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(screen.getByRole('alert')).toBeInTheDocument();
    expect(screen.getByText(/validation error 1/i)).toBeInTheDocument();
  });

  it('does not render validation-errors component when errors are absent', async () => {
    const { user, userData, authStore } = await setup();

    await user.type(screen.getByLabelText(/email/i), userData.email);
    await user.type(screen.getByLabelText(/userName/i), userData.userName);
    await user.type(screen.getByLabelText(/password/i), userData.password);

    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(authStore.register).toHaveBeenCalledWith({
      userData,
      onError: expect.any(Function),
    });
    expect(screen.queryByRole('alert')).not.toBeInTheDocument();
  });
});

async function setup() {
  const user = userEvent.setup();

  const authStore = {
    register: vi.fn(),
  };

  await render(RegisterFormComponent, {
    providers: [
      {
        provide: AuthStore,
        useValue: authStore,
      },
    ],
  });

  const userData: RegisterRequest = {
    email: 'test@test.test',
    userName: 'test',
    password: 'Pa$$w0rd',
  };

  return {
    user,
    userData,
    authStore,
  };
}
