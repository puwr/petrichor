import {
  Component,
  computed,
  DestroyRef,
  inject,
  OnDestroy,
  OnInit,
  signal,
} from '@angular/core';
import { ImageService } from '../../core/services/image.service';
import { ActivatedRoute, Router } from '@angular/router';
import { map, of, switchMap } from 'rxjs';
import { DatePipe } from '@angular/common';
import { environment } from '../../../environments/environment';
import { TagsComponent } from './tags/tags.component';
import { LoadingService } from '../../core/services/loading.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Image } from '../../shared/models/image';
import { AccountService } from '../../core/services/account.service';

@Component({
  selector: 'app-image-page',
  imports: [TagsComponent, DatePipe],
  templateUrl: './image-page.component.html',
  styleUrl: './image-page.component.scss',
})
export class ImagePageComponent implements OnInit, OnDestroy {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private imageService = inject(ImageService);
  private loadingService = inject(LoadingService);
  private accountService = inject(AccountService);
  private destroyRef = inject(DestroyRef);

  baseUrl = environment.baseUrl;

  currentUser = this.accountService.currentUser;

  imageId$ = this.route.paramMap.pipe(map((params) => params.get('id')));

  image = signal<Image | null>(null);
  isUploader = computed(
    () => this.image()?.uploaderId === this.currentUser()?.id
  );

  ngOnInit(): void {
    this.loadingService.show();
    this.loadImage();
  }

  ngOnDestroy(): void {
    this.loadingService.hide();
  }

  handleTagsChanged(): void {
    this.loadImage();
  }

  onImageLoad(): void {
    this.loadingService.hide();
  }

  onImageDelete(): void {
    this.imageService
      .deleteImage(this.image()!.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.router.navigateByUrl('/');
      });
  }

  private loadImage(): void {
    this.imageId$
      .pipe(
        switchMap((id) => (id ? this.imageService.getImage(id) : of(null))),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe((image) => this.image.set(image));
  }
}
