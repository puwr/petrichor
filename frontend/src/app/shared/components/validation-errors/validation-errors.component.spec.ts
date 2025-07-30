import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ValidationErrorsComponent } from './validation-errors.component';
import { By } from '@angular/platform-browser';

describe('ValidationErrorsComponent', () => {
  let component: ValidationErrorsComponent;
  let fixture: ComponentFixture<ValidationErrorsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ValidationErrorsComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ValidationErrorsComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display validation errors', () => {
    fixture.componentRef.setInput('validationErrors', ['Error 1', 'Error 2']);
    fixture.detectChanges();

    const errorElements = fixture.debugElement.queryAll(
      By.css('.validation-errors__item')
    );

    expect(errorElements.length).toBe(2);
  });
});
