import { Pipe, PipeTransform } from '@angular/core';
import { defaultValidationMessages, VALIDATION_MESSAGES } from './validation-messages.token';
import { inject } from '@angular/core';
import { ValidationError } from '@angular/forms/signals';

@Pipe({
  name: 'validationMessage',
})
export class ValidationMessagePipe implements PipeTransform {
  private readonly messages = {
    ...defaultValidationMessages,
    ...inject(VALIDATION_MESSAGES, { optional: true }),
  };

  public transform(error: ValidationError.WithOptionalField | null | undefined): string {
    if (!error) {
      return '';
    }
    const message = error.message ?? (this.messages[error.kind] ?? this.messages['invalid'])(error);
    return message;
  }
}
