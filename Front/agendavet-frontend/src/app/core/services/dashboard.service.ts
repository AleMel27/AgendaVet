import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { forkJoin, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7188/api';

  private obtenerLista(respuesta: any): any[] {
    if (Array.isArray(respuesta)) {
      return respuesta;
    }

    for (const propiedad of ['items', 'data', 'resultados', 'citas', 'pacientes', 'propietarios', 'servicios']) {
      if (Array.isArray(respuesta?.[propiedad])) {
        return respuesta[propiedad];
      }
    }

    return []
  }

  obtenerEstadisticas() {
    return forkJoin({
      citas: this.http.get<unknown>(`${this.apiUrl}/Cita/todas`),
      pacientes: this.http.get<unknown>(`${this.apiUrl}/Paciente/todos`),
      propietarios: this.http.get<unknown>(`${this.apiUrl}/Propietario/todos`),
      servicios: this.http.get<unknown>(`${this.apiUrl}/Servicio?soloActivos=true`)
    }).pipe(
      map(respuestas => {
        const citas = this.obtenerLista(respuestas.citas);
        const pacientes = this.obtenerLista(respuestas.pacientes);
        const propietarios = this.obtenerLista(respuestas.propietarios);
        const servicios = this.obtenerLista(respuestas.servicios);

        const hoy = new Date();
        const fechaHoy = `${hoy.getFullYear()}-${String(hoy.getMonth() + 1).padStart(2, '0')}-${String(hoy.getDate()).padStart(2, '0')}`;

        const citasHoy = citas.filter(cita => {
          const fecha = cita.fechaHora ?? cita.FechaHora;

          if (!fecha) {
            return false;
          }

          const fechaCita = new Date(fecha);

          if (Number.isNaN(fechaCita.getTime())) {
            return false;
          }

          const fechaLocal = `${fechaCita.getFullYear()}-${String(fechaCita.getMonth() + 1).padStart(2, '0')}-${String(fechaCita.getDate()).padStart(2, '0')}`;

          return fechaLocal === fechaHoy;
        })

        return {
          citasHoy: citasHoy.length,
          citasLista: citasHoy,
          pacientes: pacientes.length,
          propietarios: propietarios.length,
          servicios: servicios.length
        }
      })
    )
  }
}
