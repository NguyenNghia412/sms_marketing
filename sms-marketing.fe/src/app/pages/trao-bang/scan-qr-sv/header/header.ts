import { SharedImports } from '@/shared/import.shared';
import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-header',
  imports: [SharedImports],
  templateUrl: './header.html',
})
export class Header {
  tenKhoa = input.required<string>()
  onNextSubPlan = output()

  onClickNext() {
    this.onNextSubPlan.emit()
  }
}
