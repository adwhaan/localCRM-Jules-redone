import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DataService } from '../../core/services/data.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="p-6">
      <h1 class="text-3xl font-bold mb-6 text-gray-800">Dashboard</h1>

      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div class="bg-white p-6 rounded-xl shadow-sm border border-gray-100 flex flex-col">
          <span class="text-gray-400 text-xs font-semibold uppercase tracking-wider">Companies</span>
          <span class="text-3xl font-bold text-blue-600 mt-2">{{ companyCount }}</span>
        </div>
        <div class="bg-white p-6 rounded-xl shadow-sm border border-gray-100 flex flex-col">
          <span class="text-gray-400 text-xs font-semibold uppercase tracking-wider">Contacts</span>
          <span class="text-3xl font-bold text-green-600 mt-2">{{ contactCount }}</span>
        </div>
        <div class="bg-white p-6 rounded-xl shadow-sm border border-gray-100 flex flex-col">
          <span class="text-gray-400 text-xs font-semibold uppercase tracking-wider">Interactions</span>
          <span class="text-3xl font-bold text-purple-600 mt-2">{{ interactionCount }}</span>
        </div>
        <div class="bg-white p-6 rounded-xl shadow-sm border border-gray-100 flex flex-col">
          <span class="text-gray-400 text-xs font-semibold uppercase tracking-wider">Engagements</span>
          <span class="text-3xl font-bold text-amber-600 mt-2">{{ engagementCount }}</span>
        </div>
      </div>

      <div class="bg-white p-6 rounded-xl shadow-sm border border-gray-100">
        <h2 class="text-xl font-bold mb-4 text-gray-800">Recent Activity</h2>
        <div class="overflow-x-auto">
          <table class="min-w-full table-auto">
            <thead>
              <tr class="text-left text-gray-400 text-xs uppercase font-semibold">
                <th class="pb-4">Action</th>
                <th class="pb-4">Entity</th>
                <th class="pb-4">User</th>
                <th class="pb-4">Date</th>
              </tr>
            </thead>
            <tbody class="text-sm text-gray-600">
              <tr *ngFor="let log of recentLogs" class="border-t border-gray-50">
                <td class="py-4">{{ log.actionType }}</td>
                <td class="py-4 font-medium text-gray-800">{{ log.entityName }}</td>
                <td class="py-4">{{ log.performedBy }}</td>
                <td class="py-4">{{ log.performedAt | date:'short' }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `
})
export class DashboardComponent {
  private dataService = inject(DataService);

  companyCount = 0;
  contactCount = 0;
  interactionCount = 0;
  engagementCount = 0;
  recentLogs: any[] = [];

  ngOnInit() {
    this.dataService.getCompanies().subscribe(data => this.companyCount = data.length);
    this.dataService.getContacts().subscribe(data => this.contactCount = data.length);
    this.dataService.getInteractions().subscribe(data => this.interactionCount = data.length);
    this.dataService.getEngagements().subscribe(data => this.engagementCount = data.length);
    this.dataService.getAuditLogs().subscribe(data => this.recentLogs = data.slice(0, 10));
  }
}
