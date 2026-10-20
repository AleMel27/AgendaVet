import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { HistoriaClinicaService, HistoriaClinica, HistoriaClinicaCreateRequest } from '../../core/services/historia-clinica.service';
import { PacienteService } from '../../core/services/paciente.service';
import { AuthService } from '../../core/services/auth.service';
import { Paciente } from '../../core/models/paciente.model';
import { PaginatedResponse } from '../../core/models/catalogo.model';

@Component({
    selector: 'app-historias-clinicas',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormsModule],
    templateUrl: './historias-clinicas.component.html',
    styleUrl: './historias-clinicas.component.css'
})
export class HistoriasClinicasComponent implements OnInit {
    private fb = inject(FormBuilder);
    private cdr = inject(ChangeDetectorRef);
    private historiaService = inject(HistoriaClinicaService);
    private pacienteService = inject(PacienteService);
    private authService = inject(AuthService);

    pacientes: Paciente[] = [];
    pacientesFiltrados: Paciente[] = [];
    pacienteSeleccionado: Paciente | null = null;
    historias: HistoriaClinica[] = [];
    paginaActual = 1;
    readonly registrosPorPagina = 10;
    totalPaginas = 1;
    totalRegistros = 0;

    cargando = true;
    guardando = false;
    modalAbierto = false;
    filtroBusqueda = '';

    historiaForm = this.fb.nonNullable.group({
        idPaciente: [0, [Validators.required, Validators.min(1)]],
        pesoKg: [null as number | null, [Validators.min(0)]],
        temperatura: [null as number | null],
        frecuenciaCardiaca: [null as number | null],
        diagnostico: ['', [Validators.required, Validators.maxLength(500)]],
        tratamiento: ['', [Validators.maxLength(500)]],
        observaciones: ['', [Validators.maxLength(500)]]
    });

    ngOnInit(): void {
        this.cargarDatosIniciales();
    }

    cargarDatosIniciales(): void {
        this.cargando = true;
        this.pacienteService.obtenerTodos().subscribe({
            next: (pacientes: Paciente[]) => {
                this.pacientes = pacientes;
                this.pacientesFiltrados = [...this.pacientes];
                if (this.pacientes.length > 0) {
                    this.seleccionarPacientePorId(this.pacientes[0].idPaciente);
                } else {
                    this.cargando = false;
                    this.cdr.detectChanges();
                }
            },
            error: (err) => {
                console.error('Error al cargar pacientes:', err);
                this.cargando = false;
                this.cdr.detectChanges();
            }
        });
    }

    filtrarPacientes(): void {
        const term = this.filtroBusqueda.trim().toLowerCase();
        if (!term) {
            this.pacientesFiltrados = [...this.pacientes];
        } else {
            this.pacientesFiltrados = this.pacientes.filter(p =>
                p.nombre.toLowerCase().includes(term) ||
                (p.propietarioNombreCompleto && p.propietarioNombreCompleto.toLowerCase().includes(term))
            );
        }
    }

    onSelectChange(event: Event): void {
        const target = event.target as HTMLSelectElement;
        if (target && target.value) {
            this.seleccionarPacientePorId(Number(target.value));
        }
    }

    seleccionarPacientePorId(idPaciente: number): void {
        const encontrado = this.pacientes.find(p => p.idPaciente === idPaciente);
        if (!encontrado) return;

        this.pacienteSeleccionado = encontrado;
        this.paginaActual = 1;
        this.cargarHistorias();
    }

    cargarHistorias(): void {
        const paciente = this.pacienteSeleccionado;
        if (!paciente) return;

        this.cargando = true;
        this.historiaService.obtenerPaginadoPorPaciente(
            paciente.idPaciente,
            this.paginaActual,
            this.registrosPorPagina
        ).subscribe({
            next: (respuesta: PaginatedResponse<HistoriaClinica>) => {
                this.historias = respuesta.items ?? [];
                this.totalRegistros = respuesta.totalCount ?? 0;
                this.totalPaginas = Math.max(1, respuesta.totalPages ?? 1);
                this.paginaActual = respuesta.pageNumber ?? this.paginaActual;
                this.cargando = false;
                this.cdr.detectChanges();
            },
            error: (err) => {
                console.error('Error al obtener historias clínicas:', err);
                this.historias = [];
                this.totalRegistros = 0;
                this.totalPaginas = 1;
                this.cargando = false;
                this.cdr.detectChanges();
            }
        });
    }

    cambiarPagina(pagina: number): void {
        if (pagina >= 1 && pagina <= this.totalPaginas && pagina !== this.paginaActual) {
            this.paginaActual = pagina;
            this.cargarHistorias();
        }
    }

    get ultimoRegistro(): number {
        return Math.min(this.paginaActual * this.registrosPorPagina, this.totalRegistros);
    }

    abrirModal(): void {
        if (!this.pacienteSeleccionado) return;
        this.historiaForm.reset({
            idPaciente: this.pacienteSeleccionado.idPaciente,
            pesoKg: null,
            temperatura: null,
            frecuenciaCardiaca: null,
            diagnostico: '',
            tratamiento: '',
            observaciones: ''
        });
        this.modalAbierto = true;
    }

    cerrarModal(): void {
        this.modalAbierto = false;
    }

    guardarHistoria(): void {
        if (this.historiaForm.invalid || this.guardando) {
            this.historiaForm.markAllAsTouched();
            return;
        }

        const usuarioActual = this.authService.obtenerUsuario();
        if (!usuarioActual) {
            console.error('No hay usuario autenticado');
            return;
        }

        this.guardando = true;
        const val = this.historiaForm.getRawValue();

        const request: HistoriaClinicaCreateRequest = {
            idPaciente: Number(val.idPaciente),
            idVeterinario: usuarioActual.idUsuario,
            fechaAtencion: new Date().toISOString(),
            pesoKg: val.pesoKg ? Number(val.pesoKg) : null,
            temperatura: val.temperatura ? Number(val.temperatura) : null,
            frecuenciaCardiaca: val.frecuenciaCardiaca ? Number(val.frecuenciaCardiaca) : null,
            diagnostico: val.diagnostico.trim(),
            tratamiento: val.tratamiento?.trim() || null,
            observaciones: val.observaciones?.trim() || null
        };

        this.historiaService.crear(request).subscribe({
            next: () => {
                this.guardando = false;
                this.cerrarModal();
                if (this.pacienteSeleccionado) {
                    this.paginaActual = 1;
                    this.cargarHistorias();
                }
                this.cdr.detectChanges();
            },
            error: (err) => {
                console.error('Error al guardar historia clínica:', err);
                this.guardando = false;
                alert('No se pudo registrar la historia clínica.');
                this.cdr.detectChanges();
            }
        });
    }
}