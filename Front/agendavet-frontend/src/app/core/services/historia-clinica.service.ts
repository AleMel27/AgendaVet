import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PaginatedResponse } from '../models/catalogo.model';

export interface HistoriaClinica {
  idHistoria?: number;
  idCita?: number | null;
  idPaciente: number;
  idVeterinario?: number;
  pacienteNombre?: string;
  veterinarioNombre?: string;
  fechaAtencion: string;
  pesoKg?: number | null;
  temperatura?: number | null;
  frecuenciaCardiaca?: number | null;
  diagnostico: string;
  tratamiento?: string | null;
  observaciones?: string | null;
}

export interface HistoriaClinicaCreateRequest {
  idCita?: number | null;
  idPaciente: number;
  idVeterinario: number;
  fechaAtencion?: string;
  pesoKg?: number | null;
  temperatura?: number | null;
  frecuenciaCardiaca?: number | null;
  diagnostico: string;
  tratamiento?: string | null;
  observaciones?: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class HistoriaClinicaService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/HistoriaClinica`;

  obtenerPorPaciente(idPaciente: number): Observable<HistoriaClinica[]> {
    return this.http.get<HistoriaClinica[]>(`${this.apiUrl}/paciente/${idPaciente}`);
  }

  obtenerPaginadoPorPaciente(idPaciente: number, page: number = 1, pageSize: number = 10): Observable<PaginatedResponse<HistoriaClinica>> {
    const params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);
    return this.http.get<PaginatedResponse<HistoriaClinica>>(
      `${this.apiUrl}/paciente/${idPaciente}/paginado`,
      { params }
    );
  }

  obtenerPorId(id: number): Observable<HistoriaClinica> {
    return this.http.get<HistoriaClinica>(`${this.apiUrl}/${id}`);
  }

  crear(request: HistoriaClinicaCreateRequest): Observable<HistoriaClinica> {
    return this.http.post<HistoriaClinica>(this.apiUrl, request);
  }
}