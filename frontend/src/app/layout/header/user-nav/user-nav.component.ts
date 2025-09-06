import { Component, inject } from '@angular/core';
import { CdkMenu, CdkMenuItem, CdkMenuTrigger } from '@angular/cdk/menu';
import { ConnectedPosition } from '@angular/cdk/overlay';
import { RouterLink } from '@angular/router';
import { AuthStore } from '../../../core/store/auth/auth.store';

@Component({
	selector: 'app-user-nav',
	imports: [CdkMenu, CdkMenuItem, CdkMenuTrigger, RouterLink],
	templateUrl: './user-nav.component.html',
	styleUrl: './user-nav.component.scss',
})
export class UserNavComponent {
	readonly authStore = inject(AuthStore);

	menuPosition: ConnectedPosition[] = [
		{
			originX: 'center',
			originY: 'bottom',
			overlayX: 'center',
			overlayY: 'top',
		},
	];
}
