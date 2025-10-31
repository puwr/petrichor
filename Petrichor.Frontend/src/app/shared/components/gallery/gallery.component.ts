import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { GalleryItem } from '@app/features/gallery/image.models';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-gallery',
  imports: [RouterLink],
  templateUrl: './gallery.component.html',
  styleUrl: './gallery.component.scss',
})
export class GalleryComponent {
  galleryItems = input.required<GalleryItem[]>();

  baseUrl = environment.baseUrl;

  calculateRowSpan(width: number, height: number): number {
    const aspectRatio = height / width;

    const rowSpan = Math.ceil(aspectRatio * 16);

    return rowSpan;
  }
}
