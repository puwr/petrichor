import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { UserNavComponent } from './user-nav/user-nav.component';
import { AuthFacade } from '../../core/stores/auth/auth.facade';

@Component({
  selector: 'app-header',
  imports: [RouterLink, UserNavComponent],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  private authFacade = inject(AuthFacade);

  currentUser = this.authFacade.currentUser;
}
