import {
  Component,
  DestroyRef,
  ElementRef,
  inject,
  OnDestroy,
  ViewChild,
} from '@angular/core';
import { SnackbarService } from '../../core/services/snackbar.service';
import { ProgressBarComponent } from '../../shared/components/progress-bar/progress-bar.component';
import { ImageService } from '../../core/services/image.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  isCompleteEvent,
  isProgressEvent,
  UploadEvent,
} from '../../shared/models/image';
import { Router } from '@angular/router';

@Component({
  selector: 'app-upload-page',
  imports: [ProgressBarComponent],
  templateUrl: './upload-page.component.html',
  styleUrl: './upload-page.component.scss',
})
export class UploadPageComponent implements OnDestroy {
  private snackbar = inject(SnackbarService);
  private imageService = inject(ImageService);
  private router = inject(Router);
  private destroyRef = inject(DestroyRef);

  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  isDragOver = false;
  previewUrl: string | ArrayBuffer | null = null;
  uploadPercentage = 0;

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

    const reader = new FileReader();
    reader.onload = () => {
      this.previewUrl = reader.result;
    };

    reader.readAsDataURL(file);

    this.imageService
      .uploadImage(file)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((event: UploadEvent) => {
        if (isProgressEvent(event)) {
          this.uploadPercentage = event.progress;
        }

        if (isCompleteEvent(event)) {
          this.uploadPercentage = 100;
          this.snackbar.success('Image uploaded successfully!');

          this.router.navigateByUrl(event.imageUrl);
        }
      });
  }
}
