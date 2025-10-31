import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { CursorPagedResponse } from '@app/core/pagination.models';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Comment, CreateCommentRequest } from './comment.models';

@Injectable({
  providedIn: 'root',
})
export class CommentService {
  private http = inject(HttpClient);

  private apiUrl = environment.apiUrl;

  createComment(commentData: CreateCommentRequest): Observable<string> {
    return this.http.post<string>(`${this.apiUrl}/comments`, commentData);
  }

  getComments(
    resourceId: string,
    nextCursor: string | null,
  ): Observable<CursorPagedResponse<Comment>> {
    let params = new HttpParams().set('resourceId', resourceId);

    if (nextCursor) {
      params = params.set('cursor', nextCursor);
    }

    return this.http.get<CursorPagedResponse<Comment>>(`${this.apiUrl}/comments`, {
      params,
    });
  }

  deleteComment(commendId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/comments/${commendId}`);
  }
}
