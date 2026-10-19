import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { forkJoin } from 'rxjs';
import { CitaService } from '../../core/services/cita.service';
import { PacienteService } from '../../core/services/paciente.service';
import { CatalogoService } from '../../core/services/catalogo.service';
import { Cita } from '../../core/models/cita.model';
import { Paciente } from '../../core/models/paciente.model';
import { Servicio, Veterinario } from '../../core/models/catalogo.model';

interface DiaCalendario {
  fecha: Date;
  numero: number;
  esMesActual: boolean;
  esHoy: boolean;
  citas: Cita[];
}

interface DiaSemana {
  fecha: Date;
  nombre: string;
  numero: number;
  esHoy: boolean;
}

@Component({
  selector: 'app-citas',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule],
  templateUrl: './citas.component.html',
  styleUrl: './citas.component.css'
})
export class CitasComponent implements OnInit {
  private fb = inject(FormBuilder);
  private cdr = inject(ChangeDetectorRef);
  private citaService = inject(CitaService);
  private pacienteService = inject(PacienteService);
  private catalogoService = inject(CatalogoService);

  fechaActual = new Date();
  vista: 'mes' | 'semana' | 'agenda' = 'mes';

  citas: Cita[] = [];
  pacientes: Paciente[] = [];
  veterinarios: Veterinario[] = [];
  servicios: Servicio[] = [];

  filtroVeterinario: number | null = null;
  filtroEstado: string = '';

  cargando = true;
  guardando = false;
  errorMensaje = '';
  exitoMensaje = '';

  // Modales
  modalNuevoAbierto = false;
  modalDetalleAbierto = false;
  citaSeleccionada: Cita | null = null;

  // Formulario de nueva cita
  citaForm = this.fb.nonNullable.group({
    idPaciente: [0, [Validators.required, Validators.min(1)]],
    idVeterinario: [0, [Validators.required, Validators.min(1)]],
    idServicio: [0, [Validators.required, Validators.min(1)]],
    fechaHora: ['', [Validators.required]],
    motivo: ['', [Validators.maxLength(255)]],
    notasAgendamiento: ['', [Validators.maxLength(500)]]
  });

  // Nombres de días y meses
  readonly diasNombres = ['Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb', 'Dom'];
  readonly horasDelDia = [8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19];

  ngOnInit(): void {
    this.cargarDatos();
  }

