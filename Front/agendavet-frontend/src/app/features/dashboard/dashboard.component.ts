import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DashboardService } from '../../core/services/dashboard.service';

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

  citasHoy: any[] = [];

  cargando = true;
  errorMensaje = '';

  ngOnInit(): void {
    this.dashboardService.obtenerEstadisticas().subscribe({
      next: (datos) => {
        this.estadisticas = datos;
        this.citasHoy = datos.citasLista;
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

  obtenerClaseEstado(estado: string): string {
    switch (estado?.toLowerCase()) {
      case 'confirmada':
        return 'status-confirmada';

      case 'enatencion':
      case 'en atención':
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