import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HopTrucTuyen } from './hop-truc-tuyen';

describe('HopTrucTuyen', () => {
  let component: HopTrucTuyen;
  let fixture: ComponentFixture<HopTrucTuyen>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HopTrucTuyen]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HopTrucTuyen);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
