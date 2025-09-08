import { Component } from '@angular/core';

@Component({
    standalone: true,
    selector: 'app-footer',
    template: `<div class="layout-footer">
        SMS MARKETING by
        <a href="https://nhaplieu.com" target="_blank" rel="noopener noreferrer" class="text-primary font-bold hover:underline">nhaplieu.com</a>
    </div>`
})
export class AppFooter {}
