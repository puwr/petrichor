export type Image = {
  id: string;
  url: string;
  width: number;
  height: number;
  uploader: string | null;
  tags: string[];
  uploadedAt: Date;
};

export type GalleryItem = {
  id: string;
  thumbnailUrl: string;
  thumbnailWidth: number;
  thumbnailHeight: number;
};

export type UploadEvent =
  | { type: 'progress'; progress: number }
  | { type: 'complete'; imageUrl: string };

export const isProgressEvent = (
  event: UploadEvent,
): event is { type: 'progress'; progress: number } => event.type === 'progress';

export const isCompleteEvent = (
  event: UploadEvent,
): event is { type: 'complete'; imageUrl: string } => event.type === 'complete';
