import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Sms } from './sms';

describe('Sms', () => {
  let component: Sms;
  let fixture: ComponentFixture<Sms>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Sms]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Sms);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
