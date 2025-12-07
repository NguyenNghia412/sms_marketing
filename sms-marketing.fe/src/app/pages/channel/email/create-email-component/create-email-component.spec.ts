import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEmailComponent } from './create-email-component';

describe('CreateEmailComponent', () => {
  let component: CreateEmailComponent;
  let fixture: ComponentFixture<CreateEmailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateEmailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateEmailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
