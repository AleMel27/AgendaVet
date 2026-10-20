
import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { filter } from 'rxjs';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.css'
})
export class MainLayout implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  menuAbierto = false;

  ngOnInit(): void {
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.cdr.detectChanges();
      });
  }

  get usuario() {
    return this.authService.obtenerUsuario();
  }

  get tituloPagina(): string {
    const ruta = this.router.url.split(/[?#]/, 1)[0].split('/').filter(Boolean).at(-1);
    const titulos: Record<string, string> = {
      dashboard: 'Dashboard',
      citas: 'Citas',
      pacientes: 'Pacientes',
      propietarios: 'Propietarios',
      'historias-clinicas': 'Historias clínicas',
      servicios: 'Servicios',
      usuarios: 'Usuarios'
    };
    return ruta ? titulos[ruta] ?? 'Panel de gestión' : 'Panel de gestión';
  }

  get esAdministrador(): boolean {
    return Number(this.usuario?.idRol) === 1 ||
      this.normalizarRol(this.usuario?.rolNombre) === 'administrador';
  }

  get esVeterinario(): boolean {
    return Number(this.usuario?.idRol) === 2 ||
      this.normalizarRol(this.usuario?.rolNombre) === 'veterinario';
  }

  get esRecepcionista(): boolean {
    return Number(this.usuario?.idRol) === 3 ||
      this.normalizarRol(this.usuario?.rolNombre) === 'recepcionista';
  }

  get puedeVerHistoriasClinicas(): boolean {
    return this.esAdministrador || this.esVeterinario;
  }

  get puedeVerUsuarios(): boolean {
    return this.esAdministrador;
  }

  private normalizarRol(rol?: string | null): string {
    return (rol ?? '')
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .toLowerCase()
      .trim();
  }

  cerrarSesion(): void {
    this.authService.cerrarSesion();
    this.router.navigate(['/login']);
  }
}
