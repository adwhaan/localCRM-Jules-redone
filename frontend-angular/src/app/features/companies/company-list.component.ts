import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DataService } from '../../core/services/data.service';
import { CompanyDto } from '../../core/models/crm.models';

@Component({
  selector: 'app-company-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="p-6">
      <div class="flex justify-between items-center mb-6">
        <h1 class="text-3xl font-bold">Companies</h1>
        <button routerLink="/companies/new" class="bg-blue-600 text-white px-4 py-2 rounded shadow hover:bg-blue-700">New Company</button>
      </div>

      <div class="bg-white rounded-lg shadow overflow-hidden border border-gray-100">
        <table class="min-w-full table-auto">
          <thead>
            <tr class="bg-gray-50 text-left text-gray-500 text-xs uppercase font-semibold">
              <th class="p-4">Reference</th>
              <th class="p-4">Name</th>
              <th class="p-4">City</th>
              <th class="p-4">Type</th>
              <th class="p-4">Rating</th>
              <th class="p-4 text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let company of companies" class="border-t border-gray-50 hover:bg-gray-50 transition">
              <td class="p-4 font-medium text-gray-800">{{ company.companyRef }}</td>
              <td class="p-4">{{ company.name }}</td>
              <td class="p-4 text-gray-500">{{ company.city }}</td>
              <td class="p-4">
                <span class="px-2 py-1 text-xs rounded-full bg-blue-50 text-blue-600">{{ company.companyType }}</span>
              </td>
              <td class="p-4">
                <div class="flex text-amber-400">
                  <span *ngFor="let i of [1,2,3,4,5]">★</span>
                </div>
              </td>
              <td class="p-4 text-right">
                <button [routerLink]="['/companies', company.companyId]" class="text-blue-600 mr-4 font-medium hover:underline">Edit</button>
                <button class="text-red-600 font-medium hover:underline" (click)="deleteCompany(company.companyId)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class CompanyListComponent {
  private dataService = inject(DataService);
  companies: CompanyDto[] = [];

  ngOnInit() {
    this.loadCompanies();
  }

  loadCompanies() {
    this.dataService.getCompanies().subscribe(data => this.companies = data);
  }

  deleteCompany(id: number) {
    if (confirm('Are you sure?')) {
      this.dataService.delete('companies', id).subscribe(() => this.loadCompanies());
    }
  }
}
