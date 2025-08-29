import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Comment, CreateCommentRequest } from '../../shared/models/comment';
import { CursorPagedResponse } from '../../shared/models/pagination';

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
    nextCursor: string | null
  ): Observable<CursorPagedResponse<Comment>> {
    let params = new HttpParams().set('resourceId', resourceId);

    if (nextCursor) {
      params = params.set('cursor', nextCursor);
    }

    return this.http.get<CursorPagedResponse<Comment>>(
      `${this.apiUrl}/comments`,
      {
        params,
      }
    );
  }

  deleteComment(commendId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/comments/${commendId}`);
  }
}
