
import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.css'
})
export class MainLayout {
  private authService = inject(AuthService);
  private router = inject(Router);

  menuAbierto = false;

  get usuario() {
    return this.authService.obtenerUsuario();
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
