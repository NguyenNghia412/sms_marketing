import { IViewGuestSvNhanBang } from '@/models/trao-bang/guest-sv-nhan-bang.models';
import { GuestSvNhanBangService } from '@/services/trao-bang/guest-sv-nhan-bang';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

const STEPS = {
  PREV: -1,
  CURRENT: 0,
  NEXT: 1,
}

@Component({
  selector: 'app-guest-profile',
  imports: [SharedImports],
  templateUrl: './guest-profile.html',
  styleUrl: './guest-profile.scss'
})
export class GuestProfile extends BaseComponent {

  _guestService = inject(GuestSvNhanBangService);
  _route = inject(ActivatedRoute);

  mssv: string | null = null;
  step: number = STEPS.CURRENT;
  originSv: IViewGuestSvNhanBang = {}
  data: IViewGuestSvNhanBang = {}

  override ngOnInit(): void {
    this.mssv = this._route.snapshot.queryParamMap.get('mssv');
    this.getProfile();
  }

  get isShowPrevBtn() {
    return (this.step === STEPS.CURRENT && this.originSv.isShowPrev) || this.step === STEPS.NEXT
  }

  get isShowNextBtn() {
    return (this.step === STEPS.CURRENT && this.originSv.isShowNext) || this.step === STEPS.PREV
  }

  getProfile() {
    this._guestService.getByMssv(this.mssv || '').subscribe({
      next: (res) => {
        if (this.isResponseSucceed(res)) {
          this.data = { ...res.data }
          this.originSv = { ...res.data }
        }
      },
      error: (err) => {
        this.messageError(err);
      }
    });
  }

  downloadQr() {
    const imageUrl = this.data.linkQR || '';
    this._guestService.downloadQr(imageUrl);
  }

  onPrev() {
    if (this.step === STEPS.CURRENT) {
      this._guestService.getPrevByMssv(this.mssv || '').subscribe({
        next: (res) => {
          if (this.isResponseSucceed(res)) {
            this.data = { ...res.data }
            this.step = STEPS.PREV
          }
        },
        error: (err) => {
          this.messageError(err);
        }
      });
    } else if (this.step === STEPS.NEXT) {
      this.step = STEPS.CURRENT
      this.data = { ...this.originSv }
    }
  }

  onNext() {
    if (this.step === STEPS.CURRENT) {
      this._guestService.getNextByMssv(this.mssv || '').subscribe({
        next: (res) => {
          if (this.isResponseSucceed(res)) {
            this.data = { ...res.data }
            this.step = STEPS.NEXT
          }
        },
        error: (err) => {
          this.messageError(err);
        }
      });
    } else if (this.step === STEPS.PREV) {
      this.step = STEPS.CURRENT
      this.data = { ...this.originSv }
    }
  }
}