  cargarDatos(): void {
    this.cargando = true;
    this.errorMensaje = '';

    forkJoin({
      citas: this.citaService.obtenerTodas(),
      pacientes: this.pacienteService.obtenerTodos(),
      veterinarios: this.catalogoService.obtenerVeterinarios(),
      servicios: this.catalogoService.obtenerServicios(true)
    }).subscribe({
      next: (resultado) => {
        this.citas = resultado.citas || [];
        this.pacientes = resultado.pacientes || [];
        this.veterinarios = resultado.veterinarios || [];
        this.servicios = resultado.servicios || [];
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error al cargar datos de citas:', err);
        this.errorMensaje = 'No se pudieron cargar las citas. Verifica la conexión con el servidor.';
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  // Navegación de fechas
  irAHoy(): void {
    this.fechaActual = new Date();
  }

  anterior(): void {
    if (this.vista === 'mes') {
      this.fechaActual = new Date(this.fechaActual.getFullYear(), this.fechaActual.getMonth() - 1, 1);
    } else if (this.vista === 'semana') {
      const nueva = new Date(this.fechaActual);
      nueva.setDate(nueva.getDate() - 7);
      this.fechaActual = nueva;
    } else {
      const nueva = new Date(this.fechaActual);
      nueva.setDate(nueva.getDate() - 15);
      this.fechaActual = nueva;
    }
  }

  siguiente(): void {
    if (this.vista === 'mes') {
      this.fechaActual = new Date(this.fechaActual.getFullYear(), this.fechaActual.getMonth() + 1, 1);
    } else if (this.vista === 'semana') {
      const nueva = new Date(this.fechaActual);
      nueva.setDate(nueva.getDate() + 7);
      this.fechaActual = nueva;
    } else {
      const nueva = new Date(this.fechaActual);
      nueva.setDate(nueva.getDate() + 15);
      this.fechaActual = nueva;
    }
  }

  cambiarVista(nuevaVista: 'mes' | 'semana' | 'agenda'): void {
    this.vista = nuevaVista;
  }

  get tituloPeriodo(): string {
    const meses = [
      'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
      'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
    ];
    if (this.vista === 'mes' || this.vista === 'agenda') {
      return `${meses[this.fechaActual.getMonth()]} de ${this.fechaActual.getFullYear()}`;
    }
    // Vista semana
    const dias = this.diasDeLaSemana;
    const inicio = dias[0].fecha;
    const fin = dias[6].fecha;
    if (inicio.getMonth() === fin.getMonth()) {
      return `${inicio.getDate()} - ${fin.getDate()} de ${meses[inicio.getMonth()]} ${inicio.getFullYear()}`;
    }
    return `${inicio.getDate()} de ${meses[inicio.getMonth()]} - ${fin.getDate()} de ${meses[fin.getMonth()]} ${fin.getFullYear()}`;
  }

  // Citas filtradas
  get citasFiltradas(): Cita[] {
    return this.citas.filter(cita => {
      if (this.filtroVeterinario && cita.idVeterinario !== Number(this.filtroVeterinario)) {
        return false;
      }
      if (this.filtroEstado && cita.estado !== this.filtroEstado) {
        return false;
      }
      return true;
    });
  }

  // Generador de cuadrícula del mes
  get diasDelMes(): DiaCalendario[] {
    const año = this.fechaActual.getFullYear();
    const mes = this.fechaActual.getMonth();
    const hoyStr = new Date().toDateString();

    const primerDiaMes = new Date(año, mes, 1);
    const ultimoDiaMes = new Date(año, mes + 1, 0);

    // Día de la semana del primer día (1 = lunes, ..., 7 = domingo)
    let diaInicio = primerDiaMes.getDay();
    diaInicio = diaInicio === 0 ? 7 : diaInicio; // Ajuste lunes=1

    const dias: DiaCalendario[] = [];

    // Días del mes anterior para rellenar
    const ultimoDiaMesAnterior = new Date(año, mes, 0).getDate();
    for (let i = diaInicio - 1; i > 0; i--) {
      const fecha = new Date(año, mes - 1, ultimoDiaMesAnterior - i + 1);
      dias.push({
        fecha,
        numero: fecha.getDate(),
        esMesActual: false,
        esHoy: fecha.toDateString() === hoyStr,
        citas: this.obtenerCitasDeFecha(fecha)
      });
    }

    // Días del mes actual
    for (let dia = 1; dia <= ultimoDiaMes.getDate(); dia++) {
      const fecha = new Date(año, mes, dia);
      dias.push({
        fecha,
        numero: dia,
        esMesActual: true,
        esHoy: fecha.toDateString() === hoyStr,
        citas: this.obtenerCitasDeFecha(fecha)
      });
    }

    // Días del siguiente mes para completar 35 o 42 casillas
    const casillasRestantes = 7 - (dias.length % 7);
    if (casillasRestantes < 7) {
      for (let i = 1; i <= casillasRestantes; i++) {
        const fecha = new Date(año, mes + 1, i);
        dias.push({
          fecha,
          numero: i,
          esMesActual: false,
          esHoy: fecha.toDateString() === hoyStr,
          citas: this.obtenerCitasDeFecha(fecha)
        });
      }
    }

    return dias;
  }

  // Días de la semana actual
  get diasDeLaSemana(): DiaSemana[] {
    const hoy = new Date(this.fechaActual);
    let diaSemana = hoy.getDay();
    diaSemana = diaSemana === 0 ? 7 : diaSemana; // Lunes = 1

    const lunes = new Date(hoy);
    lunes.setDate(hoy.getDate() - (diaSemana - 1));

    const dias: DiaSemana[] = [];
    const hoyStr = new Date().toDateString();

    for (let i = 0; i < 7; i++) {
      const fecha = new Date(lunes);
      fecha.setDate(lunes.getDate() + i);
      dias.push({
        fecha,
        nombre: this.diasNombres[i],
        numero: fecha.getDate(),
        esHoy: fecha.toDateString() === hoyStr
      });
    }

    return dias;
  }

  obtenerCitasDeFecha(fecha: Date): Cita[] {
    const año = fecha.getFullYear();
    const mes = String(fecha.getMonth() + 1).padStart(2, '0');
    const dia = String(fecha.getDate()).padStart(2, '0');
    const fechaBuscada = `${año}-${mes}-${dia}`;

    return this.citasFiltradas.filter(c => {
      const f = c.fechaHora?.substring(0, 10);
      return f === fechaBuscada;
    }).sort((a, b) => (a.fechaHora > b.fechaHora ? 1 : -1));
  }

  obtenerCitasDeFechaYHora(fecha: Date, hora: number): Cita[] {
    const citasDelDia = this.obtenerCitasDeFecha(fecha);
    return citasDelDia.filter(c => {
      const fechaCita = new Date(c.fechaHora);
      return fechaCita.getHours() === hora;
    });
  }

  // Modales
  abrirModalNuevo(fechaPrevia?: Date, hora?: number): void {
    const fecha = fechaPrevia ? new Date(fechaPrevia) : new Date();
    if (hora !== undefined) {
      fecha.setHours(hora, 0, 0, 0);
    } else if (!fechaPrevia) {
      fecha.setHours(9, 0, 0, 0);
    }

    // Formatear para datetime-local (YYYY-MM-DDTHH:mm)
    const yyyy = fecha.getFullYear();
    const mm = String(fecha.getMonth() + 1).padStart(2, '0');
    const dd = String(fecha.getDate()).padStart(2, '0');
    const hh = String(fecha.getHours()).padStart(2, '0');
    const min = String(fecha.getMinutes()).padStart(2, '0');
    const valorFechaHora = `${yyyy}-${mm}-${dd}T${hh}:${min}`;

    this.citaForm.reset({
      idPaciente: this.pacientes.length > 0 ? this.pacientes[0].idPaciente : 0,
      idVeterinario: this.veterinarios.length > 0 ? this.veterinarios[0].idUsuario : 0,
      idServicio: this.servicios.length > 0 ? this.servicios[0].idServicio : 0,
      fechaHora: valorFechaHora,
      motivo: '',
      notasAgendamiento: ''
    });

    this.modalNuevoAbierto = true;
  }

  cerrarModalNuevo(): void {
    this.modalNuevoAbierto = false;
  }

  abrirDetalle(cita: Cita, event?: Event): void {
    if (event) {
      event.stopPropagation();
    }
    this.citaSeleccionada = cita;
    this.modalDetalleAbierto = true;
  }

  cerrarDetalle(): void {
    this.modalDetalleAbierto = false;
    this.citaSeleccionada = null;
  }

  guardarCita(): void {
    if (this.citaForm.invalid || this.guardando) {
      this.citaForm.markAllAsTouched();
      return;
    }

    this.guardando = true;
    const datos = this.citaForm.getRawValue();

    this.citaService.crear({
      idPaciente: Number(datos.idPaciente),
      idVeterinario: Number(datos.idVeterinario),
      idServicio: Number(datos.idServicio),
      fechaHora: new Date(datos.fechaHora).toISOString(),
      motivo: datos.motivo || null,
      notasAgendamiento: datos.notasAgendamiento || null
    }).subscribe({
      next: (creada) => {
        this.guardando = false;
        this.citas.unshift(creada);
        this.cerrarModalNuevo();
        this.mostrarMensajeTemporal('¡Cita programada con éxito!');
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error al crear cita:', err);
        this.guardando = false;
        alert('Ocurrió un error al agendar la cita. Verifica que todos los campos sean válidos.');
        this.cdr.detectChanges();
      }
    });
  }

  cambiarEstado(cita: Cita, nuevoEstado: string): void {
    if (this.guardando) return;
    this.guardando = true;

    this.citaService.cambiarEstado(cita.idCita, nuevoEstado).subscribe({
      next: () => {
        cita.estado = nuevoEstado;
        this.guardando = false;
        this.mostrarMensajeTemporal(`Estado actualizado a "${nuevoEstado}"`);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error al cambiar estado:', err);
        this.guardando = false;
        alert('No se pudo actualizar el estado de la cita.');
        this.cdr.detectChanges();
      }
    });
  }

  eliminarCita(cita: Cita): void {
    if (!confirm(`¿Estás seguro de cancelar y eliminar la cita de ${cita.pacienteNombre}?`)) {
      return;
    }

    this.guardando = true;
    this.citaService.eliminar(cita.idCita).subscribe({
      next: () => {
        this.citas = this.citas.filter(c => c.idCita !== cita.idCita);
        this.guardando = false;
        this.cerrarDetalle();
        this.mostrarMensajeTemporal('Cita eliminada correctamente');
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error al eliminar cita:', err);
        this.guardando = false;
        alert('No se pudo eliminar la cita.');
        this.cdr.detectChanges();
      }
    });
  }

  mostrarMensajeTemporal(mensaje: string): void {
    this.exitoMensaje = mensaje;
    setTimeout(() => {
      this.exitoMensaje = '';
      this.cdr.detectChanges();
    }, 3500);
  }

  obtenerClaseEstado(estado: string): string {
    switch (estado?.toLowerCase()) {
      case 'confirmada':
        return 'estado-confirmada';
      case 'enatencion':
      case 'en atención':
        return 'estado-enatencion';
      case 'completada':
        return 'estado-completada';
      case 'cancelada':
        return 'estado-cancelada';
      case 'pendiente':
      default:
        return 'estado-pendiente';
    }
  }

  formatearHora(fechaIso: string): string {
    if (!fechaIso) return '';
    const d = new Date(fechaIso);
    return d.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit', hour12: false });
  }

  formatearFechaCompleta(fechaIso: string): string {
    if (!fechaIso) return '';
    const d = new Date(fechaIso);
    return d.toLocaleDateString('es-ES', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
