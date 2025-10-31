import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthStore } from '@app/core/auth';
import { ButtonComponent } from '@app/shared/components';
import { UserNavComponent } from './user-nav/user-nav.component';

@Component({
  selector: 'app-header',
  imports: [RouterLink, UserNavComponent, ButtonComponent],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  readonly authStore = inject(AuthStore);
}
