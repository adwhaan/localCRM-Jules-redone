import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DataService } from '../../core/services/data.service';
import { InteractionDto } from '../../core/models/crm.models';

@Component({
  selector: 'app-interaction-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="p-6">
      <div class="flex justify-between items-center mb-6">
        <h1 class="text-3xl font-bold">Interactions</h1>
        <button class="bg-blue-600 text-white px-4 py-2 rounded shadow hover:bg-blue-700 transition">New Interaction</button>
      </div>

      <div class="bg-white rounded-lg shadow overflow-hidden border border-gray-100">
        <table class="min-w-full table-auto">
          <thead>
            <tr class="bg-gray-50 text-left text-gray-500 text-xs uppercase font-semibold">
              <th class="p-4">Date</th>
              <th class="p-4">Subject</th>
              <th class="p-4">State</th>
              <th class="p-4">Type</th>
              <th class="p-4 text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let item of interactions" class="border-t border-gray-50 hover:bg-gray-50 transition">
              <td class="p-4 text-gray-500">{{ item.interactionDate | date:'mediumDate' }}</td>
              <td class="p-4 font-medium text-gray-800">{{ item.subject }}</td>
              <td class="p-4">
                <span [class]="item.state === 'Completed' ? 'text-green-600 bg-green-50' : 'text-amber-600 bg-amber-50'" class="px-2 py-1 rounded-full text-xs font-semibold">
                  {{ item.state }}
                </span>
              </td>
              <td class="p-4">
                <span class="text-gray-400 italic text-xs">{{ item.isTask ? 'Task' : 'Event' }}</span>
              </td>
              <td class="p-4 text-right">
                <button class="text-blue-600 mr-4 font-medium hover:underline">Edit</button>
                <button class="text-red-600 font-medium hover:underline" (click)="deleteItem(item.interactionId)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class InteractionListComponent {
  private dataService = inject(DataService);
  interactions: InteractionDto[] = [];

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.dataService.getInteractions().subscribe(data => this.interactions = data);
  }

  deleteItem(id: number) {
    if (confirm('Are you sure?')) {
      this.dataService.delete('interactions', id).subscribe(() => this.loadData());
    }
  }
}
