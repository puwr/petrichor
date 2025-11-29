import { withDevtools } from '@angular-architects/ngrx-toolkit';
import { patchState, signalStore, withMethods, withProps, withState } from '@ngrx/signals';
import { ImageService } from '../image.service';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { SnackbarService } from '@app/core/snackbar.service';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { EMPTY, finalize, switchMap, tap } from 'rxjs';
import { isCompleteEvent, isProgressEvent, UploadEvent } from '../image.models';
import { tapResponse } from '@ngrx/operators';
import { ValidationError } from '@angular/forms/signals';

interface UploadPageSlice {
  previewUrl: string | ArrayBuffer | null;
  uploadProgress: number;
  validationErrors: ValidationError[];
}

const initialUploadPageSlice: UploadPageSlice = {
  previewUrl: null,
  uploadProgress: 0,
  validationErrors: [],
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
        switchMap((file) => {
          patchState(store, { validationErrors: [] });

          if (!file.type.match('image.*')) {
            patchState(store, {
              validationErrors: [
                { kind: 'invalidFileType', message: 'Only image files are allowed.' },
              ],
            });

            return EMPTY;
          }

          return store._imageService.uploadImage(file).pipe(
            tapResponse({
              next: (event: UploadEvent) => {
                if (!store.previewUrl()) {
                  _filePreviewReader = new FileReader();
                  _filePreviewReader.onload = () => {
                    patchState(store, { previewUrl: _filePreviewReader?.result ?? null });
                  };
                  _filePreviewReader.readAsDataURL(file);
                }

                if (isProgressEvent(event)) {
                  patchState(store, { uploadProgress: event.progress });
                }

                if (isCompleteEvent(event)) {
                  patchState(store, { uploadProgress: 100 });
                  store._snackbar.success('Image uploaded successfully!');
                  store._router.navigateByUrl(event.imageUrl);
                }
              },
              error: (errors: ValidationError[]) => {
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
