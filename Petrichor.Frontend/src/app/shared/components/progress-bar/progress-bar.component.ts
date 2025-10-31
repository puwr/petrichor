import { Component, inject, input, computed } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { LoadingService } from '@app/core/loading/loading.service';

@Component({
  selector: 'app-progress-bar',
  imports: [],
  templateUrl: './progress-bar.component.html',
  styleUrl: './progress-bar.component.scss',
})
export class ProgressBarComponent {
  private loadingService = inject(LoadingService);

  mode = input<'determinate' | 'indeterminate'>('indeterminate');
  progressPercentage = input<number>(0);

  loading = toSignal(this.loadingService.loading$, { initialValue: false });

  isActive = computed(() => this.loading() || this.isDeterminate());
  isDeterminate = computed(() => this.mode() === 'determinate');
  isIndeterminate = computed(() => this.mode() === 'indeterminate');

  progressTransform = computed(
    () => `translateX(${Math.min(Math.max(this.progressPercentage(), 0), 100) - 100}%)`,
  );
}
