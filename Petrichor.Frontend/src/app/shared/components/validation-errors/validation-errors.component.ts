import { Component, input } from '@angular/core';

@Component({
  selector: 'app-validation-errors',
  imports: [],
  templateUrl: './validation-errors.component.html',
  styleUrl: './validation-errors.component.scss',
})
export class ValidationErrorsComponent {
  validationErrors = input.required<string[]>();
}
