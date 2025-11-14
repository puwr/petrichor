import { withDevtools } from '@angular-architects/ngrx-toolkit';
import { computed, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router, ActivatedRoute } from '@angular/router';
import { tapResponse } from '@ngrx/operators';
import {
  signalStore,
  type,
  withState,
  withComputed,
  withProps,
  withMethods,
  patchState,
  withHooks,
} from '@ngrx/signals';
import { withEntities, setAllEntities } from '@ngrx/signals/entities';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, distinctUntilChanged, map } from 'rxjs';
import { GalleryItem } from '../image.models';
import { ImageService } from '../image.service';

interface GalleryPageSlice {
  pageNumber: number;
  totalPages: number;
  searchTags: string[];
  selectedUploader: string | null;
}

const initialGalleryPageSlice: GalleryPageSlice = {
  pageNumber: 1,
  totalPages: 0,
  searchTags: [],
  selectedUploader: null,
};

export const GalleryPageStore = signalStore(
  withEntities({ entity: type<GalleryItem>(), collection: '_galleryItem' }),
  withState(initialGalleryPageSlice),
  withComputed((store) => ({
    galleryItems: store._galleryItemEntities,
    filterMessage: computed(() => {
      const searchTags = store.searchTags();
      const uploader = store.selectedUploader();

      if (uploader && searchTags.length > 0) {
        return `Images uploaded by ${uploader} with tags: ${searchTags.join(', ')}`;
      } else if (uploader) {
        return `Images uploaded by ${uploader}`;
      } else if (searchTags.length > 0) {
        return `Search results for: ${searchTags.join(', ')}`;
      }

      return '';
    }),
  })),
  withProps(() => ({
    _imageService: inject(ImageService),
    _router: inject(Router),
    _route: inject(ActivatedRoute),
  })),
  withMethods((store) => {
    const _getImages = rxMethod<void>(
      pipe(
        switchMap(() =>
          store._imageService
            .getImages(store.pageNumber(), store.searchTags(), store.selectedUploader())
            .pipe(
              tapResponse({
                next: (response) => {
                  if (response.totalPages > 0 && store.pageNumber() > response.totalPages) {
                    store._router.navigate([], {
                      queryParams: { page: response.totalPages },
                      queryParamsHandling: 'merge',
                    });
                  } else {
                    patchState(
                      store,
                      setAllEntities([...response.items], { collection: '_galleryItem' }),
                    );

                    patchState(store, { totalPages: response.totalPages });
                  }
                },
                error: console.error,
              }),
            ),
        ),
      ),
    );

    return { _getImages };
  }),
  withHooks({
    onInit(store) {
      store._route.queryParamMap
        .pipe(
          distinctUntilChanged((prev, curr) => {
            return (
              prev.get('page') === curr.get('page') &&
              areTagsEqual(prev.getAll('tags'), curr.getAll('tags')) &&
              prev.get('uploader') === curr.get('uploader')
            );
          }),
          map((params) => {
            const pageNumber = Number(params.get('page')) || 1;
            const searchTags = params.getAll('tags');
            const uploader = params.get('uploader');

            return { pageNumber, searchTags, uploader };
          }),
          takeUntilDestroyed(),
        )
        .subscribe(({ pageNumber, searchTags, uploader }) => {
          patchState(store, { pageNumber, searchTags, selectedUploader: uploader });
          store._getImages();
        });
    },
  }),
  withDevtools('gallery-page'),
);

function areTagsEqual(a: string[], b: string[]): boolean {
  if (!a || !b) {
    return a === b;
  }

  if (a.length !== b.length) {
    return false;
  }

  return a.every((tag, index) => tag.toLowerCase() === b[index].toLowerCase());
}
