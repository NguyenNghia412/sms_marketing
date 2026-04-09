import { Component, inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChartModule, UIChart } from 'primeng/chart';
import { ButtonModule } from 'primeng/button';
import { DatePickerModule } from 'primeng/datepicker';
import { TooltipModule } from 'primeng/tooltip';
import { SelectModule } from 'primeng/select';
import { Popover, PopoverModule } from 'primeng/popover';
import { debounceTime, Subscription } from 'rxjs';
import { LayoutService } from '../../../layout/service/layout.service';
import { DashBoardService } from '@/services/dashboard.service';
import { NhaCungCapDichVuService } from '@/services/nha-cung-cap-dich-vu.service';
import { IGetListDropDownUserNhaCungCapDichVuDto } from '@/models/nha-cung-cap-dich-vu.models';
import { IGetStatisticsDashBoard, IGetStatisticsTongSoTinNhanDaGuiTheoNam, IGetStatisticsUserCreditsTheoNam, IGetStatisticsUserCreditsTheoThangByUser } from '@/models/dash-board.models';

@Component({
    standalone: true,
    selector: 'app-dashboard-sms',
    templateUrl: './dashboard-sms.html',
    styleUrls: ['./dashboard-sms.scss'],
    imports: [CommonModule, FormsModule, ChartModule, ButtonModule, DatePickerModule, TooltipModule, SelectModule, PopoverModule]
})
export class DashboardSms implements OnInit, OnDestroy {
    @ViewChild('creditsChartRef') creditsChartRef!: UIChart;
    @ViewChild('smsChartRef') smsChartRef!: UIChart;
    @ViewChild('creditsByMonthChartRef') creditsByMonthChartRef!: UIChart;

    @ViewChild('creditsOp') creditsOp!: Popover;
    @ViewChild('smsOp') smsOp!: Popover;
    @ViewChild('creditsByMonthOp') creditsByMonthOp!: Popover;

    private dashBoardService = inject(DashBoardService);
    private layoutService = inject(LayoutService);
    private _nhaCungCapDichVuService = inject(NhaCungCapDichVuService);
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

    listUsers: IGetListDropDownUserNhaCungCapDichVuDto[] = [];
    selectedUserId: string = '';

    // Từ tháng mặc định T1 năm hiện tại, đến tháng mặc định = NOW
    creditsByMonthTuThang: Date = new Date(new Date().getFullYear(), 0, 1);
    creditsByMonthDenThang: Date = new Date();

    creditsByMonthChartData: any;
    creditsByMonthChartOptions: any;

    ngOnInit(): void {
        this.loadStats();
        this.loadCreditsChart();
        this.loadSmsChart();
        this.loadListUsers();

        this.subscription = this.layoutService.configUpdate$
            .pipe(debounceTime(25))
            .subscribe(() => this.applyChartStyles());
    }

    private formatDateForFilename(d: Date): string {
        const dd = String(d.getDate()).padStart(2, '0');
        const mm = String(d.getMonth() + 1).padStart(2, '0');
        const yyyy = d.getFullYear();
        return `${dd}-${mm}-${yyyy}`;
    }

    private formatDateLabel(d: Date): string {
        const dd = String(d.getDate()).padStart(2, '0');
        const mm = String(d.getMonth() + 1).padStart(2, '0');
        const yyyy = d.getFullYear();
        return `${dd}/${mm}/${yyyy}`;
    }

    private formatMonthForFilename(d: Date): string {
        const mm = String(d.getMonth() + 1).padStart(2, '0');
        const yyyy = d.getFullYear();
        return `T${mm}-${yyyy}`;
    }

    private formatMonthLabel(d: Date): string {
        const mm = String(d.getMonth() + 1).padStart(2, '0');
        const yyyy = d.getFullYear();
        return `Tháng ${mm}/${yyyy}`;
    }

