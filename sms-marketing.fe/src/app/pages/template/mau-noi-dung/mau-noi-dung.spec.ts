import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MauNoiDung } from './mau-noi-dung';

describe('MauNoiDung', () => {
  let component: MauNoiDung;
  let fixture: ComponentFixture<MauNoiDung>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MauNoiDung]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MauNoiDung);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
