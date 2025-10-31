import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LoadingService } from './core/loading/loading.service';
import { HeaderComponent } from './layout';
import { ProgressBarComponent } from './shared/components';

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
