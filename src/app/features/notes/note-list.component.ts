import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DataService } from '../../core/services/data.service';
import { NoteDto } from '../../core/models/crm.models';

@Component({
  selector: 'app-note-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="p-6">
      <div class="flex justify-between items-center mb-6">
        <h1 class="text-3xl font-bold">Notes</h1>
        <button class="bg-blue-600 text-white px-4 py-2 rounded shadow hover:bg-blue-700">New Note</button>
      </div>

      <div class="bg-white rounded-lg shadow overflow-hidden">
        <table class="min-w-full table-auto">
          <thead>
            <tr class="bg-gray-100 text-left">
              <th class="p-3">Subject</th>
              <th class="p-3">Visibility</th>
              <th class="p-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let item of items" class="border-b hover:bg-gray-50">
              <td class="p-3">{{ item.subject }}</td>
              <td class="p-3">{{ item.visibility }}</td>
              <td class="p-3 text-right">
                <button class="text-blue-600 mr-4 font-medium">Edit</button>
                <button class="text-red-600 font-medium" (click)="deleteItem(item.noteId)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class NoteListComponent {
  private dataService = inject(DataService);
  items: NoteDto[] = [];

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.dataService.getNotes().subscribe(data => this.items = data);
  }

  deleteItem(id: number) {
    if (confirm('Are you sure?')) {
      this.dataService.delete('notes', id).subscribe(() => this.loadData());
    }
  }
}
