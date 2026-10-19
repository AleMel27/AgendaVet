import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { forkJoin } from 'rxjs';
import { PacienteService } from '../../core/services/paciente.service';
import { CatalogoService } from '../../core/services/catalogo.service';
import { Paciente, PacienteCrearRequest } from '../../core/models/paciente.model';
import { Especie, Raza, Propietario } from '../../core/models/catalogo.model';

@Component({
  selector: 'app-pacientes',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule],
  templateUrl: './pacientes.component.html',
  styleUrl: './pacientes.component.css'
})
export class PacientesComponent implements OnInit {
  private fb = inject(FormBuilder);
  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);
  private pacienteService = inject(PacienteService);
  private catalogoService = inject(CatalogoService);

  pacientes: Paciente[] = [];
  especies: Especie[] = [];
  razas: Raza[] = [];
  razasFiltradas: Raza[] = [];
  propietarios: Propietario[] = [];

  // Filtros
  filtroTexto = '';
  filtroEspecie: number | null = null;
  filtroSexo = '';

  cargando = true;
  guardando = false;
  errorMensaje = '';
  exitoMensaje = '';

  // Modales
  modalFormularioAbierto = false;
  modoEdicion = false;
  pacienteEditandoId: number | null = null;

  modalFichaAbierta = false;
  pacienteSeleccionado: Paciente | null = null;

  // Formulario
  pacienteForm = this.fb.nonNullable.group({
    nombre: ['', [Validators.required, Validators.maxLength(50)]],
    idPropietario: [0, [Validators.required, Validators.min(1)]],
    idEspecie: [0, [Validators.required, Validators.min(1)]],
    idRaza: [null as number | null],
    sexo: ['Macho', [Validators.required]],
    fechaNacimiento: [''],
    color: ['', [Validators.maxLength(30)]],
    esterilizado: [false],
    observaciones: ['', [Validators.maxLength(500)]]
  });

  ngOnInit(): void {
    this.cargarDatos();

    // Actualizar razas cuando cambia la especie en el formulario
    this.pacienteForm.controls.idEspecie.valueChanges.subscribe(idEspecie => {
      this.actualizarRazasPorEspecie(Number(idEspecie));
    });
  }

  cargarDatos(): void {
    this.cargando = true;
    this.errorMensaje = '';

    forkJoin({
      pacientes: this.pacienteService.obtenerTodos(),
      especies: this.catalogoService.obtenerEspecies(),
      razas: this.catalogoService.obtenerTodasRazas(),
      propietarios: this.catalogoService.obtenerPropietarios()
    }).subscribe({
      next: (res) => {
        this.pacientes = res.pacientes || [];
        this.especies = res.especies || [];
        this.razas = res.razas || [];
        this.propietarios = res.propietarios || [];
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error al cargar pacientes:', err);
        this.errorMensaje = 'No se pudieron cargar los datos de pacientes.';
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  actualizarRazasPorEspecie(idEspecie: number): void {
    if (!idEspecie || idEspecie === 0) {
      this.razasFiltradas = [];
      return;
    }
    this.razasFiltradas = this.razas.filter(r => r.idEspecie === Number(idEspecie));
  }

  // Pacientes filtrados
  get pacientesFiltrados(): Paciente[] {
    const texto = this.filtroTexto.toLowerCase().trim();

    return this.pacientes.filter(p => {
      if (texto) {
        const nombre = (p.nombre || '').toLowerCase();
        const dueño = (p.propietarioNombreCompleto || '').toLowerCase();
        const raza = (p.razaNombre || '').toLowerCase();
        if (!nombre.includes(texto) && !dueño.includes(texto) && !raza.includes(texto)) {
          return false;
        }
      }

      if (this.filtroEspecie && p.idEspecie !== Number(this.filtroEspecie)) {
        return false;
      }

      if (this.filtroSexo && p.sexo !== this.filtroSexo) {
        return false;
      }

      return true;
    });
  }

  // Estadísticas rápidas
  get totalCaninos(): number {
    return this.pacientes.filter(p => p.especieNombre?.toLowerCase().includes('canino') || p.especieNombre?.toLowerCase().includes('perro')).length;
  }

  get totalFelinos(): number {
    return this.pacientes.filter(p => p.especieNombre?.toLowerCase().includes('felino') || p.especieNombre?.toLowerCase().includes('gato')).length;
  }

  get totalEsterilizados(): number {
    return this.pacientes.filter(p => p.esterilizado).length;
  }

  // Modales
  abrirModalCrear(): void {
    this.modoEdicion = false;
    this.pacienteEditandoId = null;

    this.pacienteForm.reset({
      nombre: '',
      idPropietario: this.propietarios.length > 0 ? this.propietarios[0].idPropietario : 0,
      idEspecie: this.especies.length > 0 ? this.especies[0].idEspecie : 0,
      idRaza: null,
      sexo: 'Macho',
      fechaNacimiento: '',
      color: '',
      esterilizado: false,
      observaciones: ''
    });

    if (this.especies.length > 0) {
      this.actualizarRazasPorEspecie(this.especies[0].idEspecie);
    }

    this.modalFormularioAbierto = true;
  }

  abrirModalEditar(paciente: Paciente, event?: Event): void {
    if (event) event.stopPropagation();

    this.modoEdicion = true;
    this.pacienteEditandoId = paciente.idPaciente;

    this.actualizarRazasPorEspecie(paciente.idEspecie);

    let fechaFormateada = '';
    if (paciente.fechaNacimiento) {
      fechaFormateada = paciente.fechaNacimiento.substring(0, 10);
    }

    this.pacienteForm.reset({
      nombre: paciente.nombre,
      idPropietario: paciente.idPropietario,
      idEspecie: paciente.idEspecie,
      idRaza: paciente.idRaza ?? null,
      sexo: paciente.sexo || 'Macho',
      fechaNacimiento: fechaFormateada,
      color: paciente.color || '',
      esterilizado: paciente.esterilizado,
      observaciones: paciente.observaciones || ''
    });

    this.modalFormularioAbierto = true;
  }

  cerrarModalFormulario(): void {
    this.modalFormularioAbierto = false;
    this.pacienteEditandoId = null;
  }

  abrirFicha(paciente: Paciente): void {
    this.pacienteSeleccionado = paciente;
    this.modalFichaAbierta = true;
  }

  cerrarFicha(): void {
    this.modalFichaAbierta = false;
    this.pacienteSeleccionado = null;
  }

  guardarPaciente(): void {
    if (this.pacienteForm.invalid || this.guardando) {
      this.pacienteForm.markAllAsTouched();
      return;
    }

    this.guardando = true;
    const formVal = this.pacienteForm.getRawValue();

    const requestData: PacienteCrearRequest = {
      idPropietario: Number(formVal.idPropietario),
      idEspecie: Number(formVal.idEspecie),
      idRaza: formVal.idRaza ? Number(formVal.idRaza) : null,
      nombre: formVal.nombre.trim(),
      sexo: formVal.sexo,
      fechaNacimiento: formVal.fechaNacimiento ? new Date(formVal.fechaNacimiento).toISOString() : null,
      color: formVal.color?.trim() || null,
      esterilizado: !!formVal.esterilizado,
      observaciones: formVal.observaciones?.trim() || null
    };

    if (this.modoEdicion && this.pacienteEditandoId) {
      this.pacienteService.actualizar(this.pacienteEditandoId, requestData).subscribe({
        next: (actualizado) => {
          this.guardando = false;
          const idx = this.pacientes.findIndex(p => p.idPaciente === actualizado.idPaciente);
          if (idx !== -1) {
            this.pacientes[idx] = actualizado;
          }
          this.cerrarModalFormulario();
          this.mostrarMensaje('¡Paciente actualizado exitosamente!');
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error('Error al actualizar paciente:', err);
          this.guardando = false;
          alert('No se pudo actualizar el paciente.');
          this.cdr.detectChanges();
        }
      });
    } else {
      this.pacienteService.crear(requestData).subscribe({
        next: (creado) => {
          this.guardando = false;
          this.pacientes.unshift(creado);
          this.cerrarModalFormulario();
          this.mostrarMensaje('¡Paciente registrado con éxito!');
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error('Error al registrar paciente:', err);
          this.guardando = false;
          alert('No se pudo registrar el paciente. Verifica los campos requeridos.');
          this.cdr.detectChanges();
        }
      });
    }
  }

  eliminarPaciente(paciente: Paciente, event?: Event): void {
    if (event) event.stopPropagation();

    if (!confirm(`¿Estás seguro de eliminar el expediente de ${paciente.nombre}?`)) {
      return;
    }

    this.guardando = true;
    this.pacienteService.eliminar(paciente.idPaciente).subscribe({
      next: () => {
        this.pacientes = this.pacientes.filter(p => p.idPaciente !== paciente.idPaciente);
        this.guardando = false;
        if (this.modalFichaAbierta && this.pacienteSeleccionado?.idPaciente === paciente.idPaciente) {
          this.cerrarFicha();
        }
        this.mostrarMensaje('Paciente eliminado del sistema.');
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error al eliminar paciente:', err);
        this.guardando = false;
        alert('No se pudo eliminar el paciente. Puede que tenga citas o historias clínicas asociadas.');
        this.cdr.detectChanges();
      }
    });
  }

  agendarCitaParaPaciente(paciente: Paciente): void {
    this.cerrarFicha();
    this.router.navigate(['/citas']);
  }

  mostrarMensaje(msg: string): void {
    this.exitoMensaje = msg;
    setTimeout(() => {
      this.exitoMensaje = '';
      this.cdr.detectChanges();
    }, 3500);
  }

  calcularEdad(fechaIso?: string | null): string {
    if (!fechaIso) return 'No especificada';
    const nacimiento = new Date(fechaIso);
    const hoy = new Date();
    let anios = hoy.getFullYear() - nacimiento.getFullYear();
    let meses = hoy.getMonth() - nacimiento.getMonth();

    if (meses < 0 || (meses === 0 && hoy.getDate() < nacimiento.getDate())) {
      anios--;
      meses += 12;
    }

    if (anios <= 0) {
      return `${meses} mes${meses !== 1 ? 'es' : ''}`;
    }
    return `${anios} año${anios !== 1 ? 's' : ''}${meses > 0 ? ` y ${meses} m.` : ''}`;
  }
}
