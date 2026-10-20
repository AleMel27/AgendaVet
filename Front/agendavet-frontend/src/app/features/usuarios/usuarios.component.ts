import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UsuarioService } from '../../core/services/usuario.service';
import { Usuario, Rol, UsuarioCreateRequest, UsuarioUpdateRequest } from '../../core/models/usuario.model';

@Component({
  selector: 'app-usuarios',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './usuarios.component.html',
  styleUrl: './usuarios.component.css'
})
export class UsuariosComponent implements OnInit {
  private usuarioService = inject(UsuarioService);
  private fb = inject(FormBuilder);
  private cdr = inject(ChangeDetectorRef);

  usuarios: Usuario[] = [];
  roles: Rol[] = [];

  // Paginación y búsqueda
  paginaActual = 1;
  tamanoPagina = 10;
  totalRegistros = 0;
  totalPaginas = 1;
  busqueda = '';
  filtroRolId: number | 'todos' = 'todos';

  // Métricas generales
  totalUsuariosCount = 0;
  adminCount = 0;
  vetCount = 0;
  recepCount = 0;

  // Estados de carga
  cargando = true;
  guardando = false;
  errorMensaje: string | null = null;
  mensajeExito: string | null = null;

  // Modales
  mostrarModal = false;
  modoEdicion = false;
  usuarioSeleccionado: Usuario | null = null;
  mostrarPassword = false;

  mostrarConfirmacionEliminar = false;
  usuarioParaEliminar: Usuario | null = null;

  // Formularios
  formCrear: FormGroup;
  formEditar: FormGroup;

