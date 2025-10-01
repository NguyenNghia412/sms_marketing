import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ChiTietImport } from './chi-tiet';



describe('Chi-tiet-import', () => {
  let component: ChiTietImport;
  let fixture: ComponentFixture<ChiTietImport>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChiTietImport]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChiTietImport);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
