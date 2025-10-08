import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SvInfo } from './sv-info';

describe('SvInfo', () => {
  let component: SvInfo;
  let fixture: ComponentFixture<SvInfo>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SvInfo]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SvInfo);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
