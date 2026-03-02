import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UpdateChiTietDuLieuThueBao } from './update-chi-tiet-du-lieu-thue-bao';






describe('UpdateChiTietDuLieuThueBao', () => {
  let component: UpdateChiTietDuLieuThueBao;
  let fixture: ComponentFixture<UpdateChiTietDuLieuThueBao>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateChiTietDuLieuThueBao]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UpdateChiTietDuLieuThueBao);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
