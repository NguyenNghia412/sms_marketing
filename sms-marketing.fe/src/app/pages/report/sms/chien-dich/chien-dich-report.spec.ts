import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChienDichReport } from './chien-dich-report';

describe('ChienDichReport', () =>{
    let component: ChienDichReport;
    let fixture: ComponentFixture<ChienDichReport>;

     beforeEach(async () => {
        await TestBed.configureTestingModule({
          imports: [ChienDichReport]
        })
        .compileComponents();
    
        fixture = TestBed.createComponent(ChienDichReport);
        component = fixture.componentInstance;
        fixture.detectChanges();
      });
    
      it('should create', () => {
        expect(component).toBeTruthy();
      });
})