import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import {
  GalleryComponent,
  PaginationComponent,
  TextInputComponent,
  ButtonComponent,
} from '@app/shared/components';
import { GalleryPageStore } from './gallery-page.store';

@Component({
  selector: 'app-gallery-page',
  imports: [
    GalleryComponent,
    PaginationComponent,
    TextInputComponent,
    ReactiveFormsModule,
    ButtonComponent,
  ],
  providers: [GalleryPageStore],
  templateUrl: './gallery-page.component.html',
  styleUrl: './gallery-page.component.scss',
})
export class GalleryPageComponent {
  private router = inject(Router);
  private fb = inject(FormBuilder);
  readonly galleryPageStore = inject(GalleryPageStore);

  searchForm = this.fb.group({
    tags: [''],
  });

  onSearch(): void {
    const tags =
      this.searchForm.value.tags
        ?.split(',')
        .map((tag) => tag.trim().toLowerCase())
        .filter((tag) => tag) || [];

    if (tags.length) {
      this.router.navigate([], {
        queryParams: { page: null, tags },
        queryParamsHandling: 'merge',
      });

      this.searchForm.reset();
    }
  }
}
