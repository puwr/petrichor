import { PartialStateUpdater } from '@ngrx/signals';
import { GalleryPageSlice } from './gallery-page.slice';

export function setPageNumber(pageNumber: number): PartialStateUpdater<GalleryPageSlice> {
  return () => ({
    pageNumber,
  });
}

export function setTotalPages(totalPages: number): PartialStateUpdater<GalleryPageSlice> {
  return () => ({ totalPages });
}

export function setSearchTags(searchTags: string[]): PartialStateUpdater<GalleryPageSlice> {
  return () => ({ searchTags });
}
