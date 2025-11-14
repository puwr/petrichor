import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './layout';
import { ProgressBarComponent } from './shared/components';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent, ProgressBarComponent],
  template: `
    <app-progress-bar position="fixed" />

    <app-header />

    <main>
      <router-outlet />
    </main>
  `,
})
export class App {}
