import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login.component').then(m => m.LoginComponent)
  },
  {
    path: '',
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'companies',
        loadComponent: () => import('./features/companies/company-list.component').then(m => m.CompanyListComponent)
      },
      {
        path: 'companies/:id',
        loadComponent: () => import('./features/companies/company-edit.component').then(m => m.CompanyEditComponent)
      },
      {
        path: 'contacts',
        loadComponent: () => import('./features/contacts/contact-list.component').then(m => m.ContactListComponent)
      },
      {
        path: 'interactions',
        loadComponent: () => import('./features/interactions/interaction-list.component').then(m => m.InteractionListComponent)
      },
      {
        path: 'engagements',
        loadComponent: () => import('./features/engagements/engagement-list.component').then(m => m.EngagementListComponent)
      },
      {
        path: 'notes',
        loadComponent: () => import('./features/notes/note-list.component').then(m => m.NoteListComponent)
      },
      {
        path: 'documents',
        loadComponent: () => import('./features/documents/document-list.component').then(m => m.DocumentListComponent)
      }
    ]
  }
];
