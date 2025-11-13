import {
  HttpClient,
  HttpContext,
  HttpEvent,
  HttpProgressEvent,
  HttpResponse,
  HttpEventType,
  HttpParams,
} from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { SKIP_GLOBAL_LOADING } from '@app/core/http-tokens';
import { PagedResponse } from '@app/core/pagination.models';
import { Observable, filter, map } from 'rxjs';
import { environment } from 'src/environments/environment';
import { UploadEvent, GalleryItem, Image } from './image.models';

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
          (event: HttpEvent<string>): event is HttpProgressEvent | HttpResponse<string> =>
            event.type === HttpEventType.UploadProgress || event.type === HttpEventType.Response,
        ),
        map((event) => {
          if (event.type === HttpEventType.UploadProgress) {
            const progress = event.total ? Math.round((100 * event.loaded) / event.total) : 0;

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
        }),
      );
  }

  getImages(
    pageNumber: number,
    tags: string[],
    uploader: string | null,
  ): Observable<PagedResponse<GalleryItem>> {
    let params = new HttpParams().set('page', pageNumber);

    tags.forEach((tag) => {
      params = params.append('tags', tag);
    });

    if (uploader) {
      params = params.append('uploader', uploader);
    }

    return this.http.get<PagedResponse<GalleryItem>>(`${this.apiUrl}/images`, {
      params,
    });
  }

  getImage(id: string): Observable<Image> {
    return this.http.get<Image>(`${this.apiUrl}/images/${id}`);
  }

  deleteImage(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/images/${id}`);
  }

  updateImageTags(imageId: string, tags: string[]): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/images/${imageId}/tags`, {
      tags,
    });
  }
}
