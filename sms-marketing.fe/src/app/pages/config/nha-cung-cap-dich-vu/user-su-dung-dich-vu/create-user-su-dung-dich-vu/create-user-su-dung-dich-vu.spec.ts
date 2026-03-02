import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CreateUserSuDungDichVu } from './create-user-su-dung-dich-vu';



describe('CreateUserSuDungDichVu', () => {
  let component: CreateUserSuDungDichVu;
  let fixture: ComponentFixture<CreateUserSuDungDichVu>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateUserSuDungDichVu]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateUserSuDungDichVu);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