  constructor() {
    this.formCrear = this.fb.group({
      idRol: [null, [Validators.required]],
      nombre: ['', [Validators.required, Validators.maxLength(50)]],
      apellido: ['', [Validators.required, Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      telefono: ['', [Validators.maxLength(20)]],
      activo: [true]
    });

    this.formEditar = this.fb.group({
      idRol: [null, [Validators.required]],
      nombre: ['', [Validators.required, Validators.maxLength(50)]],
      apellido: ['', [Validators.required, Validators.maxLength(50)]],
      telefono: ['', [Validators.maxLength(20)]],
      activo: [true]
    });
  }

  ngOnInit(): void {
    this.cargarRoles();
    this.cargarMetricasGenerales();
    this.cargarUsuarios();
  }

  cargarRoles(): void {
    this.usuarioService.obtenerRoles().subscribe({
      next: (data) => {
        this.roles = data ?? [];
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error al cargar roles:', err);
      }
    });
  }

  cargarMetricasGenerales(): void {
    this.usuarioService.obtenerTodos().subscribe({
      next: (todos) => {
        const lista = todos ?? [];
        this.totalUsuariosCount = lista.length;
        this.adminCount = lista.filter(u => this.normalizarRol(u.rolNombre) === 'administrador' || u.idRol === 1).length;
        this.vetCount = lista.filter(u => this.normalizarRol(u.rolNombre) === 'veterinario' || u.idRol === 2).length;
        this.recepCount = lista.filter(u => this.normalizarRol(u.rolNombre) === 'recepcionista' || u.idRol === 3).length;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error al cargar métricas generales de usuarios:', err);
      }
    });
  }

  cargarUsuarios(): void {
    this.cargando = true;
    this.errorMensaje = null;
    this.cdr.detectChanges();

    this.usuarioService.obtenerPaginado(this.paginaActual, this.tamanoPagina, this.busqueda).subscribe({
      next: (resp) => {
        this.usuarios = resp.items ?? [];
        this.totalRegistros = resp.totalCount ?? 0;
        this.paginaActual = resp.pageNumber ?? 1;
        this.tamanoPagina = resp.pageSize ?? 10;
        this.totalPaginas = resp.totalPages || Math.ceil(this.totalRegistros / this.tamanoPagina) || 1;
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error al cargar usuarios:', err);
        this.errorMensaje = 'No se pudo obtener la lista de usuarios. Por favor intenta de nuevo.';
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  get usuariosFiltrados(): Usuario[] {
    if (this.filtroRolId === 'todos') {
      return this.usuarios;
    }
    return this.usuarios.filter(u => u.idRol === this.filtroRolId);
  }

  onBuscar(evento: Event): void {
    this.busqueda = (evento.target as HTMLInputElement).value;
    this.paginaActual = 1;
    this.cargarUsuarios();
  }

  limpiarBusqueda(): void {
    this.busqueda = '';
    this.paginaActual = 1;
    this.cargarUsuarios();
  }

  cambiarFiltroRol(idRol: number | 'todos'): void {
    this.filtroRolId = idRol;
    this.cdr.detectChanges();
  }

  cambiarPagina(nuevaPagina: number): void {
    if (nuevaPagina >= 1 && nuevaPagina <= this.totalPaginas && nuevaPagina !== this.paginaActual) {
      this.paginaActual = nuevaPagina;
      this.cargarUsuarios();
    }
  }

  cambiarTamanoPagina(evento: Event): void {
    this.tamanoPagina = Number((evento.target as HTMLSelectElement).value);
    this.paginaActual = 1;
    this.cargarUsuarios();
  }

  // Modales
  abrirModalNuevo(): void {
    this.modoEdicion = false;
    this.usuarioSeleccionado = null;
    this.mostrarPassword = false;
    this.formCrear.reset({
      idRol: this.roles.length > 0 ? this.roles[0].idRol : 2,
      nombre: '',
      apellido: '',
      email: '',
      password: '',
      telefono: '',
      activo: true
    });
    this.mostrarModal = true;
    this.cdr.detectChanges();
  }

  abrirModalEditar(usuario: Usuario): void {
    this.modoEdicion = true;
    this.usuarioSeleccionado = usuario;
    this.formEditar.patchValue({
      idRol: usuario.idRol,
      nombre: usuario.nombre,
      apellido: usuario.apellido,
      telefono: usuario.telefono ?? '',
      activo: usuario.activo
    });
    this.mostrarModal = true;
    this.cdr.detectChanges();
  }

  cerrarModal(): void {
    this.mostrarModal = false;
    this.usuarioSeleccionado = null;
    this.mostrarPassword = false;
    this.cdr.detectChanges();
  }

  guardarUsuario(): void {
    if (this.modoEdicion) {
      if (this.formEditar.invalid) {
        this.formEditar.markAllAsTouched();
        return;
      }

      if (!this.usuarioSeleccionado) return;

      this.guardando = true;
      this.cdr.detectChanges();
      const v = this.formEditar.value;
      const dto: UsuarioUpdateRequest = {
        idRol: Number(v.idRol),
        nombre: v.nombre.trim(),
        apellido: v.apellido.trim(),
        telefono: v.telefono ? v.telefono.trim() : null,
        activo: Boolean(v.activo)
      };

      this.usuarioService.actualizar(this.usuarioSeleccionado.idUsuario, dto).subscribe({
        next: (actualizado) => {
          this.guardando = false;
          this.cerrarModal();
          this.mostrarNotificacion('Usuario actualizado correctamente.');
          this.cargarUsuarios();
          this.cargarMetricasGenerales();
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error('Error al actualizar usuario:', err);
          this.guardando = false;
          this.cdr.detectChanges();
          alert('No se pudo actualizar el usuario. Verifica los campos requeridos.');
        }
      });
    } else {
      if (this.formCrear.invalid) {
        this.formCrear.markAllAsTouched();
        return;
      }

      this.guardando = true;
      this.cdr.detectChanges();
      const v = this.formCrear.value;
      const dto: UsuarioCreateRequest = {
        idRol: Number(v.idRol),
        nombre: v.nombre.trim(),
        apellido: v.apellido.trim(),
        email: v.email.trim(),
        password: v.password,
        telefono: v.telefono ? v.telefono.trim() : null,
        activo: Boolean(v.activo)
      };

      this.usuarioService.crear(dto).subscribe({
        next: (creado) => {
          this.guardando = false;
          this.cerrarModal();
          this.mostrarNotificacion('Usuario registrado exitosamente.');
          this.cargarUsuarios();
          this.cargarMetricasGenerales();
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error('Error al crear usuario:', err);
          this.guardando = false;
          this.cdr.detectChanges();
          const msg = err.error?.mensaje || err.error?.message || 'No se pudo crear el usuario. Revisa el correo y contraseña.';
          alert(msg);
        }
      });
    }
  }

  // Eliminación / Desactivación
  solicitarEliminacion(usuario: Usuario): void {
    this.usuarioParaEliminar = usuario;
    this.mostrarConfirmacionEliminar = true;
    this.cdr.detectChanges();
  }

  cancelarEliminacion(): void {
    this.usuarioParaEliminar = null;
    this.mostrarConfirmacionEliminar = false;
    this.cdr.detectChanges();
  }

  confirmarEliminacion(): void {
    if (!this.usuarioParaEliminar) return;

    const id = this.usuarioParaEliminar.idUsuario;
    this.usuarioService.eliminar(id).subscribe({
      next: () => {
        this.mostrarConfirmacionEliminar = false;
        this.usuarioParaEliminar = null;
        this.mostrarNotificacion('Usuario desactivado del sistema.');
        this.cargarUsuarios();
        this.cargarMetricasGenerales();
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error al desactivar usuario:', err);
        this.mostrarConfirmacionEliminar = false;
        this.usuarioParaEliminar = null;
        this.cdr.detectChanges();
        alert('No se pudo desactivar el usuario.');
      }
    });
  }

  getIniciales(usuario: Usuario): string {
    const n = usuario.nombre?.charAt(0) || '';
    const a = usuario.apellido?.charAt(0) || '';
    return (n + a).toUpperCase() || 'U';
  }

  obtenerClaseRol(rolNombre?: string): string {
    const rol = this.normalizarRol(rolNombre);
    if (rol === 'administrador') return 'role-admin';
    if (rol === 'veterinario') return 'role-vet';
    if (rol === 'recepcionista') return 'role-recep';
    return 'role-default';
  }

  private normalizarRol(rol?: string | null): string {
    return (rol ?? '')
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .toLowerCase()
      .trim();
  }

  private mostrarNotificacion(mensaje: string): void {
    this.mensajeExito = mensaje;
    this.cdr.detectChanges();
    setTimeout(() => {
      if (this.mensajeExito === mensaje) {
        this.mensajeExito = null;
        this.cdr.detectChanges();
      }
    }, 3500);
  }
}