    downloadChart(type: 'credits' | 'sms' | 'creditsByMonth'): void {
        const configMap: Record<string, { ref: UIChart; filename: string; title: string; subtitle: string }> = {
            credits: {
                ref: this.creditsChartRef,
                filename: `Thong-ke-Credits_${this.formatDateForFilename(this.creditsTuNgay)}_${this.formatDateForFilename(this.creditsDenNgay)}.png`,
                title: 'Thống kê Credits',
                subtitle: `Từ ngày ${this.formatDateLabel(this.creditsTuNgay)} đến ${this.formatDateLabel(this.creditsDenNgay)}`
            },
            sms: {
                ref: this.smsChartRef,
                filename: `Thong-ke-Tin-nhan_${this.formatDateForFilename(this.smsTuNgay)}_${this.formatDateForFilename(this.smsDenNgay)}.png`,
                title: 'Thống kê tin nhắn',
                subtitle: `Từ ngày ${this.formatDateLabel(this.smsTuNgay)} đến ${this.formatDateLabel(this.smsDenNgay)}`
            },
            creditsByMonth: {
                ref: this.creditsByMonthChartRef,
                filename: `Thong-ke-Credits-theo-thang_${this.formatMonthForFilename(this.creditsByMonthTuThang)}_${this.formatMonthForFilename(this.creditsByMonthDenThang)}.png`,
                title: 'Thống kê Credits theo tháng của từng người dùng',
                subtitle: `Từ ${this.formatMonthLabel(this.creditsByMonthTuThang)} đến ${this.formatMonthLabel(this.creditsByMonthDenThang)}`
            }
        };

        const config = configMap[type];
        if (!config?.ref?.chart) return;

        const originalCanvas = config.ref.chart.canvas as HTMLCanvasElement;

        // Scale 3x để ảnh sắc nét cao
        const SCALE = 3;
        const PADDING_H = 60;   // padding trái/phải
        const PADDING_TOP = 90; // vùng cho tiêu đề + subtitle
        const PADDING_BOT = 40; // padding dưới

        const exportCanvas = document.createElement('canvas');
        exportCanvas.width  = (originalCanvas.width  + PADDING_H * 2) * SCALE;
        exportCanvas.height = (originalCanvas.height + PADDING_TOP + PADDING_BOT) * SCALE;

        const ctx = exportCanvas.getContext('2d')!;
        ctx.scale(SCALE, SCALE);

        const W = exportCanvas.width  / SCALE;
        const H = exportCanvas.height / SCALE;

        // --- Nền trắng + border nhẹ ---
        ctx.fillStyle = '#ffffff';
        ctx.fillRect(0, 0, W, H);

        // Đường kẻ viền ngoài nhẹ
        ctx.strokeStyle = '#e2e8f0';
        ctx.lineWidth = 1;
        ctx.strokeRect(0.5, 0.5, W - 1, H - 1);

        // --- Tiêu đề chính ---
        ctx.fillStyle = '#0f172a';
        ctx.font = 'bold 18px "Segoe UI", Arial, sans-serif';
        ctx.textAlign = 'left';
        ctx.fillText(config.title, PADDING_H, 38);

        // --- Subtitle (khoảng thời gian) ---
        ctx.fillStyle = '#64748b';
        ctx.font = '13px "Segoe UI", Arial, sans-serif';
        ctx.fillText(config.subtitle, PADDING_H, 60);

        // --- Đường phân cách ---
        ctx.strokeStyle = '#e2e8f0';
        ctx.lineWidth = 1;
        ctx.beginPath();
        ctx.moveTo(PADDING_H, 72);
        ctx.lineTo(W - PADDING_H, 72);
        ctx.stroke();

        // --- Vẽ chart ---
        ctx.drawImage(originalCanvas, PADDING_H, PADDING_TOP, originalCanvas.width, originalCanvas.height);

        // --- Footer: ngày xuất ---
        const now = new Date();
        const exportedAt = `Xuất lúc: ${String(now.getHours()).padStart(2,'0')}:${String(now.getMinutes()).padStart(2,'0')} ngày ${String(now.getDate()).padStart(2,'0')}/${String(now.getMonth()+1).padStart(2,'0')}/${now.getFullYear()}`;
        ctx.fillStyle = '#94a3b8';
        ctx.font = '11px "Segoe UI", Arial, sans-serif';
        ctx.textAlign = 'right';
        ctx.fillText(exportedAt, W - PADDING_H, H - 14);

        const a = document.createElement('a');
        a.href = exportCanvas.toDataURL('image/png');
        a.download = config.filename;
        a.click();
    }

