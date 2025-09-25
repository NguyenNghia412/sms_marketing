import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ChiTietChienDichReport } from './chi-tiet-report';




describe('ChiTietChienDichReport', () =>{
    let component: ChiTietChienDichReport;
    let fixture: ComponentFixture<ChiTietChienDichReport>;

     beforeEach(async () => {
        await TestBed.configureTestingModule({
          imports: [ChiTietChienDichReport]
        })
        .compileComponents();
    
        fixture = TestBed.createComponent(ChiTietChienDichReport);
        component = fixture.componentInstance;
        fixture.detectChanges();
      });
    
      it('should create', () => {
        expect(component).toBeTruthy();
      });
})