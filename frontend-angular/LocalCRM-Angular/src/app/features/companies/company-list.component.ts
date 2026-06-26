import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CompanyService } from '../../core/services/company.service';
import { CompanyDto } from '../../core/models/crm.models';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-company-list',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule],
  template: `
    <div class="p-6">
      <h1 class="text-2xl font-bold mb-4">Companies</h1>
      <table mat-table [dataSource]="companies" class="mat-elevation-z8 w-full">
        <ng-container matColumnDef="companyRef">
          <th mat-header-cell *matHeaderCellDef> Ref </th>
          <td mat-cell *matCellDef="let element"> {{element.companyRef}} </td>
        </ng-container>
        <ng-container matColumnDef="name">
          <th mat-header-cell *matHeaderCellDef> Name </th>
          <td mat-cell *matCellDef="let element"> {{element.name}} </td>
        </ng-container>
        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
      </table>
    </div>
  `
})
export class CompanyListComponent implements OnInit {
  companies: CompanyDto[] = [];
  displayedColumns: string[] = ['companyRef', 'name'];

  constructor(private companyService: CompanyService) {}

  ngOnInit(): void {
    this.companyService.getCompanies().subscribe(data => this.companies = data);
  }
}
