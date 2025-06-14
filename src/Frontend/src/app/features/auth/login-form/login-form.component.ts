import { Component, DestroyRef, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TextInputComponent } from '../../../shared/components/text-input/text-input.component';
import { AuthService } from '../../../core/services/auth.service';
import { LoginRequest } from '../../../shared/models/auth';
import { AccountService } from '../../../core/services/account.service';
import { Router } from '@angular/router';
import { exhaustMap } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-login-form',
  imports: [ReactiveFormsModule, TextInputComponent],
  templateUrl: './login-form.component.html',
  styleUrl: './login-form.component.scss',
})
export class LoginFormComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private accountService = inject(AccountService);
  private router = inject(Router);
  private destroyRef = inject(DestroyRef);

  validationErrors?: string[];

  loginForm = this.fb.group({
    email: ['', Validators.required],
    password: ['', Validators.required],
  });

  onSubmit(): void {
    const credentials = this.loginForm.getRawValue() as LoginRequest;

    this.authService
      .login(credentials)
      .pipe(
        exhaustMap(() => this.accountService.updateCurrentUser()),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        complete: () => {
          this.router.navigateByUrl('/');
        },
        error: (errors) => (this.validationErrors = errors),
      });
  }
}
