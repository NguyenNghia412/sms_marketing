import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CreateNhaMang } from './create-nha-mang';
import { Create } from '@/pages/channel/sms/create/create';


describe('CreateNhaMang', () => {
  let component: CreateNhaMang;
  let fixture: ComponentFixture<CreateNhaMang>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateNhaMang]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateNhaMang);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
