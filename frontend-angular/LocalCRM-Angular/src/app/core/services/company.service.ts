import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CompanyDto } from '../models/crm.models';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {
  private apiUrl = 'http://localhost:5000/api/companies';

  constructor(private http: HttpClient) { }

  getCompanies(): Observable<CompanyDto[]> {
    return this.http.get<CompanyDto[]>(this.apiUrl);
  }

  getCompany(id: number): Observable<CompanyDto> {
    return this.http.get<CompanyDto>(`${this.apiUrl}/${id}`);
  }

  createCompany(company: CompanyDto): Observable<CompanyDto> {
    return this.http.post<CompanyDto>(this.apiUrl, company);
  }

  updateCompany(id: number, company: CompanyDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, company);
  }

  deleteCompany(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
