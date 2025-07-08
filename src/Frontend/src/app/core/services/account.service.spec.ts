import { TestBed } from '@angular/core/testing';
import { AccountService } from './account.service';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { mockUser } from '../../../testing/fixtures';

describe('AccountService', () => {
  let accountService: AccountService;
  let httpMock: HttpTestingController;

  const apiUrl = environment.apiUrl;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        AccountService,
      ],
    });

    accountService = TestBed.inject(AccountService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(accountService).toBeTruthy();
  });

  it('should initialize currentUser as null', () => {
    expect(accountService.currentUser()).toBeNull();
  });

  describe('updateCurrentUser', () => {
    it('should update currentUser with user data on success', () => {
      accountService.updateCurrentUser().subscribe();

      const req = httpMock.expectOne(`${apiUrl}/account/me`);
      req.flush(mockUser);

      expect(accountService.currentUser()).toEqual(mockUser);
    });

    test.each([
      { status: 401, statusText: 'Unauthorized' },
      { status: 403, statusText: 'Forbidden' },
    ])(
      'should set currentUser to null on $status error',
      ({ status, statusText }) => {
        accountService.currentUser.set(mockUser);

        accountService.updateCurrentUser().subscribe();

        const req = httpMock.expectOne(`${apiUrl}/account/me`);
        req.flush(null, { status, statusText });

        expect(accountService.currentUser()).toBeNull();
      }
    );
  });
});
