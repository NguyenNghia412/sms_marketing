import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NhaCungCapDichVu } from './nha-cung-cap-dich-vu';



describe('NhaCungCapDichVu', () => {
  let component: NhaCungCapDichVu;
  let fixture: ComponentFixture<NhaCungCapDichVu>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NhaCungCapDichVu]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NhaCungCapDichVu);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
