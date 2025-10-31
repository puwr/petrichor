import { signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormBuilder } from '@angular/forms';
import { Router, ActivatedRoute, convertToParamMap } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { GalleryItem } from '../image.models';
import { GalleryPageComponent } from './gallery-page.component';
import { GalleryPageStore } from './store/gallery-page.store';

describe('GalleryPageComponent', () => {
  let component: GalleryPageComponent;
  let fixture: ComponentFixture<GalleryPageComponent>;

  let router: Router;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [GalleryPageComponent],
      providers: [
        FormBuilder,
        {
          provide: ActivatedRoute,
          useValue: new BehaviorSubject(
            convertToParamMap({ page: '1', tags: ['tag1', 'tag2'] }),
          ).asObservable(),
        },
      ],
    })
      .overrideProvider(GalleryPageStore, {
        useValue: {
          galleryItems: signal<GalleryItem[]>([]),
          searchTags: signal<string[]>([]),
          pageNumber: signal<number>(1),
          totalPages: signal<number>(1),
        },
      })
      .compileComponents();

    fixture = TestBed.createComponent(GalleryPageComponent);
    component = fixture.componentInstance;

    router = TestBed.inject(Router);
    vi.spyOn(router, 'navigate');

    fixture.detectChanges();
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
