export interface CommentSlice {
	resourceId: string | null;
	nextCursor: string | null;
	validationErrors: string[] | null;
}

export const initialCommentSlice: CommentSlice = {
	resourceId: null,
	nextCursor: null,
	validationErrors: null,
};
