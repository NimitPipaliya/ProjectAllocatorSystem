import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { allocatorGuard } from './allocator.guard';

describe('allocatorGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => allocatorGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
