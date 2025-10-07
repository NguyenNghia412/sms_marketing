import { ComponentFixture, TestBed } from '@angular/core/testing';

import { McScreen } from './mc-screen';

describe('McScreen', () => {
  let component: McScreen;
  let fixture: ComponentFixture<McScreen>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [McScreen]
    })
    .compileComponents();

    fixture = TestBed.createComponent(McScreen);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
