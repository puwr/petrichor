import { Component, OnDestroy, inject, DestroyRef, ViewChild, ElementRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { SnackbarService } from '@app/core/snackbar.service';
import { ProgressBarComponent, ValidationErrorsComponent } from '@app/shared/components';
import { tap, finalize } from 'rxjs';
import { UploadEvent, isProgressEvent, isCompleteEvent } from '../image.models';
import { ImageService } from '../image.service';

@Component({
  selector: 'app-upload-page',
  imports: [ProgressBarComponent, ValidationErrorsComponent],
  templateUrl: './upload-page.component.html',
  styleUrl: './upload-page.component.scss',
})
export class UploadPageComponent implements OnDestroy {
  private snackbar = inject(SnackbarService);
  private imageService = inject(ImageService);
  private router = inject(Router);
  private destroyRef = inject(DestroyRef);

  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  private filePreviewReader: FileReader | null = null;

  isDragOver = false;
  previewUrl: string | ArrayBuffer | null = null;
  uploadPercentage = 0;

  validationErrors: string[] | null = null;

  ngOnDestroy(): void {
    this.previewUrl = null;
  }

  triggerFileInput() {
    this.fileInput.nativeElement.click();
  }

  onFileSeleted(event: Event) {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {
      this.handleFile(input.files[0]);
    }
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = true;
  }

  onDragLeave(event: DragEvent) {
    this.isDragOver = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = false;

    if (event.dataTransfer?.files && event.dataTransfer.files.length > 0) {
      this.handleFile(event.dataTransfer.files[0]);
    }
  }

  onKeydown(event: KeyboardEvent) {
    if (event.key === 'Enter' || event.code === 'Space') {
      event.preventDefault();
      this.triggerFileInput();
    }
  }

  private handleFile(file: File) {
    if (!file.type.match('image.*')) {
      this.snackbar.error('Only image files are allowed.');
      return;
    }

    this.validationErrors = null;

    this.filePreviewReader = new FileReader();
    this.filePreviewReader.onload = () => {
      if (this.uploadPercentage > 0) {
        this.previewUrl = this.filePreviewReader?.result ?? null;
      }
    };

    this.imageService
      .uploadImage(file)
      .pipe(
        tap({
          next: () => {
            if (!this.previewUrl && this.filePreviewReader) {
              this.filePreviewReader.readAsDataURL(file);
            }
          },
        }),
        takeUntilDestroyed(this.destroyRef),
        finalize(() => {
          if (this.filePreviewReader) {
            this.filePreviewReader.onload = null;
            this.filePreviewReader.abort();
            this.filePreviewReader = null;
          }
        }),
      )
      .subscribe({
        next: (event: UploadEvent) => {
          if (isProgressEvent(event)) {
            this.uploadPercentage = event.progress;
          }

          if (isCompleteEvent(event)) {
            this.uploadPercentage = 100;
            this.snackbar.success('Image uploaded successfully!');

            this.router.navigateByUrl(event.imageUrl);
          }
        },
        error: (errors) => {
          this.validationErrors = errors;
          this.uploadPercentage = 0;
          this.previewUrl = null;
        },
      });
  }
}
