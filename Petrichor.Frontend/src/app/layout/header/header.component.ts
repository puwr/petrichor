import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { UserNavComponent } from './user-nav/user-nav.component';
import { AuthStore } from '../../core/store/auth/auth.store';
import { ButtonComponent } from '../../shared/components/button/button.component';

@Component({
  selector: 'app-header',
  imports: [RouterLink, UserNavComponent, ButtonComponent],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  readonly authStore = inject(AuthStore);
}
