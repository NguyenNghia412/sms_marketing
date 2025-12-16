import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateTemplateSms } from './create-template-sms';

describe('CreateTemplateSms', () => {
  let component: CreateTemplateSms;
  let fixture: ComponentFixture<CreateTemplateSms>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateTemplateSms]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateTemplateSms);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
