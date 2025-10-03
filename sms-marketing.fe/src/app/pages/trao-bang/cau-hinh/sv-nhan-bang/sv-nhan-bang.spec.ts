import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SvNhanBang } from './sv-nhan-bang';

describe('SvNhanBang', () => {
  let component: SvNhanBang;
  let fixture: ComponentFixture<SvNhanBang>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SvNhanBang]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SvNhanBang);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
