import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { AuthResponse } from '../models/crm.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5000/api/auth';
  private currentUserSubject = new BehaviorSubject<AuthResponse | null>(null);
  public currentUser = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) { }

  login(credentials: any): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap(user => {
        localStorage.setItem('accessToken', user.accessToken);
        this.currentUserSubject.next(user);
      })
    );
  }

  logout() {
    localStorage.removeItem('accessToken');
    this.currentUserSubject.next(null);
  }
}
