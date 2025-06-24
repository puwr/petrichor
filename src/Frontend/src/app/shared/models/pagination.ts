export type PagedResponse<T> = {
  items: T[];
  count: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
};
