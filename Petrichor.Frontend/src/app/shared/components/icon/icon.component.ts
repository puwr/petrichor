import { Component, inject, input, ViewEncapsulation } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-icon',
  template: '<span class="icon" [innerHTML]="svg"></span>',
  styles:
    '.icon { display: flex; align-items: center; } .icon svg { width: 0.9rem; height: 0.9rem; }',
  encapsulation: ViewEncapsulation.None,
})
export class IconComponent {
  private readonly sanitizer = inject(DomSanitizer);
  name = input<string>('');

  get svg() {
    return this.sanitizer.bypassSecurityTrustHtml(ICONS[this.name()] || '');
  }
}

const ICONS: Record<string, string> = {
  delete: `
    <svg
      xmlns="http://www.w3.org/2000/svg"
      fill="none"
      viewBox="0 0 24 24"
      stroke-width="1.5"
      stroke="currentColor"
    >
      <path
        stroke-linecap="round"
        stroke-linejoin="round"
        d="m14.74 9-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 0 1-2.244 2.077H8.084a2.25 2.25 0 0 1-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 0 0-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 0 1 3.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 0 0-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 0 0-7.5 0"
      />
    </svg>
  `,
  plus: `
    <svg
      viewBox="0 0 24 24"
      fill="none"
      stroke-width="1.5"
      stroke="currentColor"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path d="M11 11v-11h1v11h11v1h-11v11h-1v-11h-11v-1h11z" />
    </svg>
  `,
};
