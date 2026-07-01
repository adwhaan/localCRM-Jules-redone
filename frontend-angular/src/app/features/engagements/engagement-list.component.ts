import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DataService } from '../../core/services/data.service';
import { EngagementDto } from '../../core/models/crm.models';

@Component({
  selector: 'app-engagement-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="p-6">
      <div class="flex justify-between items-center mb-6">
        <h1 class="text-3xl font-bold">Engagements</h1>
        <button class="bg-blue-600 text-white px-4 py-2 rounded shadow hover:bg-blue-700 transition">New Engagement</button>
      </div>

      <div class="bg-white rounded-lg shadow overflow-hidden border border-gray-100">
        <table class="min-w-full table-auto">
          <thead>
            <tr class="bg-gray-50 text-left text-gray-500 text-xs uppercase font-semibold">
              <th class="p-4">Reference</th>
              <th class="p-4">Status</th>
              <th class="p-4 text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let item of items" class="border-t border-gray-50 hover:bg-gray-50 transition">
              <td class="p-4 font-medium text-gray-800">{{ item.engagementRef }}</td>
              <td class="p-4">
                <span [class]="item.engagementStatus === 'Open' ? 'text-blue-600 bg-blue-50' : 'text-gray-600 bg-gray-50'" class="px-2 py-1 rounded-full text-xs font-semibold">
                  {{ item.engagementStatus }}
                </span>
              </td>
              <td class="p-4 text-right">
                <button class="text-blue-600 mr-4 font-medium hover:underline">Edit</button>
                <button class="text-red-600 font-medium hover:underline" (click)="deleteItem(item.engagementId)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class EngagementListComponent {
  private dataService = inject(DataService);
  items: EngagementDto[] = [];

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.dataService.getEngagements().subscribe(data => this.items = data);
  }

  deleteItem(id: number) {
    if (confirm('Are you sure?')) {
      this.dataService.delete('engagements', id).subscribe(() => this.loadData());
    }
  }
}
