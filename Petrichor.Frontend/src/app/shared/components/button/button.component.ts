import { Component, input } from '@angular/core';

@Component({
  selector: 'app-button',
  imports: [],
  host: { '[attr.tabindex]': 'null' },
  templateUrl: './button.component.html',
  styleUrl: './button.component.scss',
})
export class ButtonComponent {
  variant = input<'text' | 'fill' | 'neutral' | 'danger'>('text');
  type = input<'button' | 'submit' | 'reset'>('button');
  disabled = input<boolean>(false);
  ariaLabel = input<string | null>(null);
  fullWidth = input<boolean>(false);
}
