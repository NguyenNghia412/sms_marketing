import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SideScreen } from './side-screen';

describe('SideScreen', () => {
  let component: SideScreen;
  let fixture: ComponentFixture<SideScreen>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SideScreen]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SideScreen);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
