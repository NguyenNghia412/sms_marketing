import { IViewScanQrSubPlan } from '@/models/trao-bang/sv-nhan-bang.models';
import { SubPlanStatuses } from '@/shared/constants/sv-nhan-bang.constants';
import { SharedImports } from '@/shared/import.shared';
import { Component, input, output } from '@angular/core';
import { TagModule } from 'primeng/tag';

@Component({
  selector: 'app-left-sidebar',
  imports: [SharedImports, TagModule],
  templateUrl: './left-sidebar.html',
})
export class LeftSidebar {
  data = input.required<IViewScanQrSubPlan[]>();
  allowChangeSubPlan = input<boolean>();
  onChangeSubPlan = output<number | null | undefined>()

  subPlanStatuses = SubPlanStatuses

  onClickSubPlan(data: IViewScanQrSubPlan) {
    if (data.trangThai !== this.subPlanStatuses.DANG_TRAO_BANG) {
      this.onChangeSubPlan.emit(data.id)
    }
  }

}
