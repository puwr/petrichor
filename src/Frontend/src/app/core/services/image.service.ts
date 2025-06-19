import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { GalleryItem } from '../../shared/models/galleryItem';
import { Image } from '../../shared/models/image';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ImageService {
  private http = inject(HttpClient);

  private apiUrl = environment.apiUrl;

  getImages(): Observable<GalleryItem[]> {
    return this.http.get<GalleryItem[]>(`${this.apiUrl}/images`);
  }

  getImage(id: string): Observable<Image> {
    return this.http.get<Image>(`${this.apiUrl}/images/${id}`);
  }

  deleteImage(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/images/${id}`);
  }

  addImageTags(imageId: string, tags: string[]): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/images/${imageId}/tags`, {
      tags,
    });
  }

  deleteImageTag(imageId: string, tagId: string): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/images/${imageId}/tags/${tagId}`
    );
  }
}
