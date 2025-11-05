import { TestBed } from '@angular/core/testing';
import { TextInputComponent } from './text-input.component';
import {
  ControlContainer,
  FormControl,
  FormGroup,
  FormGroupDirective,
  Validators,
} from '@angular/forms';
import { FocusMonitor } from '@angular/cdk/a11y';
import { render, screen, waitFor } from '@testing-library/angular';
import userEvent from '@testing-library/user-event';
import { inputBinding } from '@angular/core';

describe('TextInputComponent', () => {
  it('displays correct label', async () => {
    await setup();

    expect(screen.getByText(/test label/i)).toBeInTheDocument();
  });

  it('focuses input when label is clicked', async () => {
    const { user } = await setup();

    await user.click(screen.getByText(/test label/i));

    expect(screen.getByRole('textbox', { name: /test label/i })).toHaveFocus();
  });

  it('does not display error when control is untouched', async () => {
    await setup();

    expect(screen.queryByRole('alert')).not.toBeInTheDocument();
  });

  it('displays error when control is invalid after interaction', async () => {
    const { user } = await setup();

    const inputEl = screen.getByRole('textbox', { name: /test label/i });

    await user.type(inputEl, 'test');
    await user.clear(inputEl);
    await user.tab();

    expect(screen.getByRole('alert')).toBeInTheDocument();
    expect(screen.getByText(/required/i)).toBeInTheDocument();
  });

  describe('autoFocus', () => {
    it('does not focus input by default', async () => {
      await setup();

      expect(screen.getByRole('textbox', { name: /test label/i })).not.toHaveFocus();
    });

    it('focuses input when enabled', async () => {
      await setup({ autoFocus: true });

      await waitFor(() =>
        expect(screen.getByRole('textbox', { name: /test label/i })).toHaveFocus(),
      );
    });

    it('cleans up focus monitoring on component destruction', async () => {
      const { fixture } = await setup();

      const inputEl = screen.getByRole('textbox', { name: /test label/i });
      const focusMonitor = TestBed.inject(FocusMonitor);
      vi.spyOn(focusMonitor, 'stopMonitoring');

      fixture.destroy();

      expect(focusMonitor.stopMonitoring).toHaveBeenCalledWith(inputEl);
    });
  });
});

async function setup(config: { autoFocus?: boolean } = {}) {
  const user = userEvent.setup();
  const formGroupDirective = new FormGroupDirective([], []);
  formGroupDirective.form = new FormGroup({
    testControl: new FormControl('', Validators.required),
  });

  const { fixture } = await render(TextInputComponent, {
    bindings: [
      inputBinding('controlName', () => 'testControl'),
      inputBinding('label', () => 'test label'),
      inputBinding('autoFocus', () => config.autoFocus ?? false),
    ],
    providers: [
      {
        provide: ControlContainer,
        useValue: formGroupDirective,
      },
    ],
  });

  return { user, fixture };
}
