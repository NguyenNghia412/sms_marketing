import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubPlan } from './sub-plan';

describe('SubPlan', () => {
  let component: SubPlan;
  let fixture: ComponentFixture<SubPlan>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SubPlan]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SubPlan);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
