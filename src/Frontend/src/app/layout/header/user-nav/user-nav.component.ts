import { Component, DestroyRef, inject } from '@angular/core';
import { AccountService } from '../../../core/services/account.service';
import { CdkMenu, CdkMenuItem, CdkMenuTrigger } from '@angular/cdk/menu';
import { AuthService } from '../../../core/services/auth.service';
import { ConnectedPosition } from '@angular/cdk/overlay';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-user-nav',
  imports: [CdkMenu, CdkMenuItem, CdkMenuTrigger, RouterLink],
  templateUrl: './user-nav.component.html',
  styleUrl: './user-nav.component.scss',
})
export class UserNavComponent {
  private accountService = inject(AccountService);
  private authServicce = inject(AuthService);
  private destroyRef = inject(DestroyRef);

  currentUser = this.accountService.currentUser();

  menuPosition: ConnectedPosition[] = [
    {
      originX: 'center',
      originY: 'bottom',
      overlayX: 'center',
      overlayY: 'top',
    },
  ];

  logout(): void {
    this.authServicce
      .logout()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe();
  }
}
