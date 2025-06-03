import { Component, inject, OnInit } from '@angular/core';
import { ImageService } from '../../core/services/image.service';
import { ActivatedRoute } from '@angular/router';
import { map, of, switchMap } from 'rxjs';
import { AsyncPipe, DatePipe } from '@angular/common';
import { environment } from '../../../environments/environment';
import { TagsComponent } from './tags/tags.component';
import { LoadingService } from '../../core/services/loading.service';

@Component({
  selector: 'app-image-page',
  imports: [TagsComponent, AsyncPipe, DatePipe],
  templateUrl: './image-page.component.html',
  styleUrl: './image-page.component.scss',
})
export class ImagePageComponent implements OnInit {
  private route = inject(ActivatedRoute);

  private imageService = inject(ImageService);
  public loadingService = inject(LoadingService);

  baseUrl = environment.baseUrl;

  imageId$ = this.route.paramMap.pipe(map((params) => params.get('id')));
  image$ = this.imageId$.pipe(
    switchMap((id) => (id ? this.imageService.getImage(id) : of(null)))
  );

  ngOnInit() {
    this.loadingService.show();
  }

  onImageLoad() {
    this.loadingService.hide();
  }
}
