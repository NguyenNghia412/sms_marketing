import { IViewSvDangTraoBang } from '@/models/trao-bang/sv-nhan-bang.models';
import { ViewSvTypeConstants } from '@/shared/constants/sv-nhan-bang.constants';
import { SharedImports } from '@/shared/import.shared';
import { Component, input } from '@angular/core';

@Component({
  selector: 'app-sv-info',
  imports: [SharedImports],
  templateUrl: './sv-info.html',
  styleUrl: './sv-info.scss'
})
export class SvInfo {
  svDangTrao = input.required<IViewSvDangTraoBang | null>();
  type = ViewSvTypeConstants
}
