import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
} from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TextInputComponent } from '../../../shared/components/text-input/text-input.component';
import { RegisterRequest } from '../../../shared/models/auth';
import { ValidationErrorsComponent } from '../../../shared/components/validation-errors/validation-errors.component';
import { AuthFacade } from '../../../core/stores/auth/auth.facade';

@Component({
  selector: 'app-register-form',
  imports: [ReactiveFormsModule, TextInputComponent, ValidationErrorsComponent],
  templateUrl: './register-form.component.html',
  styleUrl: './register-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegisterFormComponent {
  private fb = inject(FormBuilder);
  private authFacade = inject(AuthFacade);

  registerStatus = this.authFacade.registerStatus;

  validationErrors = computed(() => {
    const status = this.registerStatus();
    if (status?.value === 'error') {
      return status.error as string[];
    }

    return null;
  });

  onSubmit(): void {
    const userData = this.registerForm.getRawValue() as RegisterRequest;
    this.authFacade.registerEffect(userData);
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
    email: [
      '',
      [Validators.required, Validators.email, Validators.maxLength(100)],
    ],
    userName: [
      '',
      [Validators.required, Validators.pattern(this.userNamePattern)],
    ],
    password: [
      '',
      [Validators.required, Validators.pattern(this.passwordPattern)],
    ],
  });
}
