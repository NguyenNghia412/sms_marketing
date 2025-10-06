import { BaseComponent } from '@/shared/components/base/base-component';
import { SharedImports } from '@/shared/import.shared';
import { Component } from '@angular/core';
import { TableModule } from 'primeng/table';
import { LeftSidebar } from "./left-sidebar/left-sidebar";
import { Header } from "./header/header";
import { StudentList } from "./student-list/student-list";
import { Footer } from "./footer/footer";

@Component({
  selector: 'app-scan-qr-sv',
  imports: [SharedImports, TableModule, LeftSidebar, Header, StudentList, Footer],
  templateUrl: './scan-qr-sv.html',
  styleUrl: './scan-qr-sv.scss'
})
export class ScanQrSv extends BaseComponent {
  students = [
    { stt: 45, mssv: '2021600123', hoTen: 'Nguyễn Văn A', trangThai: 'Đã trao' },
    { stt: 46, mssv: '2021600124', hoTen: 'Trần Thị B', trangThai: 'Đang trao' },
    { stt: 47, mssv: '2021600125', hoTen: 'Lê Văn C', trangThai: 'Chuẩn bị' },
    { stt: 48, mssv: '2021600128', hoTen: 'Phạm Thị D', trangThai: 'Chuẩn bị' },
  ];
}
