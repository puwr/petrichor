import { Component, computed, input } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-pagination',
  imports: [RouterLink],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.scss',
})
export class PaginationComponent {
  maxVisiblePages = input<number>(4);
  pageNumber = input.required<number>();
  totalPages = input.required<number>();

  visiblePages = computed((): number[] => {
    const current = this.pageNumber();
    const total = this.totalPages();

    if (total <= this.maxVisiblePages() + 2) {
      return Array.from({ length: total - 2 }, (_, i) => i + 2);
    }

    const half = Math.floor(this.maxVisiblePages() / 2);
    let start = Math.max(2, current - half);
    let end = Math.min(total - 1, current + half);

    if (current <= half + 1) {
      end = start + this.maxVisiblePages() - 1;
    }

    if (current >= total - half) {
      start = end - this.maxVisiblePages() + 1;
    }

    return Array.from({ length: end - start + 1 }, (_, i) => start + i);
  });
}
