import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './layout/header/header.component';
import { LoadingService } from './core/services/loading.service';
import { AsyncPipe } from '@angular/common';
import { ProgressBarComponent } from './shared/components/progress-bar/progress-bar.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent, AsyncPipe, ProgressBarComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  private loadingService = inject(LoadingService);
  loading$ = this.loadingService.loading$;
}
