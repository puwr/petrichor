import { AfterViewInit, Directive, inject, ViewContainerRef } from '@angular/core';
import { Field } from '@angular/forms/signals';
import { ValidationErrorsComponent } from './validation-errors.component';

@Directive({
  selector: '[field]',
})
export class FieldErrorsDirective implements AfterViewInit {
  private readonly viewContainerRef = inject(ViewContainerRef);
  private readonly field = inject(Field);

  ngAfterViewInit(): void {
    const errorsContainer = this.viewContainerRef.createComponent(ValidationErrorsComponent);
    errorsContainer.setInput('field', this.field);
    errorsContainer.setInput('describedby', `error-${this.field.state().name()}`);
  }
}
