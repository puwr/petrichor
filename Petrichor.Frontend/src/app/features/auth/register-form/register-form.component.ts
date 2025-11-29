import { Component, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { email, Field, form, maxLength, pattern, required, submit } from '@angular/forms/signals';
import { AuthStore } from '@app/core/auth';
import {
  ValidationErrorsComponent,
  ButtonComponent,
  FieldErrorsDirective,
} from '@app/shared/components';
import { catchError, firstValueFrom, of } from 'rxjs';

@Component({
  selector: 'app-register-form',
  imports: [
    ReactiveFormsModule,
    ValidationErrorsComponent,
    ButtonComponent,
    Field,
    FieldErrorsDirective,
  ],
  templateUrl: './register-form.component.html',
  styleUrl: './register-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegisterFormComponent {
  private authStore = inject(AuthStore);

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

  registerForm = form(signal(this.initRegisterForm()), (schema) => {
    required(schema.email);
    email(schema.email);
    maxLength(schema.email, 100);

    required(schema.userName);
    pattern(schema.userName, new RegExp(this.userNamePattern), {
      message:
        'Username must be 3-30 characters long and only contain letters, digits or underscores.',
    });

    required(schema.password);
    pattern(schema.password, new RegExp(this.passwordPattern), {
      message:
        'Password must be 8-128 characters long, have lowercase letter, uppercase letter, digit and special character.',
    });
  });

  async onSubmit(event: SubmitEvent): Promise<void> {
    event.preventDefault();

    await submit(this.registerForm, async (form) => {
      if (form().invalid()) return;

      const formValue = form().value();

      return await firstValueFrom(
        this.authStore
          .register({
            email: formValue.email,
            userName: formValue.userName,
            password: formValue.password,
          })
          .pipe(catchError((errors) => of(errors))),
      );
    });
  }

  private initRegisterForm() {
    return {
      email: '',
      userName: '',
      password: '',
    };
  }
}
