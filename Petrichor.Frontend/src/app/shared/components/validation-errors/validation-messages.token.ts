import { InjectionToken } from '@angular/core';
import { MaxLengthValidationError, RequiredValidationError } from '@angular/forms/signals';

export type ValidationMessageFn<T = any> = (error: T) => string;

export const VALIDATION_MESSAGES = new InjectionToken<Record<string, ValidationMessageFn>>(
  'VALIDATION_MESSAGES',
);

export const defaultValidationMessages: Record<string, ValidationMessageFn> = {
  invalid: () => 'Field is invalid.',
  required: (_error: RequiredValidationError) => 'Field is required.',
  email: () => 'Please enter a valid email.',
  maxLength: (error: MaxLengthValidationError) =>
    `Maximum length is ${error.maxLength} characters.`,
};
