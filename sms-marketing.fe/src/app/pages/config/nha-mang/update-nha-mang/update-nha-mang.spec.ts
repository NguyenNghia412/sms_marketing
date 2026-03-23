import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Create } from '@/pages/channel/sms/create/create';
import { UpdateNhaMang } from './update-nha-mang';


describe('UpdateNhaMang', () => {
  let component: UpdateNhaMang;
  let fixture: ComponentFixture<UpdateNhaMang>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateNhaMang]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UpdateNhaMang);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
