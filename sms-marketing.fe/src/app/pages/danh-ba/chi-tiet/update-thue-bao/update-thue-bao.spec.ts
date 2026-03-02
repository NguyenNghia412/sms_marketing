import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UpdateThueBao } from './update-thue-bao';





describe('UpdateThueBao', () => {
  let component: UpdateThueBao;
  let fixture: ComponentFixture<UpdateThueBao>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateThueBao]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UpdateThueBao);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
