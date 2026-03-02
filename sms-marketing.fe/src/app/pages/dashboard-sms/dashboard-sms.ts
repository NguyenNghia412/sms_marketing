import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChartModule } from 'primeng/chart';
import { ButtonModule } from 'primeng/button';
import { DatePickerModule } from 'primeng/datepicker';
import { TooltipModule } from 'primeng/tooltip';
import { debounceTime, Subscription } from 'rxjs';
import { LayoutService } from '../../layout/service/layout.service';
import { DashBoardService } from '@/services/dashboard.service';
import { IGetStatisticsDashBoard, IGetStatisticsTongSoTinNhanDaGuiTheoNam, IGetStatisticsUserCreditsTheoNam } from '@/models/dash-board.models';

@Component({
    standalone: true,
    selector: 'app-dashboard-sms',
    templateUrl: './dashboard-sms.html',
    styleUrls: ['./dashboard-sms.scss'],
    imports: [CommonModule, FormsModule, ChartModule, ButtonModule, DatePickerModule, TooltipModule]
})
export class DashboardSms implements OnInit, OnDestroy {
    private dashBoardService = inject(DashBoardService);
    private layoutService = inject(LayoutService);
    private subscription!: Subscription;

    stats: IGetStatisticsDashBoard = {
        tongSoLuongTinNhanGuiThanhCong: 0,
        tongSoUserSuDungDichVu: 0,
        tongSoChienDichGuiTinNhan: 0,
        tongSoNhaCungCapDichVu: 0
    };

    creditsTuNgay: Date = new Date(new Date().getFullYear(), 0, 1);
    creditsDenNgay: Date = new Date(new Date().getFullYear(), 11, 31);
    smsTuNgay: Date = new Date(new Date().getFullYear(), 0, 1);
    smsDenNgay: Date = new Date(new Date().getFullYear(), 11, 31);

    creditsChartData: any;
    creditsChartOptions: any;
    smsChartData: any;
    smsChartOptions: any;

    ngOnInit(): void {
        this.loadStats();
        this.loadCreditsChart();
        this.loadSmsChart();

        this.subscription = this.layoutService.configUpdate$
            .pipe(debounceTime(25))
            .subscribe(() => this.applyChartStyles());
    }

    onSearchCredits(): void {
        this.loadCreditsChart();
    }

    onSearchSms(): void {
        this.loadSmsChart();
    }

    private loadStats(): void {
        this.dashBoardService.getStatisticsDashBoard().subscribe({
            next: (res) => {
                if (res?.data) {
                    this.stats = res.data;
                }
            }
        });
    }

    private loadCreditsChart(): void {
        this.dashBoardService.getStatisticsUserCreditsTheoNam(this.creditsTuNgay, this.creditsDenNgay).subscribe({
            next: (res) => {
                this.buildCreditsChart(res?.data);
            }
        });
    }

    private loadSmsChart(): void {
        this.dashBoardService.getStatisticsTongSoTinNhanDaGuiTheoNam(this.smsTuNgay, this.smsDenNgay).subscribe({
            next: (res) => {
                this.buildSmsChart(res?.data);
            }
        });
    }

    private buildCreditsChart(data?: IGetStatisticsUserCreditsTheoNam): void {
        const documentStyle = getComputedStyle(document.documentElement);
        const labels = data?.listUserCredits?.map(item => item.user?.fullName || item.user?.userId || '') || [];
        const values = data?.listUserCredits?.map(item => Number(item.creditDaSuDung) || 0) || [];

        this.creditsChartData = {
            labels,
            datasets: [
                {
                    label: 'Credit đã sử dụng',
                    backgroundColor: documentStyle.getPropertyValue('--p-primary-400'),
                    data: values,
                    barThickness: 32,
                    borderRadius: { topLeft: 8, topRight: 8, bottomLeft: 0, bottomRight: 0 },
                    borderSkipped: false
                }
            ]
        };

        this.applyCreditsChartOptions();
    }

    private buildSmsChart(data?: IGetStatisticsTongSoTinNhanDaGuiTheoNam): void {
        const documentStyle = getComputedStyle(document.documentElement);
        const labels = data?.listThongKeTongSoTinNhanDaGui?.map(item => item.user?.fullName || item.user?.userId || '') || [];
        const thanhCong = data?.listThongKeTongSoTinNhanDaGui?.map(item => item.tongSoTinNhanDaGuiThanhCong || 0) || [];
        const thatBai = data?.listThongKeTongSoTinNhanDaGui?.map(item => item.tongSoTinNhanDaGuiThatBai || 0) || [];

        this.smsChartData = {
            labels,
            datasets: [
                {
                    type: 'bar',
                    label: 'Gửi thành công',
                    backgroundColor: documentStyle.getPropertyValue('--p-primary-400'),
                    data: thanhCong,
                    barThickness: 32
                },
                {
                    type: 'bar',
                    label: 'Gửi thất bại',
                    backgroundColor: '#ef4444',
                    data: thatBai,
                    barThickness: 32,
                    borderRadius: { topLeft: 8, topRight: 8, bottomLeft: 0, bottomRight: 0 },
                    borderSkipped: false
                }
            ]
        };

        this.applySmsChartOptions();
    }

    private applyChartStyles(): void {
        this.applyCreditsChartOptions();
        this.applySmsChartOptions();
    }

    private applyCreditsChartOptions(): void {
        const documentStyle = getComputedStyle(document.documentElement);
        const textColor = documentStyle.getPropertyValue('--text-color');
        const textMutedColor = documentStyle.getPropertyValue('--text-color-secondary');
        const borderColor = documentStyle.getPropertyValue('--surface-border');

        this.creditsChartOptions = {
            maintainAspectRatio: false,
            aspectRatio: 0.8,
            plugins: {
                legend: { labels: { color: textColor } }
            },
            scales: {
                x: {
                    ticks: { color: textMutedColor },
                    grid: { color: 'transparent', borderColor: 'transparent' }
                },
                y: {
                    ticks: { color: textMutedColor },
                    grid: { color: borderColor, borderColor: 'transparent', drawTicks: false }
                }
            }
        };
    }

    private applySmsChartOptions(): void {
        const documentStyle = getComputedStyle(document.documentElement);
        const textColor = documentStyle.getPropertyValue('--text-color');
        const textMutedColor = documentStyle.getPropertyValue('--text-color-secondary');
        const borderColor = documentStyle.getPropertyValue('--surface-border');

        this.smsChartOptions = {
            maintainAspectRatio: false,
            aspectRatio: 0.8,
            plugins: {
                legend: { labels: { color: textColor } }
            },
            scales: {
                x: {
                    stacked: true,
                    ticks: { color: textMutedColor },
                    grid: { color: 'transparent', borderColor: 'transparent' }
                },
                y: {
                    stacked: true,
                    ticks: { color: textMutedColor },
                    grid: { color: borderColor, borderColor: 'transparent', drawTicks: false }
                }
            }
        };
    }

    ngOnDestroy(): void {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
    }
}
