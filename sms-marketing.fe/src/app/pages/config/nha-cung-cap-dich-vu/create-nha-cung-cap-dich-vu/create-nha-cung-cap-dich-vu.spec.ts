import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CreateNhaCungCapDichVu } from './create-nha-cung-cap-dich-vu';



describe('CreateNhaCungCapDichVu', () => {
  let component: CreateNhaCungCapDichVu;
  let fixture: ComponentFixture<CreateNhaCungCapDichVu>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateNhaCungCapDichVu]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateNhaCungCapDichVu);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
