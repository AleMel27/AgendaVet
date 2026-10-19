
import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  mostrarPassword = false;
  cargando = false;
  errorMensaje = '';

  loginForm = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  iniciarSesion(): void {
    if (this.loginForm.invalid || this.cargando) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.cargando = true;
    this.errorMensaje = '';

    this.authService.login(this.loginForm.getRawValue()).subscribe({
      next: () => {
        this.router.navigate(['/dashboard']);
      },
      error: (error) => {
        this.cargando = false;

        if (error.status === 401) {
          this.errorMensaje = 'Correo o contraseña incorrectos, o cuenta inactiva.';
        } else if (error.status === 0) {
          this.errorMensaje = 'No se pudo conectar con el servidor. Verifica que el backend esté activo.';
        } else {
          this.errorMensaje = 'Ocurrió un error al iniciar sesión. Inténtalo nuevamente.';
        }
      }
    });
  }
}
