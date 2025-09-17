import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
    selector: 'app-create',
    imports: [SharedImports],
    templateUrl: './create.html',
    styleUrl: './create.scss'
})
export class Create extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);

    override form: FormGroup = new FormGroup({
        idBrandName: new FormControl('', [Validators.required]),
        idDanhBa: new FormControl('', [Validators.required]),
        tenChienDich: new FormControl('', [Validators.required]),
        ngayBatDau: new FormControl(new Date()),
        ngayKetThuc: new FormControl(new Date()),
        moTa: new FormControl(''),
        mauNoiDung: new FormControl('', [Validators.required]),
        isFlashSms: new FormControl(true)
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
      idBrandName: {
        required: 'Không được bỏ trống',
      },
      idDanhBa: {
        required: 'Không được bỏ trống',
      },
      tenChienDich: {
        required: 'Không được bỏ trống',
      },
      mauNoiDung: {
        required: 'Không được bỏ trống',
      },
    }

    onSubmit() {
      
    }
}
