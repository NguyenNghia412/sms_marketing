import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UpdateUserSuDungDichVu } from './update-user-su-dung-dich-vu';



describe('UpdateUserSuDungDichVu', () => {
  let component: UpdateUserSuDungDichVu;
  let fixture: ComponentFixture<UpdateUserSuDungDichVu>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateUserSuDungDichVu]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UpdateUserSuDungDichVu);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
