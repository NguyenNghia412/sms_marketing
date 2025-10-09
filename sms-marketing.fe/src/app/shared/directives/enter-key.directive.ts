import { Directive, EventEmitter, HostListener, Output } from '@angular/core';

@Directive({
  selector: '[appEnterKey]',
  standalone: true
})
export class EnterKeyDirective {
  @Output() appEnterKey = new EventEmitter<void>();

  @HostListener('keydown.enter', ['$event'])
  onEnter(event: KeyboardEvent) {
    event.preventDefault();
    this.appEnterKey.emit();
  }
}
