import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Propietario } from '../models/catalogo.model';
import { PaginatedResponse } from '../models/catalogo.model';

export interface PropietarioCreateRequest {
  nombre: string;
  apellido: string;
  dniCe: string;
  telefono: string;
  email?: string | null;
  direccion?: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class PropietarioService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Propietario`;

  obtenerTodos(): Observable<Propietario[]> {
    return this.http.get<Propietario[]>(`${this.apiUrl}/todos`);
  }

  obtenerPaginado(page: number = 1, pageSize: number = 10, buscar?: string): Observable<PaginatedResponse<Propietario>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (buscar && buscar.trim() !== '') {
      params = params.set('buscar', buscar.trim());
    }

    return this.http.get<PaginatedResponse<Propietario>>(this.apiUrl, { params });
  }

  obtenerPorId(id: number): Observable<Propietario> {
    return this.http.get<Propietario>(`${this.apiUrl}/${id}`);
  }

  obtenerPorDni(dniCe: string): Observable<Propietario> {
    return this.http.get<Propietario>(`${this.apiUrl}/dni/${dniCe}`);
  }

  crear(request: PropietarioCreateRequest): Observable<Propietario> {
    return this.http.post<Propietario>(this.apiUrl, request);
  }

  actualizar(id: number, request: PropietarioCreateRequest): Observable<Propietario> {
    return this.http.put<Propietario>(`${this.apiUrl}/${id}`, request);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}