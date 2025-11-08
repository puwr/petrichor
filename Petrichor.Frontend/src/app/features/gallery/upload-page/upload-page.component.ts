import { Component, ViewChild, ElementRef, inject } from '@angular/core';
import { ProgressBarComponent, ValidationErrorsComponent } from '@app/shared/components';
import { UploadPageStore } from './upload-page.store';

@Component({
  selector: 'app-upload-page',
  imports: [ProgressBarComponent, ValidationErrorsComponent],
  providers: [UploadPageStore],
  templateUrl: './upload-page.component.html',
  styleUrl: './upload-page.component.scss',
})
export class UploadPageComponent {
  readonly uploadPageStore = inject(UploadPageStore);

  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  isDragOver = false;

  triggerFileInput() {
    this.fileInput.nativeElement.click();
  }

  onFileSeleted(event: Event) {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {
      this.uploadPageStore.uploadImage(input.files[0]);
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
      this.uploadPageStore.uploadImage(event.dataTransfer.files[0]);
    }
  }

  onKeydown(event: KeyboardEvent) {
    if (event.key === 'Enter' || event.code === 'Space') {
      event.preventDefault();
      this.triggerFileInput();
    }
  }
}
