import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Paciente, PacienteCrearRequest } from '../models/paciente.model';
import { PaginatedResponse } from '../models/catalogo.model';

@Injectable({
  providedIn: 'root'
})
export class PacienteService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Paciente`;

  obtenerTodos(): Observable<Paciente[]> {
    return this.http.get<Paciente[]>(`${this.apiUrl}/todos`);
  }

  obtenerPaginado(page: number = 1, pageSize: number = 10, buscar?: string): Observable<PaginatedResponse<Paciente>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (buscar && buscar.trim() !== '') {
      params = params.set('buscar', buscar.trim());
    }

    return this.http.get<PaginatedResponse<Paciente>>(this.apiUrl, { params });
  }

  obtenerPorId(id: number): Observable<Paciente> {
    return this.http.get<Paciente>(`${this.apiUrl}/${id}`);
  }

  obtenerPorPropietario(idPropietario: number): Observable<Paciente[]> {
    return this.http.get<Paciente[]>(`${this.apiUrl}/propietario/${idPropietario}`);
  }

  crear(paciente: PacienteCrearRequest): Observable<Paciente> {
    return this.http.post<Paciente>(this.apiUrl, paciente);
  }

  actualizar(id: number, paciente: PacienteCrearRequest): Observable<Paciente> {
    return this.http.put<Paciente>(`${this.apiUrl}/${id}`, paciente);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
