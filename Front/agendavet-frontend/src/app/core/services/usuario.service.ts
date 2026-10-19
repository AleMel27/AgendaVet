import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Usuario,
  UsuarioCreateRequest,
  UsuarioUpdateRequest,
  Rol,
  PaginatedResponse
} from '../models/usuario.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UsuarioService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Usuario`;
  private rolUrl = `${environment.apiUrl}/Rol`;

  obtenerPaginado(
    page: number = 1,
    pageSize: number = 10,
    buscar?: string
  ): Observable<PaginatedResponse<Usuario>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (buscar && buscar.trim() !== '') {
      params = params.set('buscar', buscar.trim());
    }

    return this.http.get<PaginatedResponse<Usuario>>(this.apiUrl, { params });
  }

  obtenerTodos(): Observable<Usuario[]> {
    return this.http.get<Usuario[]>(`${this.apiUrl}/todos`);
  }

  obtenerPorId(id: number): Observable<Usuario> {
    return this.http.get<Usuario>(`${this.apiUrl}/${id}`);
  }

  crear(usuario: UsuarioCreateRequest): Observable<Usuario> {
    return this.http.post<Usuario>(this.apiUrl, usuario);
  }

  actualizar(id: number, usuario: UsuarioUpdateRequest): Observable<Usuario> {
    return this.http.put<Usuario>(`${this.apiUrl}/${id}`, usuario);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  obtenerRoles(): Observable<Rol[]> {
    return this.http.get<Rol[]>(this.rolUrl);
  }
}
