import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsMain } from './sms-main';

describe('SmsMain', () => {
  let component: SmsMain;
  let fixture: ComponentFixture<SmsMain>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SmsMain]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SmsMain);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
