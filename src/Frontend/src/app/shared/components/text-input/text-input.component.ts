import { FocusMonitor } from '@angular/cdk/a11y';
import {
  AfterViewInit,
  Component,
  ElementRef,
  inject,
  input,
  OnDestroy,
  ViewChild,
} from '@angular/core';
import {
  ControlContainer,
  FormControl,
  ReactiveFormsModule,
} from '@angular/forms';

@Component({
  selector: 'app-text-input',
  imports: [ReactiveFormsModule],
  templateUrl: './text-input.component.html',
  styleUrl: './text-input.component.scss',
  viewProviders: [
    {
      provide: ControlContainer,
      useFactory: () => inject(ControlContainer, { skipSelf: true }),
    },
  ],
})
export class TextInputComponent implements AfterViewInit, OnDestroy {
  type = input<'text' | 'email' | 'password'>('text');
  label = input.required();
  controlName = input.required<string>();
  autoFocus = input<boolean>(false);

  private parentContainer = inject(ControlContainer);
  private focusMonitor = inject(FocusMonitor);

  @ViewChild('input') inputRef!: ElementRef;

  ngAfterViewInit(): void {
    if (this.autoFocus() && this.inputRef) {
      this.focusMonitor.focusVia(this.inputRef.nativeElement, 'program');
    }
  }

  ngOnDestroy(): void {
    this.focusMonitor.stopMonitoring(this.inputRef.nativeElement);
  }

  get control(): FormControl {
    return this.parentContainer.control?.get(this.controlName()) as FormControl;
  }

  get errorMessage(): string {
    const errors = this.control.errors;

    if (!errors) return '';

    if (errors['required']) {
      return `${this.label()} is required.`;
    }

    if (errors['email']) {
      return 'Please enter a valid email.';
    }

    if (errors['minlength']) {
      return `${this.label()} must be at least ${
        this.control.getError('minlength').requiredLength
      } characters long.`;
    }

    if (errors['maxlength']) {
      return `${this.label()} cannot exceed ${
        this.control.getError('maxlength').requiredLength
      } characters.`;
    }

    if (this.controlName() === 'userName' && errors['pattern']) {
      return `Username must be 3-30 characters long
        and only contain letters, digits or underscores.`;
    }

    if (this.controlName() === 'password' && errors['pattern']) {
      return `Password must be 8-128 characters long, have lowercase letter,
        uppercase letter, digit and special character.`;
    }

    return '';
  }

  showError(): boolean {
    return this.control.touched && this.control.dirty && this.control.invalid;
  }
}
