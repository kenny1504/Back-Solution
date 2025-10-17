import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);
  private base = environment.apiUrl;

  clientes = 'clientes';
  cuentas = 'cuentas';
  movimientos = 'movimientos';
  reporte = 'reporte';

  getAll<T>(entity: string): Observable<T[]> {
    return this.http.get<T[]>(`${this.base}/${entity}`);
  }
  getById<T>(entity: string, id: number | string): Observable<T> {
    return this.http.get<T>(`${this.base}/${entity}/${id}`);
  }
  create<T>(entity: string, body: any): Observable<T> {
    return this.http.post<T>(`${this.base}/${entity}`, body);
  }
  update<T>(entity: string, id: number | string, body: any): Observable<T> {
    return this.http.put<T>(`${this.base}/${entity}/${id}`, body);
  }
  delete<T>(entity: string, id: number | string): Observable<T> {
    return this.http.delete<T>(`${this.base}/${entity}/${id}`);
  }
  getResumenMovimientos(params?: any): Observable<any> {
    return this.http.get(`${this.base}/${this.movimientos}/${this.reporte}`, { params });
  }
}
