import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import {
  HttpClient,
  HttpContext,
  HttpEvent,
  HttpEventType,
  HttpProgressEvent,
  HttpResponse,
} from '@angular/common/http';
import { Image, GalleryItem, UploadEvent } from '../../shared/models/image';
import { filter, map, Observable } from 'rxjs';
import { SKIP_GLOBAL_LOADING } from '../../shared/constants';

@Injectable({
  providedIn: 'root',
})
export class ImageService {
  private http = inject(HttpClient);

  private apiUrl = environment.apiUrl;

  uploadImage(file: File): Observable<UploadEvent> {
    const formData = new FormData();
    formData.append('imagefile', file);

    return this.http
      .post<string>(`${this.apiUrl}/images`, formData, {
        context: new HttpContext().set(SKIP_GLOBAL_LOADING, true),
        reportProgress: true,
        observe: 'events',
      })
      .pipe(
        filter(
          (
            event: HttpEvent<string>
          ): event is HttpProgressEvent | HttpResponse<string> =>
            event.type === HttpEventType.UploadProgress ||
            event.type === HttpEventType.Response
        ),
        map((event) => {
          if (event.type === HttpEventType.UploadProgress) {
            const progress = event.total
              ? Math.round((100 * event.loaded) / event.total)
              : 0;

            return {
              type: 'progress' as const,
              progress,
            };
          }

          const response = event as HttpResponse<string>;

          return {
            type: 'complete' as const,
            imageUrl: response.headers.get('Location') ?? '',
          };
        })
      );
  }

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
