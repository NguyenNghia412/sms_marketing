import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DsDanhBa } from './ds-danh-ba';

describe('DsDanhBa', () => {
  let component: DsDanhBa;
  let fixture: ComponentFixture<DsDanhBa>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DsDanhBa]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DsDanhBa);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
