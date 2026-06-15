import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-snackbar',
  imports: [],
  templateUrl: './snackbar.component.html',
  styleUrl: './snackbar.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SnackbarComponent {
  message = input.required<string>();
  type = input.required<'success' | 'error'>();
}
