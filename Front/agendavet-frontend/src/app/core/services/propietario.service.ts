import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Propietario } from '../models/propietario.model';

@Injectable({
    providedIn: 'root'
})
export class PropietarioService {
    private http = inject(HttpClient);
    private apiUrl = 'https://localhost:7188/api/Propietario';

    getPropietarios(page: number = 1, pageSize: number = 10): Observable<any> {
        const params = new HttpParams()
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());
        return this.http.get<any>(this.apiUrl, { params });
    }

    crear(request: any): Observable<Propietario> {
        return this.http.post<Propietario>(this.apiUrl, request);
    }

    actualizar(id: number, request: any): Observable<Propietario> {
        return this.http.put<Propietario>(`${this.apiUrl}/${id}`, request);
    }

    eliminarPropietario(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}