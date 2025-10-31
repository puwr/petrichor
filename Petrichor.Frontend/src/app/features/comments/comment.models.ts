import { CurrentUser } from '@app/core/account/account.models';

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

export function makeComment(
  commentId: string,
  resourceId: string,
  message: string,
  currentUser: CurrentUser,
): Comment {
  return {
    id: commentId,
    resourceId,
    authorId: currentUser.id,
    authorUserName: currentUser.userName,
    message,
    createdAtUtc: new Date(),
  };
}
