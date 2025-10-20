import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AppMenuitem } from './app.menuitem';
import { IAppMenuItem } from '../model/app-menu-item.model';
import { SharedService } from '@/services/shared.service';
import { PermissionConstants } from '@/shared/constants/permission.constants';
import { ProfileStringeeService } from '@/services/profile-stringee';
import { IAccountProfileStringee, IResponseProfileStringee } from '@/models/profile-stringee.models';

@Component({
    selector: 'app-menu',
    standalone: true,
    imports: [CommonModule, AppMenuitem, RouterModule],
     template: `
    <div class="flex flex-col h-full">
        <ul class="layout-menu flex-1">
            <ng-container *ngFor="let item of model; let i = index">
                <li app-menuitem *ngIf="!item.separator" [item]="item" [index]="i" [root]="true"></li>
                <li *ngIf="item.separator" class="menu-separator"></li>
            </ng-container>
        </ul>
        <div *ngIf="profileData" class="px-4 py-3 mt-4 mb-4 bg-blue-50 dark:bg-blue-900/20 rounded-lg border border-blue-100 dark:border-blue-800">
            <div class="font-semibold text-sm mb-3 text-surface-900 dark:text-surface-0">Thông tin tài khoản</div>
            <div class="space-y-2">
                <div class="flex justify-between items-center text-sm">
                    <span class="text-surface-600 dark:text-surface-400">Tài khoản:</span>
                    <span class="font-medium text-surface-900 dark:text-surface-0">{{ profileData.data?.firstName }} {{ profileData.data?.lastName }}</span>
                </div>
                <div class="flex justify-between items-center text-sm">
                    <span class="text-surface-600 dark:text-surface-400">Email:</span>
                    <span class="font-medium text-surface-900 dark:text-surface-0">{{ profileData.data?.email }}</span>
                </div>
                <div class="flex justify-between items-center text-sm">
                    <span class="text-surface-600 dark:text-surface-400">SĐT:</span>
                    <span class="font-medium text-surface-900 dark:text-surface-0">{{ profileData.data?.countryNumber }} {{ profileData.data?.phoneNumber }}</span>
                </div>
                <div class="flex justify-between items-center text-sm pt-2 border-t border-surface-200 dark:border-surface-700">
                    <span class="text-surface-600 dark:text-surface-400">Số dư:</span>
                    <span class="font-semibold text-lg text-primary">{{ profileData.data?.amount | number:'1.0-0' }} VND</span>
                </div>
            </div>
        </div>
        <!--
        <div *ngIf="profileData" class="mx-4 mb-4 p-4 bg-blue-50 dark:bg-blue-900/20 rounded-xl border border-blue-100 dark:border-blue-800">
            <div class="text-blue-600 dark:text-blue-400 text-xs font-medium mb-2">Tin dụng SMS</div>
            <div class="text-blue-700 dark:text-blue-300 text-3xl font-bold mb-1">{{ profileData.data?.amount | number:'1.0-0' }}</div>
            <div class="text-blue-500 dark:text-blue-400 text-xs">tin nhắn còn lại</div>
        </div>
        -->
    </div>
    `
})
export class AppMenu {

    _sharedService = inject(SharedService);
    _profileStringeeService = inject(ProfileStringeeService);

    model: IAppMenuItem[] = [];
    profileData?: IResponseProfileStringee;
    loading = false;

