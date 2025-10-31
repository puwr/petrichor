import { Component, inject } from '@angular/core';
import { GalleryComponent } from '../../shared/components/gallery/gallery.component';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { Router } from '@angular/router';
import { TextInputComponent } from '../../shared/components/text-input/text-input.component';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { GalleryPageStore } from './store/gallery-page.store';

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
