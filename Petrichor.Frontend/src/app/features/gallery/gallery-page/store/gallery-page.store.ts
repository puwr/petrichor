import { withDevtools } from '@angular-architects/ngrx-toolkit';
import { inject } from '@angular/core';
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
import { GalleryItem } from '../../image.models';
import { ImageService } from '../../image.service';
import { initialGalleryPageSlice } from './gallery-page.slice';
import { setTotalPages, setPageNumber, setSearchTags } from './gallery-page.updaters';

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
