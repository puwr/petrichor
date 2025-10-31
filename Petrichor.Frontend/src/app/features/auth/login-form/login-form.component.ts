import { Component, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { AuthStore, LoginRequest } from '@app/core/auth';
import {
  TextInputComponent,
  ValidationErrorsComponent,
  ButtonComponent,
} from '@app/shared/components';

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
