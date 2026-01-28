import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { GalleryComponent, PaginationComponent, ButtonComponent } from '@app/shared/components';
import { GalleryPageStore } from './gallery-page.store';
import { form, FormField } from '@angular/forms/signals';

@Component({
  selector: 'app-gallery-page',
  imports: [GalleryComponent, PaginationComponent, ReactiveFormsModule, ButtonComponent, FormField],
  providers: [GalleryPageStore],
  templateUrl: './gallery-page.component.html',
  styleUrl: './gallery-page.component.scss',
})
export class GalleryPageComponent {
  private router = inject(Router);
  readonly galleryPageStore = inject(GalleryPageStore);

  searchForm = form(signal({ tags: '' }));

  onSearch(event: SubmitEvent): void {
    event.preventDefault();

    const tags =
      this.searchForm()
        .value()
        .tags.split(',')
        .map((tag) => tag.trim().toLowerCase())
        .filter((tag) => tag) || [];

    if (tags.length) {
      this.router.navigate([], {
        queryParams: { page: null, tags },
        queryParamsHandling: 'merge',
      });

      this.searchForm().reset();
      this.searchForm().value.set({ tags: '' });
    }
  }
}
