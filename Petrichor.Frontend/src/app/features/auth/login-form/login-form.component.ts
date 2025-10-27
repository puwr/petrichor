import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TextInputComponent } from '../../../shared/components/text-input/text-input.component';
import { LoginRequest } from '../../../shared/models/auth';
import { ValidationErrorsComponent } from '../../../shared/components/validation-errors/validation-errors.component';
import { AuthStore } from '../../../core/store/auth/auth.store';
import { ButtonComponent } from '../../../shared/components/button/button.component';

@Component({
  selector: 'app-login-form',
  imports: [ReactiveFormsModule, TextInputComponent, ValidationErrorsComponent, ButtonComponent],
  templateUrl: './login-form.component.html',
  styleUrl: './login-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginFormComponent {
  private fb = inject(FormBuilder);
  private authStore = inject(AuthStore);

  validationErrors = signal<string[] | null>(null);

  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  onSubmit(): void {
    this.authStore.login({
      credentials: this.loginForm.getRawValue() as LoginRequest,
      onError: (errors) => {
        this.validationErrors.set(errors);
      },
    });
  }
}
