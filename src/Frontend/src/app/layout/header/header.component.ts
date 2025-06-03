import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LoadingService } from '../../core/services/loading.service';
import { ProgressBarComponent } from '../../shared/components/progress-bar/progress-bar.component';

@Component({
  selector: 'app-header',
  imports: [ProgressBarComponent, RouterLink],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  private loadingService = inject(LoadingService);
  loading$ = this.loadingService.loading$;
}
