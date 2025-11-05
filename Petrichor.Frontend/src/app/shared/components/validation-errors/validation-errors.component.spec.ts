import { ValidationErrorsComponent } from './validation-errors.component';
import { render, screen } from '@testing-library/angular';
import { inputBinding } from '@angular/core';

describe('ValidationErrorsComponent', () => {
  it('displays validation errors if errors are present', async () => {
    await render(ValidationErrorsComponent, {
      bindings: [
        inputBinding('validationErrors', () => ['validation error 1', 'validation error 2']),
      ],
    });

    expect(screen.getByRole('alert')).toBeInTheDocument();
    expect(screen.getByText(/validation error 1/i)).toBeInTheDocument();
    expect(screen.getByText(/validation error 2/i)).toBeInTheDocument();
  });

  it('does not display validation errors if errors are absent', async () => {
    await render(ValidationErrorsComponent, {
      bindings: [inputBinding('validationErrors', () => [])],
    });

    expect(screen.queryByRole('alert')).not.toBeInTheDocument();
  });
});
