import { Component, inject, OnInit } from '@angular/core';
import { ImageService } from '../../core/services/image.service';
import { GalleryItem } from '../../shared/models/galleryItem';
import { GalleryComponent } from '../../shared/components/gallery/gallery.component';

@Component({
  selector: 'app-gallery-page',
  imports: [GalleryComponent],
  templateUrl: './gallery-page.component.html',
  styleUrl: './gallery-page.component.scss',
})
export class GalleryPageComponent implements OnInit {
  private imageService = inject(ImageService);

  galleryItems: GalleryItem[] = [];

  ngOnInit(): void {
    this.getImages();
  }

  getImages() {
    this.imageService.getImages().subscribe({
      next: (response) => (this.galleryItems = response),
      error: (error) => console.log(error),
    });
  }
}
