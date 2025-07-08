import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TextInputComponent } from './text-input.component';
import {
  ControlContainer,
  FormControl,
  FormGroup,
  FormGroupDirective,
  Validators,
} from '@angular/forms';
import { Mock } from 'vitest';
import { FocusMonitor } from '@angular/cdk/a11y';
import { By } from '@angular/platform-browser';

describe('TextInputComponent', () => {
  let component: TextInputComponent;
  let fixture: ComponentFixture<TextInputComponent>;
  let inputElement: HTMLInputElement;

  let formGroup: FormGroup;
  let formGroupDirective: FormGroupDirective;
  let focusMonitor: { focusVia: Mock; stopMonitoring: Mock };

  beforeEach(() => {
    formGroup = new FormGroup({
      testControl: new FormControl('', Validators.required),
    });

    formGroupDirective = new FormGroupDirective([], []);
    formGroupDirective.form = formGroup;

    focusMonitor = { focusVia: vi.fn(), stopMonitoring: vi.fn() };

    TestBed.configureTestingModule({
      imports: [TextInputComponent],
      providers: [
        { provide: ControlContainer, useValue: formGroupDirective },
        { provide: FocusMonitor, useValue: focusMonitor },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(TextInputComponent);
    component = fixture.componentInstance;

    fixture.componentRef.setInput('label', 'Test Label');
    fixture.componentRef.setInput('controlName', 'testControl');

    fixture.detectChanges();

    inputElement = fixture.debugElement.query(By.css('input')).nativeElement;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display correct label', () => {
    const label = fixture.debugElement.query(By.css('.text-input__label'));

    expect(label.nativeElement.textContent).toBe('Test Label');
  });

  it('should set input id to match control name', () => {
    expect(inputElement.id).toBe('testControl');
  });

  it('should not display error when control is untouched', () => {
    component.control.setValue('');
    fixture.detectChanges();

    const error = fixture.debugElement.query(By.css('.text-input__error'));

    expect(component.showError()).toBe(false);
    expect(error).toBeNull();
  });

  it('should display error when control is invalid after interaction', () => {
    component.control.markAsTouched();
    component.control.markAsDirty();
    component.control.setValue('');
    fixture.detectChanges();

    const error = fixture.debugElement.query(By.css('.text-input__error'));

    expect(component.showError()).toBe(true);
    expect(error.nativeElement.textContent).toContain('required');
  });

  describe('autoFocus', () => {
    it('should be disabled by default', () => {
      expect(component.autoFocus()).toBe(false);
    });

    it('should not focus input when disabled', () => {
      component.ngAfterViewInit();

      expect(focusMonitor.focusVia).not.toHaveBeenCalled();
    });

    it('should focus input when enabled', () => {
      fixture.componentRef.setInput('autoFocus', true);
      fixture.detectChanges();

      component.ngAfterViewInit();

      expect(focusMonitor.focusVia).toHaveBeenCalledTimes(1);
      expect(focusMonitor.focusVia).toHaveBeenCalledWith(
        inputElement,
        'program'
      );
    });

    it('should clean up focus monitoring on component destruction', () => {
      component.ngOnDestroy();

      expect(focusMonitor.stopMonitoring).toHaveBeenCalledTimes(1);
      expect(focusMonitor.stopMonitoring).toHaveBeenCalledWith(inputElement);
    });
  });
});
