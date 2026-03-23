import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UpdateToiDaHanMuc } from './update-toi-da-han-muc';





describe('UpdateToiDaHanMuc', () => {
  let component: UpdateToiDaHanMuc;
  let fixture: ComponentFixture<UpdateToiDaHanMuc>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateToiDaHanMuc]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UpdateToiDaHanMuc);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
