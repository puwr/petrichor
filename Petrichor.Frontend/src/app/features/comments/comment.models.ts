import { CurrentUser } from '@app/core/account/account.models';

export type CreateCommentRequest = {
  resourceId: string;
  message: string;
};

export type Comment = {
  id: string;
  resourceId: string;
  author: string;
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
    author: currentUser.userName,
    message,
    createdAtUtc: new Date(),
  };
}
