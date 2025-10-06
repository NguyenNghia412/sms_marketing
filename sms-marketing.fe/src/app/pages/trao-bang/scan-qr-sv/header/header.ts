import { SharedImports } from '@/shared/import.shared';
import { Component, input } from '@angular/core';

@Component({
  selector: 'app-header',
  imports: [SharedImports],
  templateUrl: './header.html',
})
export class Header {
  tenKhoa = input.required<string>()
}
