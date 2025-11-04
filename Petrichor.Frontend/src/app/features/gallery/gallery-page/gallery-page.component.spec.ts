import { signal } from '@angular/core';
import { Router } from '@angular/router';
import { GalleryItem } from '../image.models';
import { GalleryPageComponent } from './gallery-page.component';
import { GalleryPageStore } from './store/gallery-page.store';
import { render, screen, waitFor } from '@testing-library/angular';
import userEvent from '@testing-library/user-event';

describe('GalleryPageComponent', () => {
  it('renders "search results for" message when searchTags are present', async () => {
    const { galleryPageStore } = await setup();

    galleryPageStore.searchTags.set(['tag1', 'tag2']);

    await waitFor(() => {
      expect(screen.getByText(/search results for: tag1, tag2/i)).toBeInTheDocument();
    });
  });

  it('does not render "search results for" message when searchTags are absent', async () => {
    await setup();

    expect(screen.queryByText(/search results for: tag1, tag2/i)).not.toBeInTheDocument();
  });

  it('processes tags correctly on search submission', async () => {
    const { user, router } = await setup();

    await user.type(screen.getByPlaceholderText(/search by tags/i), '  tag1,  tag2   ');
    await user.click(screen.getByRole('button', { name: /search/i }));

    expect(router.navigate).toHaveBeenCalledWith([], {
      queryParams: { page: null, tags: ['tag1', 'tag2'] },
      queryParamsHandling: 'merge',
    });
    expect(screen.getByPlaceholderText(/search by tags/i)).toHaveValue('');
  });

  it('ignores empty or whitespace-only tag submissions', async () => {
    const { user, router } = await setup();

    await user.type(screen.getByPlaceholderText(/search by tags/i), '  ');
    await user.click(screen.getByRole('button', { name: /search/i }));

    expect(router.navigate).not.toHaveBeenCalled();
  });
});

async function setup() {
  const user = userEvent.setup();
  const galleryPageStore = {
    galleryItems: signal<GalleryItem[]>([]),
    searchTags: signal<string[]>([]),
    pageNumber: signal<number>(1),
    totalPages: signal<number>(1),
  };
  const router = { navigate: vi.fn() };

  await render(GalleryPageComponent, {
    providers: [
      {
        provide: Router,
        useValue: router,
      },
    ],
    configureTestBed: (testBed) =>
      testBed.overrideProvider(GalleryPageStore, { useValue: galleryPageStore }),
  });

  return { user, galleryPageStore, router };
}
