import { Component, computed, input } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-pagination',
  imports: [RouterLink],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.scss',
})
export class PaginationComponent {
  maxVisiblePages = input<number>(5);
  pageNumber = input.required<number>();
  totalPages = input.required<number>();

  visiblePages = computed((): number[] => {
    const current = this.pageNumber();
    const total = this.totalPages();
    const maxVisible = this.maxVisiblePages();

    if (total <= maxVisible + 2) {
      return Array.from({ length: total - 2 }, (_, i) => i + 2);
    }

    const half = Math.floor(maxVisible / 2);
    let start = Math.max(2, current - half);
    let end = Math.min(total - 1, current + half);

    if (current <= half + 1) {
      end = start + maxVisible - 1;
    }

    if (current >= total - half) {
      start = end - maxVisible + 1;
    }

    return Array.from({ length: end - start + 1 }, (_, i) => start + i);
  });

  paginationItems = computed((): (number | '…')[] => {
    const items: (number | '…')[] = [1];
    const visible = this.visiblePages();
    const total = this.totalPages();

    if (visible.length > 0 && visible[0] > 2) {
      items.push('…');
    }
    items.push(...visible);

    if (visible.length > 0 && visible[visible.length - 1] < total - 1) {
      items.push('…');
    }

    if (total > 1) {
      items.push(total);
    }

    return items;
  });
}
