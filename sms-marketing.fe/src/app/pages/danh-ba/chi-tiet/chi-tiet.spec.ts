import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChiTiet } from './chi-tiet';

describe('ChiTiet', () => {
  let component: ChiTiet;
  let fixture: ComponentFixture<ChiTiet>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChiTiet]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChiTiet);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
