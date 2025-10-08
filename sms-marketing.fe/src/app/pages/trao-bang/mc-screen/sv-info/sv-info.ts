import { IViewSvDangTraoBang } from '@/models/trao-bang/sv-nhan-bang.models';
import { Component, input } from '@angular/core';

@Component({
  selector: 'app-sv-info',
  imports: [],
  templateUrl: './sv-info.html',
  styleUrl: './sv-info.scss'
})
export class SvInfo {
  svDangTrao = input.required<IViewSvDangTraoBang | null>();

}
