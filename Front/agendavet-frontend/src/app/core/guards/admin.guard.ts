import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const adminGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.estaAutenticado()) {
    return router.createUrlTree(['/login']);
  }

  const usuario = authService.obtenerUsuario();
  const esAdmin =
    Number(usuario?.idRol) === 1 ||
    (usuario?.rolNombre ?? '')
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .toLowerCase()
      .trim() === 'administrador';

  if (esAdmin) {
    return true;
  }

  return router.createUrlTree(['/dashboard']);
};
