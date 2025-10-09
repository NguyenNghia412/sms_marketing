import { IViewScanQrSubPlan, IViewTienDoTraoBang } from '@/models/trao-bang/sv-nhan-bang.models';
import { TraoBangSvService } from '@/services/trao-bang/sv-nhan-bang.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SubPlanStatuses } from '@/shared/constants/sv-nhan-bang.constants';
import { SharedImports } from '@/shared/import.shared';
import { Component, effect, inject, input, output } from '@angular/core';
import { TagModule } from 'primeng/tag';
import { toObservable } from '@angular/core/rxjs-interop';


@Component({
  selector: 'app-left-sidebar',
  imports: [SharedImports, TagModule],
  templateUrl: './left-sidebar.html',
})
export class LeftSidebar extends BaseComponent {

  _svTraoBangService = inject(TraoBangSvService);

  data = input.required<IViewScanQrSubPlan[]>();
  allowChangeSubPlan = input<boolean>();
  onChangeSubPlan = output<number | null | undefined>()

  subPlanStatuses = SubPlanStatuses
  tienDo: IViewTienDoTraoBang = { tienDo: '' };

  constructor() {
    super();
    const value$ = toObservable(this.data); // 👈 Convert signal to Observable

    value$
      .subscribe(() =>
        this._svTraoBangService.getTienDoTraoBang().subscribe({
          next: (res) => {
            if (this.isResponseSucceed(res)) {
              this.tienDo = res.data;
            }
          }
        })
      );
  }

  onClickSubPlan(data: IViewScanQrSubPlan) {
    if (data.trangThai === this.subPlanStatuses.DA_TRAO_BANG) {
      this.confirmAction(
        {
          header: 'Thực hiện chuyển khoa',
          message: `Chắc chắn muốn chuyển khoa?`
        },
        () => {
          this.onChangeSubPlan.emit(data.id)
        }
      );

    }
  }

}
