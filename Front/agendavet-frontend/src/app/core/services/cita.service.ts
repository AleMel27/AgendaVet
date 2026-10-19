import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Cita, CitaCrearRequest, CitaActualizarRequest } from '../models/cita.model';
import { PaginatedResponse } from '../models/catalogo.model';

@Injectable({
  providedIn: 'root'
})
export class CitaService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Cita`;

  obtenerTodas(): Observable<Cita[]> {
    return this.http.get<Cita[]>(`${this.apiUrl}/todas`);
  }

  obtenerPaginado(
    page: number = 1,
    pageSize: number = 10,
    idVeterinario?: number,
    estado?: string
  ): Observable<PaginatedResponse<Cita>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (idVeterinario) {
      params = params.set('idVeterinario', idVeterinario.toString());
    }

    if (estado && estado.trim() !== '') {
      params = params.set('estado', estado.trim());
    }

    return this.http.get<PaginatedResponse<Cita>>(this.apiUrl, { params });
  }

  obtenerPorId(id: number): Observable<Cita> {
    return this.http.get<Cita>(`${this.apiUrl}/${id}`);
  }

  crear(cita: CitaCrearRequest): Observable<Cita> {
    return this.http.post<Cita>(this.apiUrl, cita);
  }

  actualizar(id: number, cita: CitaActualizarRequest): Observable<Cita> {
    return this.http.put<Cita>(`${this.apiUrl}/${id}`, cita);
  }

  cambiarEstado(id: number, nuevoEstado: string): Observable<void> {
    const params = new HttpParams().set('nuevoEstado', nuevoEstado);
    return this.http.patch<void>(`${this.apiUrl}/${id}/estado`, null, { params });
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
