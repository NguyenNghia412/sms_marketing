import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Create } from '@/pages/channel/sms/create/create';
import { UpdateUserCredits } from './update-user-credits';



describe('UpdateUserCredits', () => {
  let component: UpdateUserCredits;
  let fixture: ComponentFixture<UpdateUserCredits>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateUserCredits]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UpdateUserCredits);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
