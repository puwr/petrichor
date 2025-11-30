import { AfterViewInit, Directive, ElementRef, inject, OnDestroy } from '@angular/core';
import { FocusMonitor } from '@angular/cdk/a11y';

@Directive({
  selector: '[appAutoFocus]',
})
export class AutoFocusDirective implements AfterViewInit, OnDestroy {
  private elementRef = inject(ElementRef);
  private focusMonitor = inject(FocusMonitor);

  ngAfterViewInit(): void {
    this.focusMonitor.monitor(this.elementRef.nativeElement);
    this.focusMonitor.focusVia(this.elementRef.nativeElement, 'program');
  }

  ngOnDestroy(): void {
    this.focusMonitor.stopMonitoring(this.elementRef.nativeElement);
  }
}
