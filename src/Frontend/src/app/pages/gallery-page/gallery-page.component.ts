import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { ImageService } from '../../core/services/image.service';
import { GalleryItem } from '../../shared/models/galleryItem';
import { GalleryComponent } from '../../shared/components/gallery/gallery.component';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-gallery-page',
  imports: [GalleryComponent],
  templateUrl: './gallery-page.component.html',
  styleUrl: './gallery-page.component.scss',
})
export class GalleryPageComponent implements OnInit {
  private imageService = inject(ImageService);
  private destroyRef = inject(DestroyRef);

  galleryItems: GalleryItem[] = [];

  ngOnInit(): void {
    this.getImages();
  }

  getImages(): void {
    this.imageService
      .getImages()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((response) => (this.galleryItems = response));
  }
}
