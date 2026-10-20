import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Servicio, ServicioRequest } from '../models/servicio.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ServicioService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Servicio`;

  obtenerTodos(soloActivos: boolean = false): Observable<Servicio[]> {
    const params = new HttpParams().set('soloActivos', soloActivos.toString());
    return this.http.get<Servicio[]>(this.apiUrl, { params });
  }

  obtenerPorId(id: number): Observable<Servicio> {
    return this.http.get<Servicio>(`${this.apiUrl}/${id}`);
  }

  crear(servicio: ServicioRequest): Observable<Servicio> {
    return this.http.post<Servicio>(this.apiUrl, servicio);
  }

  actualizar(id: number, servicio: ServicioRequest): Observable<Servicio> {
    return this.http.put<Servicio>(`${this.apiUrl}/${id}`, servicio);
  }
}