    ngOnInit() {
        this.model = [
            // {
            //     items:[
            //         {
            //             label: 'Dashboard',
            //             routerLink: ['/']
            //         }
            //     ]
            // },
            {
                items: [
                    {
                        label: 'Kênh marketing',
                        visible: this._sharedService.isGranted(PermissionConstants.MenuMarketing),
                        expanded: true,
                        items: [
                            { label: 'SMS', heroIcon: 'heroChatBubbleBottomCenterText', routerLink: ['/channel/sms'], visible: this._sharedService.isGranted(PermissionConstants.MenuMarketingSms) },
                            { label: 'Email', heroIcon: 'heroEnvelope', routerLink: ['/channel/email'], visible: this._sharedService.isGranted(PermissionConstants.MenuMarketingEmail) },
                            { label: 'ZNS', heroIcon: 'heroSquare3Stack3d', routerLink: ['/channel/zns'], visible: this._sharedService.isGranted(PermissionConstants.MenuMarketingZns) }
                        ],
                    }
                ],
                visible: this._sharedService.isGranted(PermissionConstants.MenuMarketing),
            },
            {
                items: [
                    {
                        label: 'Danh bạ',
                        visible: this._sharedService.isGranted(PermissionConstants.MenuContact),
                        routerLink: ['/danh-ba/ds']
                    }
                ],
                visible: this._sharedService.isGranted(PermissionConstants.MenuContact),
            },
            {
                items: [
                    {
                        label: 'Templates',
                        visible: this._sharedService.isGranted(PermissionConstants.MenuTemplate),
                        routerLink: ['/template/mau-nd']
                    }
                ],
                visible: this._sharedService.isGranted(PermissionConstants.MenuTemplate),

            },
            {
                items: [
                    {
                        label: 'Thống kê',
                        visible: this._sharedService.isGranted(PermissionConstants.MenuReport),
                        items: [
                            { label: 'SMS', heroIcon: 'heroChatBubbleBottomCenterText', routerLink: ['/report/chien-dich-report'] },
                        ]
                    }
                ],
                visible: this._sharedService.isGranted(PermissionConstants.MenuReport),

            },
            {
                items: [
                    {
                        label: 'Trao bằng',
                        visible: this._sharedService.isGranted(PermissionConstants.MenuTraoBang),
                        items: [
                            {
                                label: 'Màn hình',
                                items: [
                                    {
                                        label: 'Sân khấu',
                                        routerLink: ['/guest/trao-bang/main-screen'],
                                        visible: true,
                                    },
                                    {
                                        label: 'Cánh gà',
                                        routerLink: ['/guest/trao-bang/side-screen'],
                                        visible: true,
                                    },
                                    {
                                        label: 'Điều khiển',
                                        routerLink: ['/trao-bang/mc-screen'],
                                        visible: this._sharedService.isGranted(PermissionConstants.MenuTraoBangMc),
                                    },
                                    {
                                        label: 'Checkin',
                                        routerLink: ['/trao-bang/scan-qr-sv'],
                                        visible: this._sharedService.isGranted(PermissionConstants.MenuTraoBangQuetQr),
                                    },
                                ]
                            },
                            {
                                label: 'Cấu hình',
                                visible: this._sharedService.isGranted(PermissionConstants.MenuTraoBangCauHinh),
                                items: [
                                    {
                                        label: 'Chương trình',
                                        visible: this._sharedService.isGranted(PermissionConstants.MenuTraoBangCauHinhChuongTrinh),
                                        routerLink: ['/trao-bang/config/plan']
                                    },
                                    {
                                        label: 'Khoa',
                                        visible: this._sharedService.isGranted(PermissionConstants.MenuTraoBangCauHinhKhoa),
                                        routerLink: ['/trao-bang/config/sub-plan']
                                    },
                                    {
                                        label: 'SV nhận bằng',
                                        visible: this._sharedService.isGranted(PermissionConstants.MenuTraoBangCauHinhSinhVienNhanBang),
                                        routerLink: ['/trao-bang/config/sv']
                                    },
                                ]
                            },
                        ]
                    }
                ],
                visible: this._sharedService.isGranted(PermissionConstants.MenuTraoBang),
            },
            {
                items: [
                    {
                        label: 'QL Tài khoản',
                        visible: this._sharedService.isGranted(PermissionConstants.MenuUserManagement),
                        items: [
                            {
                                label: 'Người dùng',
                                visible: this._sharedService.isGranted(PermissionConstants.MenuUserManagementUser),
                                heroIcon: 'heroUser',
                                routerLink: ['/user-management/user']
                            },
                            {
                                label: 'Vai trò',
                                visible: this._sharedService.isGranted(PermissionConstants.MenuUserManagementRole),
                                heroIcon: 'heroUserGroup',
                                routerLink: ['/user-management/role']
                            }
                        ]
                    }
                ],
                visible: this._sharedService.isGranted(PermissionConstants.MenuUserManagement),
            },
            // {
            //     items: [
            //         {
            //             label: 'MS Teams',
            //             items: [
            //                 { label: 'Họp trực tuyến', heroIcon: 'heroRadio', routerLink: ['/meeting/hop-truc-tuyen'] },
            //             ]
            //         }
            //     ]
            // },
            // {
            //     items: [
            //         {
            //             label: 'Tài khoản',
            //             routerLink: ['/contacts']
            //         }
            //     ]
            // },
            // {
            //     items: [
            //         {
            //             label: 'Audit log',
            //             routerLink: ['/contacts']
            //         }
            //     ]
            // },

        ];
        this.getData();
    }
    getData() {
        this.loading = true;
        this._profileStringeeService.getProfileStringee().subscribe({
            next: (res) => {
                if (res.status === 1 && res.data) {
                    this.profileData = res.data;
                }
            },
            complete: () => {
                this.loading = false;
            }
        });
    }
}