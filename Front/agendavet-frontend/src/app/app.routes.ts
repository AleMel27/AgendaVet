
import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login.component';
import { authGuard } from './core/guards/auth-guard';
import { MainLayout } from './shared/layout/main-layout/main-layout';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: '',
    component: MainLayout,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then(
            m => m.DashboardComponent
          )
      },
      {
        path: 'citas',
        loadComponent: () =>
          import('./features/citas/citas.component').then(
            m => m.CitasComponent
          )
      },
      {
        path: 'pacientes',
        loadComponent: () =>
          import('./features/pacientes/pacientes.component').then(
            m => m.PacientesComponent
          )
      },
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: '**',
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then(
            m => m.DashboardComponent
          )
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];
