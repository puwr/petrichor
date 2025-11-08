import { withDevtools } from '@angular-architects/ngrx-toolkit';
import { patchState, signalStore, withMethods, withProps, withState } from '@ngrx/signals';
import { ImageService } from '../image.service';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { SnackbarService } from '@app/core/snackbar.service';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { finalize, switchMap, tap } from 'rxjs';
import { isCompleteEvent, isProgressEvent, UploadEvent } from '../image.models';
import { tapResponse } from '@ngrx/operators';

interface UploadPageSlice {
  previewUrl: string | ArrayBuffer | null;
  uploadProgress: number;
  validationErrors: string[] | null;
}

const initialUploadPageSlice: UploadPageSlice = {
  previewUrl: null,
  uploadProgress: 0,
  validationErrors: null,
};

export const UploadPageStore = signalStore(
  withState(initialUploadPageSlice),
  withProps(() => ({
    _imageService: inject(ImageService),
    _snackbar: inject(SnackbarService),
    _router: inject(Router),
  })),
  withMethods((store) => {
    let _filePreviewReader: FileReader | null;

    const uploadImage = rxMethod<File>((file$) =>
      file$.pipe(
        tap((file) => {
          patchState(store, { validationErrors: null });

          if (!file.type.match('image.*')) {
            patchState(store, { validationErrors: ['Only image files are allowed.'] });
            return;
          }

          _filePreviewReader = new FileReader();
          _filePreviewReader.onload = () => {
            patchState(store, { previewUrl: _filePreviewReader?.result ?? null });
          };
          _filePreviewReader.readAsDataURL(file);
        }),
        switchMap((file) => {
          return store._imageService.uploadImage(file).pipe(
            tapResponse({
              next: (event: UploadEvent) => {
                if (isProgressEvent(event)) {
                  patchState(store, { uploadProgress: event.progress });
                }

                if (isCompleteEvent(event)) {
                  patchState(store, { uploadProgress: 100 });
                  store._snackbar.success('Image uploaded successfully!');
                  store._router.navigateByUrl(event.imageUrl);
                }
              },
              error: (errors: string[] | null) => {
                patchState(store, {
                  validationErrors: errors,
                  uploadProgress: 0,
                  previewUrl: null,
                });
              },
            }),
            finalize(() => {
              if (_filePreviewReader) {
                _filePreviewReader.onload = null;
                _filePreviewReader.abort();
                _filePreviewReader = null;
              }
            }),
          );
        }),
      ),
    );

    return { uploadImage };
  }),
  withDevtools('upload-page'),
);
