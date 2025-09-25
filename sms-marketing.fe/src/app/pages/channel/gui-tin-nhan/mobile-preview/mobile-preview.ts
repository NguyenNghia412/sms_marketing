import { SharedImports } from '@/shared/import.shared';
import { Component, input } from '@angular/core';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputGroupAddonModule } from 'primeng/inputgroupaddon';

@Component({
    selector: 'app-mobile-preview',
    imports: [SharedImports, InputGroupModule, InputGroupAddonModule],
    templateUrl: './mobile-preview.html',
    styleUrl: './mobile-preview.scss'
})
export class MobilePreview {
    message = input.required<string>();
}
