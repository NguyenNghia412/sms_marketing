import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DashboardSms } from './dashboard-sms';



describe('DashboardSms', () => {
  let component: DashboardSms;
  let fixture: ComponentFixture<DashboardSms>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardSms]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DashboardSms);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
