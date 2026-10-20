import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { forkJoin, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Cita } from '../models/cita.model';
import { Paciente } from '../models/paciente.model';
import { Propietario } from '../models/catalogo.model';
import { Servicio } from '../models/servicio.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  obtenerEstadisticas() {
    return forkJoin({
      citas: this.http.get<Cita[]>(`${this.apiUrl}/Cita/todas`),
      pacientes: this.http.get<Paciente[]>(`${this.apiUrl}/Paciente/todos`),
      propietarios: this.http.get<Propietario[]>(`${this.apiUrl}/Propietario/todos`),
      servicios: this.http.get<Servicio[]>(`${this.apiUrl}/Servicio?soloActivos=true`)
    }).pipe(
      map(respuestas => {
        const hoy = new Date();
        const fechaHoy = this.obtenerFechaLocal(hoy);
        const citasHoy = respuestas.citas
          .filter(cita => {
            const fechaCita = new Date(cita.fechaHora);
            return !Number.isNaN(fechaCita.getTime()) &&
              this.obtenerFechaLocal(fechaCita) === fechaHoy;
          })
          .sort((a, b) => a.fechaHora.localeCompare(b.fechaHora));

        return {
          citasHoy: citasHoy.length,
          citasLista: citasHoy,
          pacientes: respuestas.pacientes.length,
          propietarios: respuestas.propietarios.length,
          servicios: respuestas.servicios.length
        };
      })
    );
  }

  private obtenerFechaLocal(fecha: Date): string {
    const año = fecha.getFullYear();
    const mes = String(fecha.getMonth() + 1).padStart(2, '0');
    const dia = String(fecha.getDate()).padStart(2, '0');
    return `${año}-${mes}-${dia}`;
  }
}
