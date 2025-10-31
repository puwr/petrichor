import { TestBed } from '@angular/core/testing';
import { convertToParamMap, ActivatedRoute, Router } from '@angular/router';
import { PagedResponse } from '@app/core/pagination.models';
import { BehaviorSubject, of } from 'rxjs';
import { GalleryItem } from '../../image.models';
import { ImageService } from '../../image.service';
import { GalleryPageStore } from './gallery-page.store';

describe('GalleryStore', () => {
  it('should be created', () => {
    const { store } = setup();

    expect(store).toBeTruthy();
  });

  it('initializes with correct pageNumber and searchTags and loads galleryItems', () => {
    const { store, imageService } = setup();

    expect(store.pageNumber()).toBe(1);
    expect(store.searchTags()).toStrictEqual(['tag1', 'tag2']);
    expect(imageService.getImages).toHaveBeenCalledExactlyOnceWith(1, ['tag1', 'tag2']);
    expect(store.galleryItems().length).toBe(0);
  });

  it('reloads galleryItems when either pageNumber or searchTags change', () => {
    const { imageService, routeParams } = setup();

    routeParams.next(convertToParamMap({ page: '2', tags: ['tag1', 'tag2'] }));
    expect(imageService.getImages).toHaveBeenCalledWith(2, ['tag1', 'tag2']);

    routeParams.next(convertToParamMap({ page: '3', tags: ['tag3'] }));
    expect(imageService.getImages).toHaveBeenCalledWith(3, ['tag3']);
  });

  it('does not reload galleryItems when tags are identical', () => {
    const { imageService, routeParams } = setup();

    routeParams.next(convertToParamMap({ page: '1', tags: ['tag4', 'tag5'] }));
    routeParams.next(convertToParamMap({ page: '1', tags: ['tag4', 'tag5'] }));
    routeParams.next(convertToParamMap({ page: '1', tags: ['TAG4', 'tag5'] }));

    // First time was on store initalization
    expect(imageService.getImages).toHaveBeenCalledTimes(2);
  });

  it('redirects to last valid page when requested page exceeds total pages', () => {
    const { routeParams, router } = setup();
    vi.spyOn(router, 'navigate');

    routeParams.next(convertToParamMap({ page: '5' }));

    expect(router.navigate).toHaveBeenCalledExactlyOnceWith([], {
      queryParams: { page: 3 },
      queryParamsHandling: 'merge',
    });
  });
});

function setup() {
  let routeParams = new BehaviorSubject(convertToParamMap({ page: '1', tags: ['tag1', 'tag2'] }));

  TestBed.configureTestingModule({
    providers: [
      GalleryPageStore,
      {
        provide: ImageService,
        useValue: {
          getImages: vi.fn().mockReturnValue(
            of({
              items: [],
              count: 0,
              pageNumber: 1,
              pageSize: 5,
              totalPages: 3,
            } as PagedResponse<GalleryItem>),
          ),
        },
      },
      {
        provide: ActivatedRoute,
        useValue: {
          queryParamMap: routeParams.asObservable(),
        },
      },
    ],
  });

  const store = TestBed.inject(GalleryPageStore);
  const imageService = TestBed.inject(ImageService);
  const router = TestBed.inject(Router);

  return { store, imageService, router, routeParams };
}
