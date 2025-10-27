import { Component, computed, DestroyRef, inject, OnDestroy, OnInit, Signal } from '@angular/core';
import { ImageService } from '../../core/services/image.service';
import { ActivatedRoute, Router } from '@angular/router';
import { filter, of, startWith, Subject, switchMap } from 'rxjs';
import { DatePipe } from '@angular/common';
import { environment } from '../../../environments/environment';
import { TagsComponent } from './tags/tags.component';
import { LoadingService } from '../../core/services/loading.service';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { Image } from '../../shared/models/image';
import { Dialog } from '@angular/cdk/dialog';
import { DialogComponent } from '../../shared/components/dialog/dialog.component';
import { DialogData } from '../../shared/models/dialog';
import { CommentsComponent } from '../../features/comments/comments.component';
import { AuthStore } from '../../core/store/auth/auth.store';
import { ButtonComponent } from '../../shared/components/button/button.component';

@Component({
  selector: 'app-image-page',
  imports: [TagsComponent, DatePipe, CommentsComponent, ButtonComponent],
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

  isUploaderOrAdmin = computed(() =>
    this.authStore.isResourceOwnerOrAdmin(this.image()?.uploaderId),
  );

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
