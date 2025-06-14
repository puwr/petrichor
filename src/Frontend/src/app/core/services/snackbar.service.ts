import { Overlay, OverlayRef } from '@angular/cdk/overlay';
import { ComponentPortal } from '@angular/cdk/portal';
import { inject, Injectable } from '@angular/core';
import { SnackbarComponent } from '../../shared/components/snackbar/snackbar.component';

@Injectable({
  providedIn: 'root',
})
export class SnackbarService {
  private overlay = inject(Overlay);

  private overlayRef: OverlayRef | null = null;

  private show(
    message: string,
    type: 'success' | 'error',
    duration: number = 3000
  ): void {
    this.overlayRef?.dispose();

    this.overlayRef = this.overlay.create({
      positionStrategy: this.overlay
        .position()
        .global()
        .centerHorizontally()
        .bottom('2.5rem'),
      hasBackdrop: false,
    });

    const portal = new ComponentPortal(SnackbarComponent);
    const componentRef = this.overlayRef.attach(portal);

    componentRef.setInput('message', message);
    componentRef.setInput('type', type);

    setTimeout(() => {
      this.overlayRef?.dispose();
    }, duration);
  }

  success(message: string, duration?: number) {
    this.show(message, 'success', duration);
  }

  error(message: string, duration?: number) {
    this.show(message, 'error', duration);
  }
}
