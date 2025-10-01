import { IVerifyImportDanhBa, IUploadFileImportDanhBa, IImportCreateDanhBa, IVerifyImportCreateDanhBa, IGetExcelInfor } from '@/models/danh-ba.models';
import { DanhBaService } from '@/services/danh-ba.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Popover } from 'primeng/popover';
import { Tooltip } from 'primeng/tooltip';

@Component({
    selector: 'app-chi-tiet-import',
    imports: [SharedImports, Popover,Tooltip],
    templateUrl: './chi-tiet.html',
    styleUrl: './chi-tiet.scss'
})
export class ChiTietImport extends BaseComponent implements OnInit {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);
    _danhBaService = inject(DanhBaService);

    loaiDanhBaOptions = [
        { label: 'Sms', value: 1 },
        { label: 'Email', value: 2 }
    ];

    sheetNameOptions: Array<{ label: string; value: string }> = [];

    override form: FormGroup = new FormGroup({
        Type: new FormControl(1, [Validators.required]),
        SheetName: new FormControl(null, [Validators.required]),
        IndexRowHeader: new FormControl(1, [Validators.required]),
        IndexRowStartImport: new FormControl(2, [Validators.required]),
        IndexColumnHoTen: new FormControl(null, [Validators.required]),
        IndexColumnSoDienThoai: new FormControl(null, [Validators.required])
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        Type: {
            required: 'Không được bỏ trống'
        },
        SheetName: {
            required: 'Không được bỏ trống'
        },
        IndexRowHeader: {
            required: 'Không được bỏ trống'
        },
        IndexRowStartImport: {
            required: 'Không được bỏ trống'
        },
        IndexColumnHoTen: {
            required: 'Không được bỏ trống'
        },
        IndexColumnSoDienThoai: {
            required: 'Không được bỏ trống'
        }
    };

    override ngOnInit(): void {
        this.loadExcelInfo();
    }

    loadExcelInfo() {
        const file = this._config.data?.file;
        if (!file) {
            this.messageError('Không tìm thấy file');
            return;
        }

        const body: IGetExcelInfor = {
            File: file
        };

        this.loading = true;
        this._danhBaService.getExcelInfor(body).subscribe({
            next: (value) => {
                if (this.isResponseSucceed(value)) {
                    this.sheetNameOptions = value.data.sheets.map(sheet => ({
                        label: sheet.sheetName,
                        value: sheet.sheetName
                    }));

                    if (this.sheetNameOptions.length > 0) {
                        this.form.patchValue({
                            SheetName: this.sheetNameOptions[0].value
                        });
                    }
                }
            },
            error: (err) => {
                this.messageError(err?.message);
            },
            complete: () => {
                this.loading = false;
            }
        });
    }

    onCancel() {
        this._ref.close(false);
    }

    onSubmit() {
        if (this.isFormInvalid()) {
            this.form.markAllAsTouched();
            return;
        }

        this.callApiVerify();
    }

    callApiVerify() {
        const body: IVerifyImportCreateDanhBa = {
            ...this.form.value,
            TenDanhBa: this._config.data?.tenDanhBa,
            File: this._config.data?.file
        };

        this.loading = true;
        this._danhBaService.verifyFileImportGuiTinNhan(body).subscribe({
            next: (value) => {
                if (this.isResponseSucceed(value)) {
                    this.confirmAction(
                        {
                            header: 'Tiếp tục import?',
                            message: `Số dòng import: ${value.data.totalRowsImported}; Số trường dữ liệu: ${value.data.totalDataImported}`
                        },
                        () => {
                            this.callApiImport();
                        }
                    );
                }
            },
            error: (err) => {
                this.messageError(err?.message);
            },
            complete: () => {
                this.loading = false;
            }
        });
    }

    callApiImport() {
        const body: IImportCreateDanhBa = {
            ...this.form.value,
            TenDanhBa: this._config.data?.tenDanhBa,
            File: this._config.data?.file
        };

        this.loading = true;
        this._danhBaService.uploadFileImportGuiTinNhan(body).subscribe({
            next: (value) => {
                if (this.isResponseSucceed(value)) {
                    this.messageSuccess('Import thành công');
                    this._ref.close(true);
                }
            },
            error: (err) => {
                this.messageError(err?.message);
            },
            complete: () => {
                this.loading = false;
            }
        });
    }
}