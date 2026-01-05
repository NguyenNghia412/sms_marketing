import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Create } from '@/pages/channel/sms/create/create';
import { CreateUserCredits } from './create-user-credits';


describe('CreateUserCredits', () => {
  let component: CreateUserCredits;
  let fixture: ComponentFixture<CreateUserCredits>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateUserCredits]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateUserCredits);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
