import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuiEmail } from './gui-email';

describe('GuiEmail', () => {
  let component: GuiEmail;
  let fixture: ComponentFixture<GuiEmail>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GuiEmail]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GuiEmail);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
