import {
  patchState,
  signalStore,
  type,
  withComputed,
  withHooks,
  withMethods,
  withProps,
  withState,
} from '@ngrx/signals';
import { initialGalleryPageSlice } from './gallery-page.slice';
import { setAllEntities, withEntities } from '@ngrx/signals/entities';
import { GalleryItem } from '../../../shared/models/image';
import { inject } from '@angular/core';
import { ImageService } from '../../../core/services/image.service';
import { ActivatedRoute, Router } from '@angular/router';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { distinctUntilChanged, map, pipe, switchMap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import { withDevtools } from '@angular-architects/ngrx-toolkit';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { setPageNumber, setSearchTags, setTotalPages } from './gallery-page.updaters';

export const GalleryPageStore = signalStore(
  withEntities({ entity: type<GalleryItem>(), collection: '_galleryItem' }),
  withState(initialGalleryPageSlice),
  withComputed(({ _galleryItemEntities }) => ({
    galleryItems: _galleryItemEntities,
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
          store._imageService.getImages(store.pageNumber(), store.searchTags()).pipe(
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

                  patchState(store, setTotalPages(response.totalPages));
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
              areTagsEqual(prev.getAll('tags'), curr.getAll('tags'))
            );
          }),
          map((params) => {
            const pageNumber = Number(params.get('page')) || 1;
            const searchTags = params.getAll('tags');

            return { pageNumber, searchTags };
          }),
          takeUntilDestroyed(),
        )
        .subscribe(({ pageNumber, searchTags }) => {
          patchState(store, setPageNumber(pageNumber), setSearchTags(searchTags));
          store._getImages();
        });
    },
  }),
  withDevtools('gallery'),
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
