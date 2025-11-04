import { Component } from '@angular/core';
import { signal } from '@angular/core';
import { AuthStore } from '@app/core/auth';
import { CommentsComponent } from './comments.component';
import { render, screen } from '@testing-library/angular';

describe('CommentsComponent', () => {
  it('renders comment-form component when isAuthenticated is true', async () => {
    await setup({ isAuthenticated: true });

    expect(screen.getByTestId('comment-form')).toBeInTheDocument();
  });

  it('does not render comment-form component when isAuthenticated is false', async () => {
    await setup();

    expect(screen.queryByTestId('comment-form')).not.toBeInTheDocument();
  });

  it('renders comment-list component', async () => {
    await setup();

    expect(screen.getByTestId('comment-list')).toBeInTheDocument();
  });
});

async function setup(config: { isAuthenticated?: boolean } = {}) {
  const authStore = { isAuthenticated: signal<boolean>(config.isAuthenticated ?? false) };

  await render(CommentsComponent, {
    providers: [
      {
        provide: AuthStore,
        useValue: authStore,
      },
    ],
    componentImports: [MockCommentFormComponent, MockCommentListComponent],
  });
}

@Component({
  selector: 'app-comment-form',
  template: '<div data-testid="comment-form">app-comment-form</div>',
})
export class MockCommentFormComponent {}

@Component({
  selector: 'app-comment-list',
  template: '<div data-testid="comment-list">app-comment-list</div>',
})
export class MockCommentListComponent {}
