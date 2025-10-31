import { Component, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { AuthStore, RegisterRequest } from '@app/core/auth';
import {
  TextInputComponent,
  ValidationErrorsComponent,
  ButtonComponent,
} from '@app/shared/components';

@Component({
  selector: 'app-register-form',
  imports: [ReactiveFormsModule, TextInputComponent, ValidationErrorsComponent, ButtonComponent],
  templateUrl: './register-form.component.html',
  styleUrl: './register-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegisterFormComponent {
  private fb = inject(FormBuilder);
  private authStore = inject(AuthStore);

  validationErrors = signal<string[] | null>(null);

  onSubmit(): void {
    this.authStore.register({
      userData: this.registerForm.getRawValue() as RegisterRequest,
      onError: (errors: string[] | null) => this.validationErrors.set(errors),
    });
  }

  private userNamePattern =
    '^' +
    '[a-zA-Z0-9_]{3,30}' + // letters, digits, underscores, 3-30 chars
    '$';

  private passwordPattern =
    '^' +
    '(?=.*[a-z])' + // lowercase letter
    '(?=.*[A-Z])' + // uppercase letter
    '(?=.*\\d)' + // digit
    '(?=.*\\W)' + // non-alphanumeric char
    '[A-Za-z\\d\\W]{8,128}' + // 8-128 chars
    '$';

  registerForm = this.fb.group({
    email: ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
    userName: ['', [Validators.required, Validators.pattern(this.userNamePattern)]],
    password: ['', [Validators.required, Validators.pattern(this.passwordPattern)]],
  });
}
