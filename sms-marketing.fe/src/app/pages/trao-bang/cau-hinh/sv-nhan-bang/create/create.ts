import { IViewRowConfigSubPlan } from '@/models/trao-bang/sub-plan.models';
import { ICreateSvNhanBang, IUpdateSvNhanBang } from '@/models/trao-bang/sv-nhan-bang.models';
import { TraoBangSubPlanService } from '@/services/trao-bang/sub-plan.service';
import { TraoBangSvService } from '@/services/trao-bang/sv-nhan-bang.service';
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
    private _svNhanBangService = inject(TraoBangSvService);
    private _subPlanService = inject(TraoBangSubPlanService);

    listSubPlan: IViewRowConfigSubPlan[] = [];

    override form: FormGroup = new FormGroup({
        idSubPlan: new FormControl(null, [Validators.required]),
        hoVaTen: new FormControl('', [Validators.required]),
        maSoSinhVien: new FormControl('', [Validators.required]),
        email: new FormControl(''),
        emailSinhVien: new FormControl(''),
        lop: new FormControl(''),
        ngaySinh: new FormControl(''),
        capBang: new FormControl(''),
        tenNganhDaoTao: new FormControl(''),
        xepHang: new FormControl(''),
        thanhTich: new FormControl(''),
        khoaQuanLy: new FormControl(''),
        soQuyetDinhTotNghiep: new FormControl(''),
        ngayQuyetDinh: new FormControl(''),
        note: new FormControl(''),
        linkQR: new FormControl(''),
        trangThai: new FormControl(''),
    });

    override ValidationMessages: Record<string, Record<string, string>> = {
        idSubPlan: {
            required: 'Không được bỏ trống'
        },
        hoVaTen: {
            required: 'Không được bỏ trống'
        },
        maSoSinhVien: {
            required: 'Không được bỏ trống'
        }
    };

    override ngOnInit(): void {
        this.getListSubPlan();
        if (this.isUpdate) {
            this.form.setValue({
                ...this._config.data
            });
        }
    }

    get isUpdate() {
        return this._config.data?.id;
    }

    getListSubPlan() {
        this._subPlanService.getList(1).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res)) {
                    this.listSubPlan = res.data
                }
            }
        })
    }

    onSubmit() {
        if (this.isFormInvalid()) {
            return;
        }

       if (this.isUpdate) {
        this.onSubmitUpdate();
       } else {
        this.onSubmitCreate();
       }
    }

    onSubmitCreate() {
        const body: ICreateSvNhanBang = {
            ...this.form.value,
        };
        this.loading = true;
        this._svNhanBangService.create(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Đã thêm sv')) {
                    this._ref?.close(true);
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

    onSubmitUpdate() {
        const body: IUpdateSvNhanBang = {
            idSubPlan: this._config.data.id,
            ...this.form.value,
        };
        this.loading = true;
        this._svNhanBangService.update(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Đã lưu')) {
                    this._ref?.close(true);
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
