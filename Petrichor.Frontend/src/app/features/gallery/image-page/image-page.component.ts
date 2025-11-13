import { Dialog } from '@angular/cdk/dialog';
import { DatePipe } from '@angular/common';
import { Component, OnInit, OnDestroy, inject, DestroyRef, Signal, computed } from '@angular/core';
import { toSignal, takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthStore } from '@app/core/auth';
import { LoadingService } from '@app/core/loading/loading.service';
import { CommentsComponent } from '@app/features/comments/comments.component';
import { ButtonComponent, IconComponent, DialogComponent } from '@app/shared/components';
import { DialogData } from '@app/shared/components/dialog/dialog.models';
import { Subject, switchMap, startWith, of, filter } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ImageService } from '../image.service';
import { TagsComponent } from './image-tags/image-tags.component';
import { Image } from '../image.models';

@Component({
  selector: 'app-image-page',
  imports: [TagsComponent, DatePipe, CommentsComponent, ButtonComponent, IconComponent],
  templateUrl: './image-page.component.html',
  styleUrl: './image-page.component.scss',
})
export class ImagePageComponent implements OnInit, OnDestroy {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private dialog = inject(Dialog);
  private imageService = inject(ImageService);
  private loadingService = inject(LoadingService);
  readonly authStore = inject(AuthStore);
  private destroyRef = inject(DestroyRef);

  baseUrl = environment.baseUrl;

  private refreshImage$ = new Subject<void>();

  imageId: string | null = null;

  image: Signal<Image | null> = toSignal(
    this.route.paramMap.pipe(
      switchMap((params) => {
        this.imageId = params.get('id');

        return this.refreshImage$.pipe(
          startWith(undefined),
          switchMap(() => (this.imageId ? this.imageService.getImage(this.imageId) : of(null))),
        );
      }),
    ),
    { initialValue: null },
  );

  isUploaderOrAdmin = computed(() => this.authStore.isResourceOwnerOrAdmin(this.image()?.uploader));

  ngOnInit(): void {
    this.loadingService.show();
  }

  ngOnDestroy(): void {
    this.loadingService.hide();
  }

  onImageLoad(): void {
    this.loadingService.hide();
  }

  onImageDelete(): void {
    const dialogRef = this.dialog.open<boolean, DialogData>(DialogComponent, {
      data: {
        title: 'Image deletion',
        message: 'The image will be removed forever. Proceed?',
        actions: [
          {
            text: 'Cancel',
            style: 'neutral',
            result: false,
          },
          {
            text: 'Delete',
            style: 'danger',
            result: true,
          },
        ],
      },
    });

    dialogRef.closed
      .pipe(
        filter((result) => result === true),
        switchMap(() => this.imageService.deleteImage(this.image()!.id)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(() => this.router.navigateByUrl('/'));
  }

  refreshImage(): void {
    this.refreshImage$.next();
  }
}
