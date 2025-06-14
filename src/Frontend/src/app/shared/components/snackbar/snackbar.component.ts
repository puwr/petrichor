import { Component, input } from '@angular/core';

@Component({
  selector: 'app-snackbar',
  imports: [],
  templateUrl: './snackbar.component.html',
  styleUrl: './snackbar.component.scss',
})
export class SnackbarComponent {
  message = input.required<string>();
  type = input.required<'success' | 'error'>();
}
