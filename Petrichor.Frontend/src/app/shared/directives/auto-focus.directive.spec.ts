import { render, screen } from '@testing-library/angular';
import { AutoFocusDirective } from './auto-focus.directive';
import { TestBed } from '@angular/core/testing';
import { FocusMonitor } from '@angular/cdk/a11y';

describe('AutoFocusDirective', () => {
  it('focuses element', async () => {
    await setup();

    expect(screen.getByPlaceholderText('test')).toHaveFocus();
  });

  it('cleans up focus monitoring on component destruction', async () => {
    const { fixture } = await setup();

    const inputEl = screen.getByPlaceholderText('test');
    const focusMonitor = TestBed.inject(FocusMonitor);
    vi.spyOn(focusMonitor, 'stopMonitoring');

    fixture.destroy();

    expect(focusMonitor.stopMonitoring).toHaveBeenCalledWith(inputEl);
  });
});

async function setup() {
  const { fixture } = await render('<input type="text" placeholder="test" appAutoFocus />', {
    imports: [AutoFocusDirective],
  });

  return { fixture };
}
