import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HistoriaClinica } from '../models/historia-clinica.model';

@Injectable({
    providedIn: 'root'
})
export class HistoriaClinicaService {
    private http = inject(HttpClient);
    private apiUrl = 'https://localhost:7188/api/HistoriaClinica';

    obtenerPorPaciente(idPaciente: number): Observable<HistoriaClinica[]> {
        return this.http.get<HistoriaClinica[]>(`${this.apiUrl}/paciente/${idPaciente}`);
    }

    crear(request: HistoriaClinica): Observable<HistoriaClinica> {
        return this.http.post<HistoriaClinica>(this.apiUrl, request);
    }
}