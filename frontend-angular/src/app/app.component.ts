import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div class="flex h-screen bg-gray-50 font-sans" *ngIf="authService.currentUser | async as user; else loginTpl">
      <!-- Sidebar -->
      <aside class="w-64 bg-white border-r border-gray-100 flex flex-col shadow-sm">
        <div class="p-8 flex items-center gap-3">
          <div class="w-8 h-8 bg-blue-600 rounded-lg shadow-blue-200 shadow-lg"></div>
          <span class="text-xl font-black text-gray-800 tracking-tight">LocalCRM</span>
        </div>

        <nav class="flex-1 px-4 space-y-1">
          <a routerLink="/dashboard" routerLinkActive="bg-blue-50 text-blue-600 font-bold" class="flex items-center px-4 py-3 text-gray-500 rounded-xl hover:bg-gray-50 hover:text-gray-900 transition">
             Dashboard
          </a>
          <a routerLink="/companies" routerLinkActive="bg-blue-50 text-blue-600 font-bold" class="flex items-center px-4 py-3 text-gray-500 rounded-xl hover:bg-gray-50 hover:text-gray-900 transition">
             Companies
          </a>
          <a routerLink="/contacts" routerLinkActive="bg-blue-50 text-blue-600 font-bold" class="flex items-center px-4 py-3 text-gray-500 rounded-xl hover:bg-gray-50 hover:text-gray-900 transition">
             Contacts
          </a>
          <a routerLink="/interactions" routerLinkActive="bg-blue-50 text-blue-600 font-bold" class="flex items-center px-4 py-3 text-gray-500 rounded-xl hover:bg-gray-50 hover:text-gray-900 transition">
             Interactions
          </a>
          <a routerLink="/engagements" routerLinkActive="bg-blue-50 text-blue-600 font-bold" class="flex items-center px-4 py-3 text-gray-500 rounded-xl hover:bg-gray-50 hover:text-gray-900 transition">
             Engagements
          </a>
          <a routerLink="/notes" routerLinkActive="bg-blue-50 text-blue-600 font-bold" class="flex items-center px-4 py-3 text-gray-500 rounded-xl hover:bg-gray-50 hover:text-gray-900 transition">
             Notes
          </a>
          <a routerLink="/documents" routerLinkActive="bg-blue-50 text-blue-600 font-bold" class="flex items-center px-4 py-3 text-gray-500 rounded-xl hover:bg-gray-50 hover:text-gray-900 transition">
             Documents
          </a>
        </nav>

        <div class="p-4 border-t border-gray-50">
           <div class="bg-gray-50 rounded-2xl p-4 flex items-center justify-between">
              <span class="text-sm font-medium text-gray-700 truncate mr-2">{{ user.username }}</span>
              <button (click)="logout()" class="text-xs text-red-500 font-bold uppercase hover:underline">Logout</button>
           </div>
        </div>
      </aside>

      <!-- Main Content -->
      <main class="flex-1 overflow-y-auto bg-gray-50/50">
        <router-outlet></router-outlet>
      </main>
    </div>

    <ng-template #loginTpl>
      <router-outlet></router-outlet>
    </ng-template>
  `
})
export class AppComponent {
  authService = inject(AuthService);

  logout() {
    this.authService.logout();
    window.location.reload();
  }
}
