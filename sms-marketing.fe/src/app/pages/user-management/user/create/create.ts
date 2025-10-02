import { IViewPermission } from '@/models/permission.models';
import { ICreateRole, IUpdateRole, IViewRowRole } from '@/models/role.models';
import { ICreateUser, IUpdateUser } from '@/models/user.models';
import { PermissionService } from '@/services/permission.service';
import { RoleService } from '@/services/role.service';
import { UserService } from '@/services/user.service';
import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component, inject } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { CheckboxChangeEvent } from 'primeng/checkbox';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { PickListModule } from 'primeng/picklist';

@Component({
    selector: 'app-create',
    imports: [SharedImports, PickListModule],
    templateUrl: './create.html',
    styleUrl: './create.scss'
})
export class Create extends BaseComponent {
    private _ref = inject(DynamicDialogRef);
    private _config = inject(DynamicDialogConfig);
    private _userService = inject(UserService);
    private _roleService = inject(RoleService);

    listRole: IViewRowRole[] = [];
    isRandomPassword: boolean = true;

    override form: FormGroup = new FormGroup(
        {
            userName: new FormControl('', [Validators.required]),
            fullName: new FormControl('', [Validators.required]),
            email: new FormControl('', [Validators.required]),
            roleNames: new FormControl([], [Validators.required]),
            phoneNumber: new FormControl('', [Validators.required]),
            password: new FormControl('123456Aa@', [Validators.required]),
            msAccount: new FormControl('')
        }
    );

    override ValidationMessages: Record<string, Record<string, string>> = {
        name: {
            required: 'Không được bỏ trống'
        },
        fullName: {
            required: 'Không được bỏ trống'
        },
        email: {
            required: 'Không được bỏ trống'
        },
        phoneNumber: {
            required: 'Không được bỏ trống'
        },
        roleNames: {
            required: 'Không được bỏ trống'
        },
        password: {
            required: 'Không được bỏ trống'
        }
    };

    get isUpdate() {
        return this._config.data?.id;
    }
    

    override ngOnInit(): void {
        this._roleService.getList().subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res)) {
                    this.listRole = res.data;
                }
            }
        });
    }

    onChangeIsRandomPassword(event :  CheckboxChangeEvent) {
        if (event.checked) {
            this.form.patchValue({password: null});
        }
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
        const body: ICreateUser = {
            ...this.form.value
        };
        this.loading = true;
        this._userService.create(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Đã tạo tài khoản')) {
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
        const body: IUpdateUser = {
            id: this._config.data?.id,
            ...this.form.value
        };
        this.loading = true;
        this._userService.update(body).subscribe({
            next: (res) => {
                if (this.isResponseSucceed(res, true, 'Đã cập nhật')) {
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
