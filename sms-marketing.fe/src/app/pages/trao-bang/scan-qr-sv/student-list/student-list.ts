import { SharedImports } from '@/shared/import.shared';
import { Component, input } from '@angular/core';
import { TableModule } from "primeng/table";

@Component({
  selector: 'app-student-list',
  imports: [SharedImports, TableModule],
  templateUrl: './student-list.html',
  styleUrl: './student-list.scss'
})
export class StudentList {
  students = input<any[]>();
}
