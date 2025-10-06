import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject, output } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'app-dialog-mssv',
  imports: [SharedImports],
  templateUrl: './dialog-mssv.html',
  styleUrl: './dialog-mssv.scss'
})
export class DialogMssv extends BaseComponent {
  private _ref = inject(DynamicDialogRef);
  private _config = inject(DynamicDialogConfig);


  override form: FormGroup = new FormGroup({
    mssv: new FormControl('', [Validators.required]),
  });

  override ValidationMessages: Record<string, Record<string, string>> = {
    mssv: {
      required: 'Không được bỏ trống'
    },
  };

  onSubmit() {
    if (this.isFormInvalid()) {
      return;
    }

    this._ref?.close(this.form.value['mssv']);

  }
}
