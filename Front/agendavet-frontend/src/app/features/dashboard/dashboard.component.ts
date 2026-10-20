import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DashboardService } from '../../core/services/dashboard.service';
import { Cita } from '../../core/models/cita.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  private dashboardService = inject(DashboardService);
  private cdr = inject(ChangeDetectorRef);

  estadisticas = {
    citasHoy: 0,
    pacientes: 0,
    propietarios: 0,
    servicios: 0
  };

  citasHoy: Cita[] = [];
  paginaCitasHoy = 1;
  readonly citasPorPagina = 10;

  cargando = true;
  errorMensaje = '';

  ngOnInit(): void {
    this.dashboardService.obtenerEstadisticas().subscribe({
      next: (datos) => {

        this.estadisticas = {
          citasHoy: datos.citasHoy,
          pacientes: datos.pacientes,
          propietarios: datos.propietarios,
          servicios: datos.servicios
        };

        this.citasHoy = datos.citasLista;
        this.paginaCitasHoy = 1;

        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error al cargar estadísticas:', error);
        this.errorMensaje = 'No se pudieron cargar las estadísticas. Verifica que el backend esté activo.';
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  obtenerHora(fecha: string): string {
    if (!fecha) {
      return '--:--';
    }

    const fechaCita = new Date(fecha);

    return fechaCita.toLocaleTimeString('es-PE', {
      hour: '2-digit',
      minute: '2-digit',
      hour12: false
    });
  }

  get citasHoyPaginadas(): Cita[] {
    const inicio = (this.paginaCitasHoy - 1) * this.citasPorPagina;
    return this.citasHoy.slice(inicio, inicio + this.citasPorPagina);
  }

  get totalPaginasCitasHoy(): number {
    return Math.max(1, Math.ceil(this.citasHoy.length / this.citasPorPagina));
  }

  cambiarPaginaCitasHoy(pagina: number): void {
    if (pagina >= 1 && pagina <= this.totalPaginasCitasHoy) {
      this.paginaCitasHoy = pagina;
    }
  }

  obtenerClaseEstado(estado: string): string {
    switch (estado?.replace(/[\s_]/g, '').toLowerCase()) {
      case 'confirmada':
        return 'status-confirmada';

      case 'enatencion':
        return 'status-atencion';

      case 'completada':
        return 'status-completada';

      case 'cancelada':
        return 'status-cancelada';

      default:
        return 'status-pendiente';
    }
  }
}