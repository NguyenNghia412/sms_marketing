import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component } from '@angular/core';

@Component({
  selector: 'app-chi-tiet',
  imports: [SharedImports],
  templateUrl: './chi-tiet.html',
  styleUrl: './chi-tiet.scss'
})
export class ChiTiet extends BaseComponent {

}
