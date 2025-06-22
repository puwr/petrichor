import { Component, input } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { GalleryItem } from '../../models/image';
import { RouterLink } from '@angular/router';

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
