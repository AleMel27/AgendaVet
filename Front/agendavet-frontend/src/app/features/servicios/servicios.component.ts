import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ServicioService } from '../../core/services/servicio.service';
import { Servicio, ServicioRequest } from '../../core/models/servicio.model';

@Component({
  selector: 'app-servicios',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './servicios.component.html',
  styleUrl: './servicios.component.css'
})
export class ServiciosComponent implements OnInit {
  private servicioService = inject(ServicioService);
  private fb = inject(FormBuilder);
  private cdr = inject(ChangeDetectorRef);

  servicios: Servicio[] = [];
  serviciosFiltrados: Servicio[] = [];
  cargando = true;
  guardando = false;
  errorMensaje: string | null = null;
  mensajeExito: string | null = null;


  // Filtros
  busqueda = '';
  filtroEstado: 'todos' | 'activos' | 'inactivos' = 'todos';

  // Modal de Crear / Editar
  mostrarModal = false;
  modoEdicion = false;
  servicioSeleccionado: Servicio | null = null;
  formularioServicio: FormGroup;

  // Modal de confirmación para alternar estado
  mostrarConfirmacion = false;
  servicioParaCambiarEstado: Servicio | null = null;

  readonly presetsDuracion = [15, 30, 45, 60, 90, 120];

