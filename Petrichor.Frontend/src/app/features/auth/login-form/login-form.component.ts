import { Component, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { email, form, FormField, required, submit } from '@angular/forms/signals';
import { AuthStore } from '@app/core/auth';
import {
  ValidationErrorsComponent,
  ButtonComponent,
  FieldErrorsDirective,
} from '@app/shared/components';
import { catchError, firstValueFrom, map, of } from 'rxjs';
import { AutoFocusDirective } from '@app/shared/directives/auto-focus.directive';

@Component({
  selector: 'app-login-form',
  imports: [
    ValidationErrorsComponent,
    ButtonComponent,
    FormField,
    FieldErrorsDirective,
    AutoFocusDirective,
  ],
  templateUrl: './login-form.component.html',
  styleUrl: './login-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginFormComponent {
  private authStore = inject(AuthStore);

  loginForm = form(signal(this.initLoginForm()), (schema) => {
    required(schema.email);
    email(schema.email, { message: 'Invalid email.' });

    required(schema.password);
  });

  async onSubmit(event: SubmitEvent): Promise<void> {
    event.preventDefault();

    await submit(this.loginForm, async (form) => {
      if (form().invalid()) return;

      return await firstValueFrom(
        this.authStore
          .login({ email: form().value().email, password: form().value().password })
          .pipe(
            map(() => null),
            catchError((errors) => of(errors)),
          ),
      );
    });
  }

  private initLoginForm() {
    return {
      email: '',
      password: '',
    };
  }
}
