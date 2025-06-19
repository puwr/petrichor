import { Tag } from './tag';

export type Image = {
  id: string;
  url: string;
  width: number;
  height: number;
  uploaderId: string;
  tags: Tag[];
  uploadedAt: Date;
};
