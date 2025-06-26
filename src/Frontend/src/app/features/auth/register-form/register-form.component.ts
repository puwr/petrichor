import { Component, DestroyRef, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TextInputComponent } from '../../../shared/components/text-input/text-input.component';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';
import { RegisterRequest } from '../../../shared/models/auth';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { ValidationErrorsComponent } from '../../../shared/components/validation-errors/validation-errors.component';

@Component({
  selector: 'app-register-form',
  imports: [ReactiveFormsModule, TextInputComponent, ValidationErrorsComponent],
  templateUrl: './register-form.component.html',
  styleUrl: './register-form.component.scss',
})
export class RegisterFormComponent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private authService = inject(AuthService);
  private snackbar = inject(SnackbarService);
  private destroyRef = inject(DestroyRef);

  validationErrors?: string[];

  private usernamePattern =
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
    email: [
      '',
      [Validators.required, Validators.email, Validators.maxLength(100)],
    ],
    userName: [
      '',
      [Validators.required, Validators.pattern(this.usernamePattern)],
    ],
    password: [
      '',
      [Validators.required, Validators.pattern(this.passwordPattern)],
    ],
  });

  onSubmit(): void {
    const userData = this.registerForm.getRawValue() as RegisterRequest;

    this.authService
      .register(userData)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.snackbar.success('Registration complete! You may now log in.');
          this.router.navigateByUrl('login');
        },
        error: (errors) => (this.validationErrors = errors),
      });
  }
}
