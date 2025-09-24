import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MobilePreview } from './mobile-preview';

describe('MobilePreview', () => {
  let component: MobilePreview;
  let fixture: ComponentFixture<MobilePreview>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MobilePreview]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MobilePreview);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
