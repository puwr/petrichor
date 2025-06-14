import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AccountService } from '../../core/services/account.service';
import { UserNavComponent } from './user-nav/user-nav.component';

@Component({
  selector: 'app-header',
  imports: [RouterLink, UserNavComponent],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  private accountService = inject(AccountService);

  currentUser = this.accountService.currentUser;
}
