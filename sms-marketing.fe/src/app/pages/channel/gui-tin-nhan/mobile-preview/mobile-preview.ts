import { Component, input } from '@angular/core';

@Component({
  selector: 'app-mobile-preview',
  imports: [],
  templateUrl: './mobile-preview.html',
  styleUrl: './mobile-preview.scss'
})
export class MobilePreview {
  message = input.required<string>();
}
