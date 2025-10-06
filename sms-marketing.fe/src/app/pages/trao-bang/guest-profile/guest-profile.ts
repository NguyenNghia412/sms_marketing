import { IViewGuestSvNhanBang } from '@/models/trao-bang/guest-sv-nhan-bang.models';
import { GuestSvNhanBangService } from '@/services/trao-bang/guest-sv-nhan-bang';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

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
  data: IViewGuestSvNhanBang = {}

  override ngOnInit(): void {
    this.mssv = this._route.snapshot.queryParamMap.get('mssv');
    this.getProfile();
  }

  getProfile() {
    this._guestService.getByMssv(this.mssv || '').subscribe({
      next: (res) => {
        if (this.isResponseSucceed(res)) {
          this.data = res.data
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
}
