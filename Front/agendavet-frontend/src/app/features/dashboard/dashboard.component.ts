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
  constructor() {
    console.log('DashboardComponent iniciado');
  }


  private cdr = inject(ChangeDetectorRef);
  private dashboardService = inject(DashboardService);

  estadisticas = {
    citasHoy: 0,
    pacientes: 0,
    propietarios: 0,
    servicios: 0
  };

  cargando = true;
  errorMensaje = '';

  ngOnInit(): void {
    this.dashboardService.obtenerEstadisticas().subscribe({
      next: (datos) => {
        this.estadisticas = datos;
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
}
