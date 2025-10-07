import { IViewSvDangTraoBang } from '@/models/trao-bang/sv-nhan-bang.models';
import { TraoBangSvService } from '@/services/trao-bang/sv-nhan-bang.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { Component, inject } from '@angular/core';

@Component({
  selector: 'app-main-screen',
  imports: [],
  templateUrl: './main-screen.html',
  styleUrl: './main-screen.scss'
})
export class MainScreen extends BaseComponent {

  _svTraoBangService = inject(TraoBangSvService);
  svDangTrao: IViewSvDangTraoBang = {};

  override ngOnInit(): void {
    this.getSvDangTrao();
  }

  getSvDangTrao() {
    this._svTraoBangService.getSvDangTraoBang().subscribe({
      next: res => {
        if (this.isResponseSucceed(res)) {
          this.svDangTrao = res.data
        }
      }
    })
  }
}
