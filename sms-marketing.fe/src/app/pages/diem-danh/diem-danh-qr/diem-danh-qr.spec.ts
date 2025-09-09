import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DiemDanhQr } from './diem-danh-qr';

describe('DiemDanhQr', () => {
  let component: DiemDanhQr;
  let fixture: ComponentFixture<DiemDanhQr>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DiemDanhQr]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DiemDanhQr);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
