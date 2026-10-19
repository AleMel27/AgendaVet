import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { HistoriaClinicaService } from '../../core/services/historia-clinica.service';
import { PacienteService } from '../../core/services/paciente.service';
import { HistoriaClinica } from '../../core/models/historia-clinica.model';
import { Paciente } from '../../core/models/paciente.model';

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

    pacientes: Paciente[] = [];
    pacientesFiltrados: Paciente[] = [];
    pacienteSeleccionado: Paciente | null = null;
    historias: HistoriaClinica[] = [];

    cargando = true;
    guardando = false;
    modalAbierto = false;
    filtroBusqueda = '';

    historiaForm = this.fb.nonNullable.group({
        idPaciente: [0, [Validators.required, Validators.min(1)]],
        peso: [0, [Validators.required, Validators.min(0)]],
        temperatura: [38, [Validators.required]],
        frecuenciaCardiaca: [80],
        diagnostico: ['', [Validators.required, Validators.maxLength(300)]],
        tratamiento: ['', [Validators.required, Validators.maxLength(300)]],
        medicamentos: [''],
        observaciones: ['', [Validators.maxLength(300)]]
    });

    ngOnInit(): void {
        this.cargarDatosIniciales();
    }

    cargarDatosIniciales(): void {
        this.cargando = true;
        this.pacienteService.obtenerTodos().subscribe({
            next: (res: any) => {
                this.pacientes = Array.isArray(res) ? res : (res?.items || res?.data || []);
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
        this.cargando = true;

        this.historiaService.obtenerPorPaciente(idPaciente).subscribe({
            next: (data) => {
                this.historias = Array.isArray(data) ? data : [];
                this.cargando = false;
                this.cdr.detectChanges();
            },
            error: (err) => {
                console.error('Error al obtener historias clínicas:', err);
                this.historias = [];
                this.cargando = false;
                this.cdr.detectChanges();
            }
        });
    }

    abrirModal(): void {
        if (!this.pacienteSeleccionado) return;
        this.historiaForm.reset({
            idPaciente: this.pacienteSeleccionado.idPaciente,
            peso: 15,
            temperatura: 38.5,
            frecuenciaCardiaca: 90,
            diagnostico: '',
            tratamiento: '',
            medicamentos: '',
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

        this.guardando = true;
        const val = this.historiaForm.getRawValue();

        const request: HistoriaClinica = {
            idPaciente: Number(val.idPaciente),
            idVeterinario: 1,
            fechaAtencion: new Date().toISOString(),
            pesoKg: Number(val.peso),
            temperatura: Number(val.temperatura),
            frecuenciaCardiaca: Number(val.frecuenciaCardiaca),
            diagnostico: val.diagnostico.trim(),
            tratamiento: val.tratamiento.trim(),
            observaciones: val.observaciones?.trim()
        };

        this.historiaService.crear(request).subscribe({
            next: () => {
                this.guardando = false;
                this.cerrarModal();
                if (this.pacienteSeleccionado) {
                    this.seleccionarPacientePorId(this.pacienteSeleccionado.idPaciente);
                }
                this.cdr.detectChanges();
            },
            error: (err) => {
                console.error('Error al guardar historia:', err);
                this.guardando = false;
                alert('No se pudo registrar la historia clínica.');
                this.cdr.detectChanges();
            }
        });
    }
}