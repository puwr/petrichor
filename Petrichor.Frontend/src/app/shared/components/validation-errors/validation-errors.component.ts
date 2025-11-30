import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { Field, ValidationError } from '@angular/forms/signals';
import { ValidationMessagePipe } from '@app/shared/components/validation-errors/validation-message.pipe';

@Component({
  selector: 'app-validation-errors',
  imports: [ValidationMessagePipe],
  template: `
    @if(showErrors()) {
    <ul class="list-reset validation-errors" [id]="describedby()" role="alert">
      @for (error of errors(); track $index) {
      <li class="validation-errors__item">{{ error | validationMessage }}</li>
      }
    </ul>
    }
  `,
  styles: `
    .validation-errors {
      display: flex;
      flex-direction: column;
      margin-bottom: 0.25rem;
      gap: 0.25rem;
      color: var(--color-danger);

      animation: fadeInDown var(--duration-standard) var(--animation-standard) both;
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ValidationErrorsComponent {
  validationErrors = input<ValidationError.WithOptionalField[] | null>(null);
  field = input<Field<unknown> | null>(null);
  describedby = input<string | null>(null);

  errors = computed(() => {
    const validationErrors = this.validationErrors();
    const field = this.field();

    if (validationErrors) {
      return validationErrors;
    }

    if (field) {
      return field.state().errors();
    }

    return [];
  });

  showErrors = computed(() => {
    const field = this.field();

    if (field) {
      return field.state().invalid() && field.state().touched() && field.state().dirty();
    }

    return this.errors().length > 0;
  });
}