    copyChart(type: 'credits' | 'sms' | 'creditsByMonth'): void {
        const refMap: Record<string, UIChart> = {
            credits: this.creditsChartRef,
            sms: this.smsChartRef,
            creditsByMonth: this.creditsByMonthChartRef
        };

        const chartRef = refMap[type];
        if (!chartRef?.chart) return;

        const originalCanvas = chartRef.chart.canvas as HTMLCanvasElement;

        const SCALE = 3;
        const PADDING_H = 60;
        const PADDING_TOP = 90;
        const PADDING_BOT = 40;

        const exportCanvas = document.createElement('canvas');
        exportCanvas.width  = (originalCanvas.width  + PADDING_H * 2) * SCALE;
        exportCanvas.height = (originalCanvas.height + PADDING_TOP + PADDING_BOT) * SCALE;

        const ctx = exportCanvas.getContext('2d')!;
        ctx.scale(SCALE, SCALE);

        const W = exportCanvas.width  / SCALE;
        const H = exportCanvas.height / SCALE;

        ctx.fillStyle = '#ffffff';
        ctx.fillRect(0, 0, W, H);

        ctx.strokeStyle = '#e2e8f0';
        ctx.lineWidth = 1;
        ctx.strokeRect(0.5, 0.5, W - 1, H - 1);

        const configMap: Record<string, { title: string; subtitle: string }> = {
            /*credits: {
                title: 'Thống kê Credits',
                subtitle: `Từ ngày ${this.formatDateLabel(this.creditsTuNgay)} đến ${this.formatDateLabel(this.creditsDenNgay)}`
            },
            sms: {
                title: 'Thống kê tin nhắn',
                subtitle: `Từ ngày ${this.formatDateLabel(this.smsTuNgay)} đến ${this.formatDateLabel(this.smsDenNgay)}`
            },*/
            creditsByMonth: {
                title: 'Thống kê Credits theo tháng của từng người dùng',
                subtitle: `Từ ${this.formatMonthLabel(this.creditsByMonthTuThang)} đến ${this.formatMonthLabel(this.creditsByMonthDenThang)}`
            }
        };

        const config = configMap[type];

        ctx.fillStyle = '#0f172a';
        ctx.font = 'bold 18px "Segoe UI", Arial, sans-serif';
        ctx.textAlign = 'left';
        ctx.fillText(config.title, PADDING_H, 38);

        ctx.fillStyle = '#64748b';
        ctx.font = '13px "Segoe UI", Arial, sans-serif';
        ctx.fillText(config.subtitle, PADDING_H, 60);

        ctx.strokeStyle = '#e2e8f0';
        ctx.lineWidth = 1;
        ctx.beginPath();
        ctx.moveTo(PADDING_H, 72);
        ctx.lineTo(W - PADDING_H, 72);
        ctx.stroke();

        ctx.drawImage(originalCanvas, PADDING_H, PADDING_TOP, originalCanvas.width, originalCanvas.height);

        const now = new Date();
        const exportedAt = `Xuất lúc: ${String(now.getHours()).padStart(2,'0')}:${String(now.getMinutes()).padStart(2,'0')} ngày ${String(now.getDate()).padStart(2,'0')}/${String(now.getMonth()+1).padStart(2,'0')}/${now.getFullYear()}`;
        ctx.fillStyle = '#94a3b8';
        ctx.font = '11px "Segoe UI", Arial, sans-serif';
        ctx.textAlign = 'right';
        ctx.fillText(exportedAt, W - PADDING_H, H - 14);

        exportCanvas.toBlob((blob) => {
            if (!blob) return;
            try {
                navigator.clipboard.write([
                    new ClipboardItem({ 'image/png': blob })
                ]);
            } catch (e) {
                console.error('Copy ảnh thất bại:', e);
            }
        }, 'image/png');
    }

    onSearchCredits(): void {
        this.loadCreditsChart();
    }

    onSearchSms(): void {
        this.loadSmsChart();
    }

    onUserChanged(): void {
        if (this.selectedUserId) {
            this.loadCreditsByMonthChart();
        }
    }

    onSearchCreditsByMonth(): void {
        if (this.selectedUserId) {
            this.loadCreditsByMonthChart();
        }
    }

    private loadStats(): void {
        this.dashBoardService.getStatisticsDashBoard().subscribe({
            next: (res) => {
                if (res?.data) this.stats = res.data;
            }
        });
    }

