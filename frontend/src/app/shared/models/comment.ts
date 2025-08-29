export type CreateCommentRequest = {
  resourceId: string;
  message: string;
};

export type Comment = {
  id: string;
  resourceId: string;
  authorId: string;
  authorUserName: string;
  message: string;
  createdAtUtc: Date;
};
