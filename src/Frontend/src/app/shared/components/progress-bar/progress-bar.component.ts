import { Component, inject } from '@angular/core';
import { LoadingService } from '../../../core/services/loading.service';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-progress-bar',
  imports: [AsyncPipe],
  templateUrl: './progress-bar.component.html',
  styleUrl: './progress-bar.component.scss',
})
export class ProgressBarComponent {
  private loadingService = inject(LoadingService);
  loading$ = this.loadingService.loading$;
}
