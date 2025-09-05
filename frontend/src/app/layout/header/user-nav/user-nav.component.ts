import { Component, inject } from '@angular/core';
import { CdkMenu, CdkMenuItem, CdkMenuTrigger } from '@angular/cdk/menu';
import { ConnectedPosition } from '@angular/cdk/overlay';
import { RouterLink } from '@angular/router';
import { AuthFacade } from '../../../core/store/auth/auth.facade';

@Component({
  selector: 'app-user-nav',
  imports: [CdkMenu, CdkMenuItem, CdkMenuTrigger, RouterLink],
  templateUrl: './user-nav.component.html',
  styleUrl: './user-nav.component.scss',
})
export class UserNavComponent {
  private authFacade = inject(AuthFacade);

  currentUser = this.authFacade.currentUser;

  menuPosition: ConnectedPosition[] = [
    {
      originX: 'center',
      originY: 'bottom',
      overlayX: 'center',
      overlayY: 'top',
    },
  ];

  logout(): void {
    this.authFacade.logoutEffect();
  }
}
