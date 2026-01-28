import { AfterViewInit, Directive, inject, ViewContainerRef } from '@angular/core';
import { FormField } from '@angular/forms/signals';
import { ValidationErrorsComponent } from './validation-errors.component';

@Directive({
  selector: '[formField]',
})
export class FieldErrorsDirective implements AfterViewInit {
  private readonly viewContainerRef = inject(ViewContainerRef);
  private readonly field = inject(FormField);

  ngAfterViewInit(): void {
    const errorsContainer = this.viewContainerRef.createComponent(ValidationErrorsComponent);
    errorsContainer.setInput('field', this.field);
    errorsContainer.setInput('describedby', `error-${this.field.state().name()}`);
  }
}
