import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  OnDestroy,
} from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TextInputComponent } from '../../../shared/components/text-input/text-input.component';
import { LoginRequest } from '../../../shared/models/auth';
import { ValidationErrorsComponent } from '../../../shared/components/validation-errors/validation-errors.component';
import { AuthFacade } from '../../../core/store/auth/auth.facade';
import { clearRequestsResult, clearRequestsStatus } from '@ngneat/elf-requests';

@Component({
  selector: 'app-login-form',
  imports: [ReactiveFormsModule, TextInputComponent, ValidationErrorsComponent],
  templateUrl: './login-form.component.html',
  styleUrl: './login-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginFormComponent implements OnDestroy {
  private fb = inject(FormBuilder);
  private authFacade = inject(AuthFacade);

  loginStatus = this.authFacade.loginStatus;

  validationErrors = computed(() => {
    const status = this.loginStatus();
    if (status?.value === 'error') {
      return status.error as string[];
    }

    return null;
  });

  loginForm = this.fb.group({
    email: ['', []],
    password: [''],
  });

  onSubmit(): void {
    const credentials = this.loginForm.getRawValue() as LoginRequest;

    this.authFacade.loginEffect(credentials);
  }

  ngOnDestroy(): void {
    this.authFacade.clearRequestStatus();
  }
}
