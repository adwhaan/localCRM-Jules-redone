import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="min-h-screen flex items-center justify-center bg-gray-50 px-4">
      <div class="bg-white p-10 rounded-2xl shadow-xl max-w-md w-full border border-gray-100">
        <div class="text-center mb-10">
          <h1 class="text-4xl font-extrabold text-blue-600">LocalCRM</h1>
          <p class="text-gray-400 mt-2 text-sm">Welcome back, please sign in</p>
        </div>

        <form (ngSubmit)="onSubmit()" #loginForm="ngForm" class="space-y-6">
          <div>
            <label class="block text-gray-700 text-xs font-bold uppercase tracking-wider mb-2" for="username">Username</label>
            <input
              class="w-full px-4 py-3 rounded-lg border border-gray-200 focus:ring-2 focus:ring-blue-500 focus:border-transparent outline-none transition"
              id="username" type="text" name="username" [(ngModel)]="credentials.username" required>
          </div>
          <div>
            <label class="block text-gray-700 text-xs font-bold uppercase tracking-wider mb-2" for="password">Password</label>
            <input
              class="w-full px-4 py-3 rounded-lg border border-gray-200 focus:ring-2 focus:ring-blue-500 focus:border-transparent outline-none transition"
              id="password" type="password" name="password" [(ngModel)]="credentials.password" required>
          </div>

          <div *ngIf="error" class="bg-red-50 text-red-500 text-sm p-3 rounded-lg border border-red-100">
            {{ error }}
          </div>

          <button
            class="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 rounded-xl shadow-lg shadow-blue-200 transition-all transform active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed"
            type="submit" [disabled]="!loginForm.form.valid || loading">
            <span *ngIf="loading">Signing in...</span>
            <span *ngIf="!loading">Sign In</span>
          </button>
        </form>
      </div>
    </div>
  `
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  credentials = { username: '', password: '' };
  loading = false;
  error = '';

  onSubmit() {
    this.loading = true;
    this.error = '';
    this.authService.login(this.credentials).subscribe({
      next: () => {
        this.router.navigate(['/']);
      },
      error: err => {
        this.error = 'Invalid credentials. Please try again.';
        this.loading = false;
      }
    });
  }
}
