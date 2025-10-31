export interface GalleryPageSlice {
  pageNumber: number;
  totalPages: number;
  searchTags: string[];
}

export const initialGalleryPageSlice: GalleryPageSlice = {
  pageNumber: 1,
  totalPages: 0,
  searchTags: [],
};
