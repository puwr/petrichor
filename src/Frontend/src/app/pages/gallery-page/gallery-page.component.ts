import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { ImageService } from '../../core/services/image.service';
import { GalleryItem } from '../../shared/models/image';
import { GalleryComponent } from '../../shared/components/gallery/gallery.component';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { PagedResponse } from '../../shared/models/pagination';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-gallery-page',
  imports: [GalleryComponent, PaginationComponent],
  templateUrl: './gallery-page.component.html',
  styleUrl: './gallery-page.component.scss',
})
export class GalleryPageComponent implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private imageService = inject(ImageService);
  private destroyRef = inject(DestroyRef);

  galleryData: PagedResponse<GalleryItem> | null = null;

  ngOnInit(): void {
    this.route.queryParams
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((params) => {
        const pageNumber = Number(params['page']) || 1;

        this.loadGalleryData(pageNumber);
      });
  }

  private loadGalleryData(pageNumber: number): void {
    this.imageService
      .getImages(pageNumber)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((response) => {
        this.galleryData = response;

        if (pageNumber > response.totalPages) {
          this.router.navigate([], {
            queryParams: { page: response.totalPages },
            queryParamsHandling: 'merge',
          });
        }
      });
  }
}
