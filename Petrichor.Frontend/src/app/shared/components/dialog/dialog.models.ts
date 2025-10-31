export type DialogData = {
  title: string;
  message: string;
  actions?: DialogAction[];
};

export type DialogAction = {
  text: string;
  style: 'neutral' | 'fill' | 'danger';
  result: boolean;
};
