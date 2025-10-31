import { Injectable } from '@angular/core';
import { BehaviorSubject, asapScheduler } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LoadingService {
  private _loading$ = new BehaviorSubject<boolean>(false);
  public loading$ = this._loading$.asObservable();
  private requestCount = 0;

  show() {
    this.requestCount++;
    asapScheduler.schedule(() => this._loading$.next(true));
  }

  hide() {
    this.requestCount = Math.max(0, this.requestCount - 1);

    if (this.requestCount === 0) {
      asapScheduler.schedule(() => this._loading$.next(false));
    }
  }
}
