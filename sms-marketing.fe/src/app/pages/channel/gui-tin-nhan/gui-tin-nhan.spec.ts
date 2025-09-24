import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuiTinNhan } from './gui-tin-nhan';

describe('GuiTinNhan', () => {
  let component: GuiTinNhan;
  let fixture: ComponentFixture<GuiTinNhan>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GuiTinNhan]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GuiTinNhan);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
