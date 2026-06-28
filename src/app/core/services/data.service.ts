import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CompanyDto,
  ContactDto,
  InteractionDto,
  EngagementDto,
  NoteDto,
  DocumentDto,
  AuditLogDto
} from '../models/crm.models';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  private apiUrl = 'http://localhost:5000/api';
  private http = inject(HttpClient);

  getAll<T>(endpoint: string): Observable<T[]> {
    return this.http.get<T[]>(`${this.apiUrl}/${endpoint}`);
  }

  getById<T>(endpoint: string, id: number): Observable<T> {
    return this.http.get<T>(`${this.apiUrl}/${endpoint}/${id}`);
  }

  create<T>(endpoint: string, data: any): Observable<T> {
    return this.http.post<T>(`${this.apiUrl}/${endpoint}`, data);
  }

  update<T>(endpoint: string, id: number, data: any): Observable<T> {
    return this.http.put<T>(`${this.apiUrl}/${endpoint}/${id}`, data);
  }

  delete(endpoint: string, id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${endpoint}/${id}`);
  }

  getCompanies(): Observable<CompanyDto[]> { return this.getAll<CompanyDto>('companies'); }
  getContacts(): Observable<ContactDto[]> { return this.getAll<ContactDto>('contacts'); }
  getInteractions(): Observable<InteractionDto[]> { return this.getAll<InteractionDto>('interactions'); }
  getEngagements(): Observable<EngagementDto[]> { return this.getAll<EngagementDto>('engagements'); }
  getNotes(): Observable<NoteDto[]> { return this.getAll<NoteDto>('notes'); }
  getDocuments(): Observable<DocumentDto[]> { return this.getAll<DocumentDto>('documents'); }
  getAuditLogs(): Observable<AuditLogDto[]> { return this.getAll<AuditLogDto>('auditlogs'); }
}
