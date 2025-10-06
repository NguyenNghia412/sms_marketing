import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScanQrSv } from './scan-qr-sv';

describe('ScanQrSv', () => {
  let component: ScanQrSv;
  let fixture: ComponentFixture<ScanQrSv>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ScanQrSv]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScanQrSv);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
