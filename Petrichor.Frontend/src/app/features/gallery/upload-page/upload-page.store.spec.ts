import { TestBed } from '@angular/core/testing';
import { UploadPageStore } from './upload-page.store';
import { ImageService } from '../image.service';
import { SnackbarService } from '@app/core/snackbar.service';
import { Router } from '@angular/router';
import { BehaviorSubject, EMPTY, Observable, of, Subject, tap, throwError } from 'rxjs';
import { UploadEvent } from '../image.models';

describe('UploadPageStore', () => {
  describe('uploadImage', () => {
    it('validates file type', () => {
      const { store } = setup();

      store.uploadImage(invalidMockFile);

      const errors = store.validationErrors();
      expect(errors[0]).not.toBeNull();
      expect(errors[0].message).toBe('Only image files are allowed.');
    });

    it('updates upload percentage and sets previewUrl', () => {
      const { store, imageService } = setup();

      const uploadEventSubject = new Subject<UploadEvent>();
      imageService.uploadImage.mockReturnValue(uploadEventSubject);

      store.uploadImage(mockFile);

      expect(store.uploadProgress()).toBe(0);

      uploadEventSubject.next({ type: 'progress' as const, progress: 10 } as UploadEvent);
      expect(store.uploadProgress()).toBe(10);

      uploadEventSubject.next({ type: 'progress' as const, progress: 50 } as UploadEvent);
      expect(store.uploadProgress()).toBe(50);

      uploadEventSubject.complete();
    });
  });

  it('sets previewUrl after reading file', async () => {
    const { store, imageService } = setup();

    vi.useFakeTimers();

    const uploadEventSubject = new Subject<UploadEvent>();
    imageService.uploadImage.mockReturnValue(uploadEventSubject);

    store.uploadImage(mockFile);

    uploadEventSubject.next({ type: 'progress' as const, progress: 10 } as UploadEvent);

    await vi.runAllTimersAsync();

    expect(store.previewUrl()).not.toBeNull();
  });

  it('sets validationErrors and resets progress on failure', () => {
    const { store, imageService } = setup();

    imageService.uploadImage.mockReturnValue(throwError(() => ['File is corrupted.']));

    store.uploadImage(mockFile);

    expect(store.validationErrors()).toContain('File is corrupted.');
    expect(store.uploadProgress()).toBe(0);
    expect(store.previewUrl()).toBeNull();
  });

  it('shows success snackbar and redirects to image page on complete', () => {
    const { store, imageService, snackbar, router } = setup();
    vi.spyOn(router, 'navigateByUrl');

    imageService.uploadImage.mockReturnValue(
      of({ type: 'complete', imageUrl: '/uploads/test.jpg' } as UploadEvent),
    );

    store.uploadImage(mockFile);

    expect(store.uploadProgress()).toBe(100);
    expect(snackbar.success).toHaveBeenCalledExactlyOnceWith('Image uploaded successfully!');
    expect(router.navigateByUrl).toHaveBeenCalledExactlyOnceWith('/uploads/test.jpg');
  });
});

function setup() {
  const imageService = { uploadImage: vi.fn().mockReturnValue(of(undefined)) };
  const snackbar = { success: vi.fn() };

  TestBed.configureTestingModule({
    providers: [
      UploadPageStore,
      { provide: ImageService, useValue: imageService },
      { provide: SnackbarService, useValue: snackbar },
    ],
  });

  const store = TestBed.inject(UploadPageStore);
  const router = TestBed.inject(Router);

  return { store, imageService, router, snackbar };
}

const mockBlob = new Blob(['image data'], { type: 'image/jpeg' });
const mockFile = new File([mockBlob], 'test.jpg', { type: 'image/jpeg' });
const invalidMockFile = new File([mockBlob], 'test.zip', { type: 'application/zip' });
