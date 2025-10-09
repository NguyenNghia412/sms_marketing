import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-header',
  imports: [SharedImports],
  templateUrl: './header.html',
})
export class Header extends BaseComponent {
  tenKhoa = input.required<string>()
  onNextSubPlan = output()

  onClickNext() {
    this.confirmAction(
        {
          header: 'Thực hiện chuyển khoa',
          message: `Chắc chắn muốn chuyển khoa?`
        },
        () => {
          this.onNextSubPlan.emit()
        }
      );
  }
}
