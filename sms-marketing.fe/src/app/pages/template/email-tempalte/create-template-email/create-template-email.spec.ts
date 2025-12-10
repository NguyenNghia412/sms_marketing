import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateTemplateEmail } from './create-template-email';

describe('CreateTemplateEmail', () => {
  let component: CreateTemplateEmail;
  let fixture: ComponentFixture<CreateTemplateEmail>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateTemplateEmail]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateTemplateEmail);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