    private loadCreditsChart(): void {
        this.dashBoardService.getStatisticsUserCreditsTheoNam(this.creditsTuNgay, this.creditsDenNgay).subscribe({
            next: (res) => { this.buildCreditsChart(res?.data); }
        });
    }

    private loadSmsChart(): void {
        this.dashBoardService.getStatisticsTongSoTinNhanDaGuiTheoNam(this.smsTuNgay, this.smsDenNgay).subscribe({
            next: (res) => { this.buildSmsChart(res?.data); }
        });
    }

    private loadListUsers(): void {
        this._nhaCungCapDichVuService.getListUserSuDungDichVu().subscribe({
            next: (res) => {
                if (res?.data) this.listUsers = res.data as any;
            }
        });
    }

    private loadCreditsByMonthChart(): void {
        this.dashBoardService.getStatisticsUserCreditsTheoThangByUser(
            this.selectedUserId,
            this.creditsByMonthTuThang,
            this.creditsByMonthDenThang
        ).subscribe({
            next: (res) => { this.buildCreditsByMonthChart(res?.data); }
        });
    }

    private buildCreditsChart(data?: IGetStatisticsUserCreditsTheoNam): void {
        const documentStyle = getComputedStyle(document.documentElement);
        const labels = data?.listUserCredits?.map(item => item.user?.fullName || item.user?.userId || '') || [];
        const values = data?.listUserCredits?.map(item => Number(item.creditDaSuDung) || 0) || [];

        this.creditsChartData = {
            labels,
            datasets: [{
                label: 'Credit đã sử dụng',
                backgroundColor: documentStyle.getPropertyValue('--p-primary-400'),
                data: values,
                barThickness: 32,
                borderRadius: { topLeft: 8, topRight: 8, bottomLeft: 0, bottomRight: 0 },
                borderSkipped: false
            }]
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

    private buildCreditsByMonthChart(data?: any): void {
        const documentStyle = getComputedStyle(document.documentElement);

        // Sinh labels động T{n}/{year} từ tuThang đến denThang — số cột không giới hạn 12
        const labels: string[] = [];
        let current = new Date(this.creditsByMonthTuThang.getFullYear(), this.creditsByMonthTuThang.getMonth(), 1);
        const end = new Date(this.creditsByMonthDenThang.getFullYear(), this.creditsByMonthDenThang.getMonth(), 1);

        while (current <= end) {
            labels.push(`T${current.getMonth() + 1}/${current.getFullYear()}`);
            current = new Date(current.getFullYear(), current.getMonth() + 1, 1);
        }

        const daSuDung = new Array(labels.length).fill(0);
        const chuaSuDung = new Array(labels.length).fill(0);
        const items = Array.isArray(data) ? data : data?.userCreditsTheoThangByUsers || [];

        items.forEach((item: any) => {
            const d = new Date(item.tuNgay);
            const label = `T${d.getMonth() + 1}/${d.getFullYear()}`;
            const idx = labels.indexOf(label);
            if (idx !== -1) {
                const used = Number(item.creditDaSuDung) || 0;
                const hanMuc = Number(item.hanMucCredit) || 0;
                daSuDung[idx] = used;
                chuaSuDung[idx] = Math.max(hanMuc - used, 0);
            }
        });

        this.creditsByMonthChartData = {
            labels,
            datasets: [
                {
                    label: 'Credits đã sử dụng',
                    backgroundColor: '#f59e0b',
                    data: daSuDung,
                    barThickness: 32,
                    borderSkipped: false
                },
                {
                    label: 'Credits chưa sử dụng',
                    backgroundColor: documentStyle.getPropertyValue('--p-primary-400'),
                    data: chuaSuDung,
                    barThickness: 32,
                    borderRadius: { topLeft: 8, topRight: 8, bottomLeft: 0, bottomRight: 0 },
                    borderSkipped: false
                }
            ]
        };
        this.applyCreditsByMonthChartOptions();
    }

    private applyChartStyles(): void {
        this.applyCreditsChartOptions();
        this.applySmsChartOptions();
        this.applyCreditsByMonthChartOptions();
    }

    private applyCreditsChartOptions(): void {
        const documentStyle = getComputedStyle(document.documentElement);
        const textColor = documentStyle.getPropertyValue('--text-color');
        const textMutedColor = documentStyle.getPropertyValue('--text-color-secondary');
        const borderColor = documentStyle.getPropertyValue('--surface-border');

        this.creditsChartOptions = {
            maintainAspectRatio: false,
            aspectRatio: 0.8,
            animation: { duration: 400 },
            plugins: {
                legend: {
                    labels: { color: textColor, font: { size: 13, weight: '500' }, boxWidth: 14, padding: 16 }
                },
                tooltip: {
                    backgroundColor: '#1e293b', titleColor: '#f1f5f9', bodyColor: '#cbd5e1',
                    padding: 10, cornerRadius: 8,
                    callbacks: { label: (ctx: any) => ` ${ctx.dataset.label}: ${Number(ctx.raw).toLocaleString('vi-VN')}` }
                }
            },
            scales: {
                x: {
                    ticks: { color: textMutedColor, font: { size: 12 } },
                    grid: { color: 'transparent', borderColor: 'transparent' },
                    border: { color: borderColor }
                },
                y: {
                    ticks: { color: textMutedColor, font: { size: 12 }, callback: (v: any) => Number(v).toLocaleString('vi-VN') },
                    grid: { color: borderColor, borderColor: 'transparent', drawTicks: false },
                    border: { dash: [4, 4] }
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
            animation: { duration: 400 },
            plugins: {
                legend: {
                    labels: { color: textColor, font: { size: 13, weight: '500' }, boxWidth: 14, padding: 16 }
                },
                tooltip: {
                    backgroundColor: '#1e293b', titleColor: '#f1f5f9', bodyColor: '#cbd5e1',
                    padding: 10, cornerRadius: 8,
                    callbacks: { label: (ctx: any) => ` ${ctx.dataset.label}: ${Number(ctx.raw).toLocaleString('vi-VN')}` }
                }
            },
            scales: {
                x: {
                    stacked: true,
                    ticks: { color: textMutedColor, font: { size: 12 } },
                    grid: { color: 'transparent', borderColor: 'transparent' },
                    border: { color: borderColor }
                },
                y: {
                    stacked: true,
                    ticks: { color: textMutedColor, font: { size: 12 }, callback: (v: any) => Number(v).toLocaleString('vi-VN') },
                    grid: { color: borderColor, borderColor: 'transparent', drawTicks: false },
                    border: { dash: [4, 4] }
                }
            }
        };
    }

    private applyCreditsByMonthChartOptions(): void {
        const documentStyle = getComputedStyle(document.documentElement);
        const textColor = documentStyle.getPropertyValue('--text-color');
        const textMutedColor = documentStyle.getPropertyValue('--text-color-secondary');
        const borderColor = documentStyle.getPropertyValue('--surface-border');

        this.creditsByMonthChartOptions = {
            maintainAspectRatio: false,
            aspectRatio: 0.8,
            animation: { duration: 400 },
            plugins: {
                legend: {
                    labels: { color: textColor, font: { size: 13, weight: '500' }, boxWidth: 14, padding: 16 }
                },
                tooltip: {
                    backgroundColor: '#1e293b', titleColor: '#f1f5f9', bodyColor: '#cbd5e1',
                    padding: 10, cornerRadius: 8,
                    callbacks: {
                        label: (ctx: any) => ` ${ctx.dataset.label}: ${Number(ctx.raw).toLocaleString('vi-VN')}`,
                        afterBody: (context: any) => {
                            const idx = context[0].dataIndex;
                            const datasets = context[0].chart.data.datasets;
                            const total = datasets.reduce((sum: number, ds: any) => sum + (ds.data[idx] || 0), 0);
                            return `Hạn mức: ${total.toLocaleString('vi-VN')}`;
                        }
                    }
                }
            },
            scales: {
                x: {
                    stacked: true,
                    ticks: { color: textMutedColor, font: { size: 12 } },
                    grid: { color: 'transparent', borderColor: 'transparent' },
                    border: { color: borderColor }
                },
                y: {
                    stacked: true,
                    ticks: { color: textMutedColor, font: { size: 12 }, callback: (v: any) => Number(v).toLocaleString('vi-VN') },
                    grid: { color: borderColor, borderColor: 'transparent', drawTicks: false },
                    border: { dash: [4, 4] }
                }
            }
        };
    }

    ngOnDestroy(): void {
        if (this.subscription) this.subscription.unsubscribe();
    }
}