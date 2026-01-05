import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UserCreditsForUser } from './user-credits-for-user';




describe('UserCreditsForUser', () => {
  let component: UserCreditsForUser;
  let fixture: ComponentFixture<UserCreditsForUser>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserCreditsForUser]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserCreditsForUser);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
