import { IViewScanQrTienDoSv } from '@/models/trao-bang/sv-nhan-bang.models';
import { SvNhanBangStatuses } from '@/shared/constants/sv-nhan-bang.constants';
import { SharedImports } from '@/shared/import.shared';
import { Component, input } from '@angular/core';
import { TableModule } from "primeng/table";
import { TagModule } from 'primeng/tag';

@Component({
  selector: 'app-student-list',
  imports: [SharedImports, TableModule, TagModule],
  templateUrl: './student-list.html',
  styleUrl: './student-list.scss'
})
export class StudentList {
  students = input.required<IViewScanQrTienDoSv[]>();
  constStatuses = SvNhanBangStatuses;
}
