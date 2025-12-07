import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TblAction } from './tbl-action';

describe('TblAction', () => {
  let component: TblAction;
  let fixture: ComponentFixture<TblAction>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TblAction]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TblAction);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
