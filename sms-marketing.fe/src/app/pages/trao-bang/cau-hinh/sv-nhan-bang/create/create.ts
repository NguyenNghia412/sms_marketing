import { IViewRowConfigSubPlan } from '@/models/trao-bang/sub-plan.models';
import { ICreateSvNhanBang, IUpdateSvNhanBang } from '@/models/trao-bang/sv-nhan-bang.models';
import { TraoBangSubPlanService } from '@/services/trao-bang/sub-plan.service';
import { TraoBangSvService } from '@/services/trao-bang/sv-nhan-bang.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SvNhanBangStatuses } from '@/shared/constants/sv-nhan-bang.constants';
import { SharedImports } from '@/shared/import.shared';
import { Utils } from '@/shared/utils';
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

    listSvTrangThai = SvNhanBangStatuses.List;
    listSubPlan: IViewRowConfigSubPlan[] = [];

    override form: FormGroup = new FormGroup({
        idSubPlan: new FormControl(null, [Validators.required]),
        hoVaTen: new FormControl('', [Validators.required]),
        maSoSinhVien: new FormControl('', [Validators.required]),
        email: new FormControl(''),
        emailSinhVien: new FormControl(''),
        lop: new FormControl(''),
        ngaySinh: new FormControl(new Date()),
        capBang: new FormControl(''),
        tenNganhDaoTao: new FormControl(''),
        xepHang: new FormControl(''),
        thanhTich: new FormControl(''),
        khoaQuanLy: new FormControl(''),
        soQuyetDinhTotNghiep: new FormControl(''),
        ngayQuyetDinh: new FormControl(new Date()),
        note: new FormControl(''),
        linkQR: new FormControl(''),
        trangThai: new FormControl(SvNhanBangStatuses.THAM_GIA_TRAO_BANG),
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
            console.log(this._config.data)
            const tmp = {...this._config.data};
            delete tmp.id;
            this.form.setValue({
                idSubPlan: this._config.data.idSubPlan,
                hoVaTen: this._config.data.hoVaTen,
                maSoSinhVien: this._config.data.maSoSinhVien,
                email: this._config.data.email,
                emailSinhVien: this._config.data.emailSinhVien,
                lop: this._config.data.lop,
                ngaySinh: new Date(this._config.data.ngaySinh),
                capBang: this._config.data.capBang,
                tenNganhDaoTao: this._config.data.tenNganhDaoTao,
                xepHang: this._config.data.xepHang,
                thanhTich: this._config.data.thanhTich,
                khoaQuanLy: this._config.data.khoaQuanLy,
                soQuyetDinhTotNghiep: this._config.data.soQuyetDinhTotNghiep,
                ngayQuyetDinh: new Date(this._config.data.ngayQuyetDinh),
                note: this._config.data.note,
                linkQR: this._config.data.linkQR,
                trangThai: this._config.data.trangThai,
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
            ngaySinh: Utils.formatDateCallApi(this.form.value['ngaySinh']),
            ngayQuyetDinh: Utils.formatDateCallApi(this.form.value['ngayQuyetDinh']),
            trangThai: SvNhanBangStatuses.THAM_GIA_TRAO_BANG
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
            ngaySinh: Utils.formatDateCallApi(this.form.value['ngaySinh']),
            ngayQuyetDinh: Utils.formatDateCallApi(this.form.value['ngayQuyetDinh']),
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
