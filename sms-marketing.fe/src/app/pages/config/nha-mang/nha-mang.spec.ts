import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NhaMang } from './nha-mang';


describe('NhaMang', () => {
  let component: NhaMang;
  let fixture: ComponentFixture<NhaMang>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NhaMang]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NhaMang);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