  constructor() {
    this.formularioServicio = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(100)]],
      descripcion: ['', [Validators.maxLength(255)]],
      precio: [null, [Validators.required, Validators.min(0.01), Validators.max(1000000)]],
      duracionEstimadaMin: [30, [Validators.required, Validators.min(5), Validators.max(480)]],
      activo: [true]
    });
  }

  ngOnInit(): void {
    this.cargarServicios();
  }

  cargarServicios(): void {
    this.cargando = true;
    this.errorMensaje = null;

    // Obtenemos todos los servicios (activos e inactivos)
    this.servicioService.obtenerTodos(false).subscribe({
      next: (data) => {
        this.servicios = data ?? [];
        this.aplicarFiltros();
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error al cargar servicios:', err);
        this.errorMensaje = 'No se pudieron cargar los servicios. Por favor intenta de nuevo.';
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  aplicarFiltros(): void {
    const termino = this.busqueda.trim().toLowerCase();

    this.serviciosFiltrados = this.servicios.filter(servicio => {
      const coincideBusqueda = !termino ||
        servicio.nombre.toLowerCase().includes(termino) ||
        (servicio.descripcion && servicio.descripcion.toLowerCase().includes(termino));

      const coincideEstado =
        this.filtroEstado === 'todos' ||
        (this.filtroEstado === 'activos' && servicio.activo) ||
        (this.filtroEstado === 'inactivos' && !servicio.activo);

      return coincideBusqueda && coincideEstado;
    });
  }

  onBuscar(evento: Event): void {
    this.busqueda = (evento.target as HTMLInputElement).value;
    this.aplicarFiltros();
  }

  limpiarBusqueda(): void {
    this.busqueda = '';
    this.aplicarFiltros();
  }

  cambiarFiltroEstado(estado: 'todos' | 'activos' | 'inactivos'): void {
    this.filtroEstado = estado;
    this.aplicarFiltros();
  }

  get totalServicios(): number {
    return this.servicios.length;
  }

  get serviciosActivos(): number {
    return this.servicios.filter(s => s.activo).length;
  }

  get serviciosInactivos(): number {
    return this.servicios.filter(s => !s.activo).length;
  }

  get duracionPromedio(): number {
    if (this.servicios.length === 0) return 0;
    const suma = this.servicios.reduce((acc, s) => acc + (s.duracionEstimadaMin || 0), 0);
    return Math.round(suma / this.servicios.length);
  }

  abrirModalNuevo(): void {
    this.modoEdicion = false;
    this.servicioSeleccionado = null;
    this.formularioServicio.reset({
      nombre: '',
      descripcion: '',
      precio: null,
      duracionEstimadaMin: 30,
      activo: true
    });
    this.mostrarModal = true;
  }

  abrirModalEditar(servicio: Servicio): void {
    this.modoEdicion = true;
    this.servicioSeleccionado = servicio;
    this.formularioServicio.patchValue({
      nombre: servicio.nombre,
      descripcion: servicio.descripcion ?? '',
      precio: servicio.precio,
      duracionEstimadaMin: servicio.duracionEstimadaMin,
      activo: servicio.activo
    });
    this.mostrarModal = true;
  }

  cerrarModal(): void {
    this.mostrarModal = false;
    this.servicioSeleccionado = null;
    this.formularioServicio.reset();
  }

  seleccionarPresetDuracion(minutos: number): void {
    this.formularioServicio.patchValue({ duracionEstimadaMin: minutos });
  }

  guardarServicio(): void {
    if (this.formularioServicio.invalid) {
      this.formularioServicio.markAllAsTouched();
      return;
    }

    this.guardando = true;
    const valor = this.formularioServicio.value;

    const dto: ServicioRequest = {
      nombre: valor.nombre.trim(),
      descripcion: valor.descripcion ? valor.descripcion.trim() : null,
      precio: Number(valor.precio),
      duracionEstimadaMin: Number(valor.duracionEstimadaMin),
      activo: Boolean(valor.activo)
    };

    if (this.modoEdicion && this.servicioSeleccionado) {
      this.servicioService.actualizar(this.servicioSeleccionado.idServicio, dto).subscribe({
        next: (actualizado) => {
          const idx = this.servicios.findIndex(s => s.idServicio === actualizado.idServicio);
          if (idx !== -1) {
            this.servicios[idx] = actualizado;
          }
          this.aplicarFiltros();
          this.guardando = false;
          this.cerrarModal();
          this.mostrarNotificacion('Servicio actualizado con éxito.');
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error('Error al actualizar servicio:', err);
          this.guardando = false;
          this.cdr.detectChanges();
          alert('No se pudo actualizar el servicio. Revisa los datos ingresados.');
        }
      });
    } else {
      this.servicioService.crear(dto).subscribe({
        next: (creado) => {
          this.servicios.unshift(creado);
          this.aplicarFiltros();
          this.guardando = false;
          this.cerrarModal();
          this.mostrarNotificacion('Servicio creado exitosamente.');
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error('Error al crear servicio:', err);
          this.guardando = false;
          this.cdr.detectChanges();
          alert('No se pudo registrar el servicio. Revisa los datos ingresados.');
        }
      });
    }
  }

  solicitarCambioEstado(servicio: Servicio): void {
    this.servicioParaCambiarEstado = servicio;
    this.mostrarConfirmacion = true;
    this.cdr.detectChanges();
  }

  cancelarCambioEstado(): void {
    this.servicioParaCambiarEstado = null;
    this.mostrarConfirmacion = false;
    this.cdr.detectChanges();
  }

  confirmarCambioEstado(): void {
    if (!this.servicioParaCambiarEstado) return;

    const s = this.servicioParaCambiarEstado;
    const nuevoEstado = !s.activo;

    const dto: ServicioRequest = {
      nombre: s.nombre,
      descripcion: s.descripcion,
      precio: s.precio,
      duracionEstimadaMin: s.duracionEstimadaMin,
      activo: nuevoEstado
    };

    this.servicioService.actualizar(s.idServicio, dto).subscribe({
      next: (actualizado) => {
        const idx = this.servicios.findIndex(item => item.idServicio === actualizado.idServicio);
        if (idx !== -1) {
          this.servicios[idx] = actualizado;
        }
        this.aplicarFiltros();
        this.mostrarConfirmacion = false;
        this.servicioParaCambiarEstado = null;
        this.mostrarNotificacion(
          nuevoEstado ? 'Servicio activado correctamente.' : 'Servicio desactivado correctamente.'
        );
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error al cambiar estado:', err);
        this.mostrarConfirmacion = false;
        this.servicioParaCambiarEstado = null;
        this.cdr.detectChanges();
        alert('No se pudo modificar el estado del servicio.');
      }
    });
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
