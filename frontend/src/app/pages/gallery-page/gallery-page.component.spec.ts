import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Mock } from 'vitest';
import { GalleryPageComponent } from './gallery-page.component';
import { BehaviorSubject, Observable, of } from 'rxjs';
import {
  ActivatedRoute,
  convertToParamMap,
  ParamMap,
  Router,
} from '@angular/router';
import { PagedResponse } from '../../shared/models/pagination';
import { GalleryItem } from '../../shared/models/image';
import { FormBuilder } from '@angular/forms';
import { ImageService } from '../../core/services/image.service';

describe('GalleryPageComponent', () => {
  let component: GalleryPageComponent;
  let fixture: ComponentFixture<GalleryPageComponent>;

  let router: Router;
  let routeParams: BehaviorSubject<ParamMap>;
  let route: { queryParamMap: Observable<ParamMap> };
  let imageService: { getImages: Mock };

  beforeEach(() => {
    routeParams = new BehaviorSubject(
      convertToParamMap({ page: '1', tags: ['tag1', 'tag2'] })
    );
    route = {
      queryParamMap: routeParams.asObservable(),
    };
    imageService = {
      getImages: vi.fn().mockReturnValue(
        of({
          items: [],
          pageNumber: 1,
          pageSize: 5,
          totalPages: 3,
          count: 0,
        } as PagedResponse<GalleryItem>)
      ),
    };

    TestBed.configureTestingModule({
      imports: [GalleryPageComponent],
      providers: [
        FormBuilder,
        { provide: ActivatedRoute, useValue: route },
        { provide: ImageService, useValue: imageService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(GalleryPageComponent);
    component = fixture.componentInstance;

    router = TestBed.inject(Router);
    vi.spyOn(router, 'navigate');

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize searchTags from route parameters', () => {
    expect(component.searchTags).toEqual(['tag1', 'tag2']);
  });

  describe('galleryData$', () => {
    it('should load data with correct parameters', () => {
      expect(imageService.getImages).toHaveBeenCalledTimes(1);
      expect(imageService.getImages).toHaveBeenCalledWith(1, ['tag1', 'tag2']);
    });

    it('should reload data when either page or tags change', () => {
      routeParams.next(
        convertToParamMap({ page: '2', tags: ['tag1', 'tag2'] })
      );
      routeParams.next(convertToParamMap({ page: '1', tags: ['tag3'] }));

      expect(imageService.getImages).toHaveBeenCalledTimes(3);
    });

    it('should not reload data when tags are identical or change case', () => {
      routeParams.next(
        convertToParamMap({ page: '1', tags: ['Tag1', 'tag2'] })
      );

      expect(imageService.getImages).toHaveBeenCalledTimes(1);
    });

    it('should redirect to last valid page when requested page exceeds total pages', () => {
      routeParams.next(convertToParamMap({ page: '5' }));

      expect(router.navigate).toHaveBeenCalledTimes(1);
      expect(router.navigate).toHaveBeenCalledWith([], {
        queryParams: { page: 3 },
        queryParamsHandling: 'merge',
      });
    });
  });

  describe('onSearch', () => {
    it('should process tags correctly on search submission', () => {
      component.searchForm.setValue({ tags: '  tag1,  tag2,  ' });
      component.onSearch();

      expect(router.navigate).toHaveBeenCalledTimes(1);
      expect(router.navigate).toHaveBeenCalledWith([], {
        queryParams: { page: null, tags: ['tag1', 'tag2'] },
        queryParamsHandling: 'merge',
      });
      expect(component.searchForm.value.tags).toBeNull();
    });

    it('should ignore empty or whitespace-only tag submissions', () => {
      component.searchForm.setValue({ tags: ' ' });
      component.onSearch();

      expect(router.navigate).not.toHaveBeenCalled();
    });
  });
});
