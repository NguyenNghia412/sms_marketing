// app/shared-imports.ts

// 🔹 Angular common imports
import { CommonModule, NgClass } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

// 🔹 PrimeNG common imports
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { ToolbarModule } from 'primeng/toolbar';
import { ToastModule } from 'primeng/toast';
import { InputNumber } from 'primeng/inputnumber';
import { Textarea } from 'primeng/textarea';
import { CardModule } from 'primeng/card';
import { PasswordModule } from 'primeng/password';

export const SharedImports = [
  // Angular
  CommonModule,
  NgClass,
  FormsModule,
  ReactiveFormsModule,

  // PrimeNG
  ButtonModule,
  CardModule,
  InputTextModule,
  InputNumber,
  PasswordModule,
  Textarea,
  CheckboxModule,
  TableModule,
  DialogModule,
  ToolbarModule,
  ToastModule
];
