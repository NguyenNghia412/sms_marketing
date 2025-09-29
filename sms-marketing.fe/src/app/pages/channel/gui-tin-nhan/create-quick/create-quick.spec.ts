import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CreateQuick } from './create-quick';

describe('CreateQuick', () => {
  let component: CreateQuick;
  let fixture: ComponentFixture<CreateQuick>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateQuick]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateQuick);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
