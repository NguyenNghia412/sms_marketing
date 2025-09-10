import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DotDiemDanh } from './dot-diem-danh';

describe('DotDiemDanh', () => {
  let component: DotDiemDanh;
  let fixture: ComponentFixture<DotDiemDanh>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DotDiemDanh]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DotDiemDanh);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
