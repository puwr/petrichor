export type PagedResponse<T> = {
  items: T[];
  count: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
};

export type CursorPagedResponse<T> = {
  items: T[];
  nextCursor: string | null;
  hasMore: boolean;
};
