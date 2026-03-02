import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UserNhaCungCapDichVu } from './user-su-dung-dich-vu';




describe('UserNhaCungCapDichVu', () => {
  let component: UserNhaCungCapDichVu;
  let fixture: ComponentFixture<UserNhaCungCapDichVu>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserNhaCungCapDichVu]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserNhaCungCapDichVu);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
