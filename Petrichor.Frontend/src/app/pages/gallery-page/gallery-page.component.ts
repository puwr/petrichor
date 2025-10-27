import { Component, inject } from '@angular/core';
import { ImageService } from '../../core/services/image.service';
import { GalleryItem } from '../../shared/models/image';
import { GalleryComponent } from '../../shared/components/gallery/gallery.component';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { PagedResponse } from '../../shared/models/pagination';
import { ActivatedRoute, Router } from '@angular/router';
import { TextInputComponent } from '../../shared/components/text-input/text-input.component';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { distinctUntilChanged, Observable, switchMap, tap } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { ButtonComponent } from '../../shared/components/button/button.component';

@Component({
  selector: 'app-gallery-page',
  imports: [
    GalleryComponent,
    PaginationComponent,
    TextInputComponent,
    ReactiveFormsModule,
    AsyncPipe,
    ButtonComponent,
  ],
  templateUrl: './gallery-page.component.html',
  styleUrl: './gallery-page.component.scss',
})
export class GalleryPageComponent {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);
  private imageService = inject(ImageService);

  searchForm = this.fb.group({
    tags: [''],
  });

  searchTags: string[] = [];

  galleryData$ = this.route.queryParamMap.pipe(
    distinctUntilChanged((prev, curr) => {
      return (
        prev.get('page') === curr.get('page') &&
        this.tagsEqual(prev.getAll('tags'), curr.getAll('tags'))
      );
    }),
    switchMap((params) => {
      const pageNumber = Number(params.get('page')) || 1;
      this.searchTags = params.getAll('tags');

      return this.loadGalleryData(pageNumber);
    }),
  );

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

  private loadGalleryData(pageNumber: number): Observable<PagedResponse<GalleryItem>> {
    return this.imageService.getImages(pageNumber, this.searchTags).pipe(
      tap((response) => {
        if (response.totalPages > 0 && pageNumber > response.totalPages) {
          this.router.navigate([], {
            queryParams: { page: response.totalPages },
            queryParamsHandling: 'merge',
          });
        }
      }),
    );
  }

  private tagsEqual(a: string[], b: string[]): boolean {
    if (!a || !b) {
      return a === b;
    }

    if (a.length !== b.length) {
      return false;
    }

    return a.every((tag, index) => tag.toLowerCase() === b[index].toLowerCase());
  }
}
