import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DataService } from '../../core/services/data.service';
import { ContactDto } from '../../core/models/crm.models';

@Component({
  selector: 'app-contact-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="p-6">
      <div class="flex justify-between items-center mb-6">
        <h1 class="text-3xl font-bold">Contacts</h1>
        <button class="bg-blue-600 text-white px-4 py-2 rounded shadow hover:bg-blue-700 transition">New Contact</button>
      </div>

      <div class="bg-white rounded-lg shadow overflow-hidden border border-gray-100">
        <table class="min-w-full table-auto">
          <thead>
            <tr class="bg-gray-50 text-left text-gray-500 text-xs uppercase font-semibold">
              <th class="p-4">Name</th>
              <th class="p-4">Email</th>
              <th class="p-4">Rating</th>
              <th class="p-4 text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let contact of contacts" class="border-t border-gray-50 hover:bg-gray-50 transition">
              <td class="p-4 font-medium text-gray-800">{{ contact.firstName }} {{ contact.lastName }}</td>
              <td class="p-4 text-gray-500">{{ contact.email }}</td>
              <td class="p-4 text-amber-400">★ {{ contact.rating }}</td>
              <td class="p-4 text-right">
                <button class="text-blue-600 mr-4 font-medium hover:underline">Edit</button>
                <button class="text-red-600 font-medium hover:underline" (click)="deleteContact(contact.contactId)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class ContactListComponent {
  private dataService = inject(DataService);
  contacts: ContactDto[] = [];

  ngOnInit() {
    this.loadContacts();
  }

  loadContacts() {
    this.dataService.getContacts().subscribe(data => this.contacts = data);
  }

  deleteContact(id: number) {
    if (confirm('Are you sure?')) {
      this.dataService.delete('contacts', id).subscribe(() => this.loadContacts());
    }
  }
}
