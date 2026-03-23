import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UpdateNhaCungCapDichVu } from './update-nha-cung-cap-dich-vu';




describe('UpdateNhaCungCapDichVu', () => {
  let component: UpdateNhaCungCapDichVu;
  let fixture: ComponentFixture<UpdateNhaCungCapDichVu>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateNhaCungCapDichVu]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UpdateNhaCungCapDichVu);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
