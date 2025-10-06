import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuestProfile } from './guest-profile';

describe('GuestProfile', () => {
  let component: GuestProfile;
  let fixture: ComponentFixture<GuestProfile>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GuestProfile]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GuestProfile);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
