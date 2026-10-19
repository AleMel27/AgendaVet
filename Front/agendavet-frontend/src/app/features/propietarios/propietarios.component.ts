import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { PropietarioService } from '../../core/services/propietario.service';
import { Propietario } from '../../core/models/propietario.model';

@Component({
    selector: 'app-propietarios',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './propietarios.component.html',
    styleUrl: './propietarios.component.css'
})
export class PropietariosComponent implements OnInit {
    private fb = inject(FormBuilder);
    private cdr = inject(ChangeDetectorRef);
    private propietarioService = inject(PropietarioService);

    propietarios: Propietario[] = [];
    cargando = true;
    guardando = false;
    errorMensaje = '';
    exitoMensaje = '';

    // Control de Modales
    modalFormularioAbierto = false;
    modoEdicion = false;
    propietarioEditandoId: number | null = null;

    // Paginación
    paginaActual = 1;
    elementosPorPagina = 10;
    totalPaginas = 1;
    totalRegistros = 0;

    // Formulario Reactivo
    propietarioForm = this.fb.nonNullable.group({
        nombre: ['', [Validators.required, Validators.maxLength(50)]],
        apellido: ['', [Validators.required, Validators.maxLength(50)]],
        dniCe: ['', [Validators.required, Validators.maxLength(15)]],
        telefono: ['', [Validators.required, Validators.maxLength(15)]],
        email: ['', [Validators.email, Validators.maxLength(100)]],
        direccion: ['', [Validators.maxLength(200)]]
    });

    ngOnInit(): void {
        this.cargarPropietarios();
    }

    cargarPropietarios(): void {
        this.cargando = true;
        this.propietarioService.getPropietarios(this.paginaActual, this.elementosPorPagina).subscribe({
            next: (res: any) => {
                if (Array.isArray(res)) {
                    this.totalRegistros = res.length;
                    this.totalPaginas = Math.ceil(this.totalRegistros / this.elementosPorPagina) || 1;
                    const inicio = (this.paginaActual - 1) * this.elementosPorPagina;
                    this.propietarios = res.slice(inicio, inicio + this.elementosPorPagina);
                } else if (res && res.items) {
                    this.propietarios = res.items;
                    this.totalPaginas = res.totalPages || 1;
                    this.totalRegistros = res.totalRecords || res.items.length;
                } else if (res) {
                    this.propietarios = res.data || res.resultado || [];
                    this.totalRegistros = this.propietarios.length;
                    this.totalPaginas = 1;
                }
                this.cargando = false;
                this.cdr.detectChanges();
            },
            error: (err) => {
                console.error('Error al cargar:', err);
                this.cargando = false;
                this.cdr.detectChanges();
            }
        });
    }

    abrirModalCrear(): void {
        this.modoEdicion = false;
        this.propietarioEditandoId = null;
        this.propietarioForm.reset();
        this.modalFormularioAbierto = true;
    }

    abrirModalEditar(p: Propietario): void {
        this.modoEdicion = true;
        this.propietarioEditandoId = p.idPropietario;
        this.propietarioForm.patchValue({
            nombre: p.nombre,
            apellido: p.apellido,
            dniCe: p.dniCe,
            telefono: p.telefono,
            email: p.email || '',
            direccion: p.direccion || ''
        });
        this.modalFormularioAbierto = true;
    }

    cerrarModalFormulario(): void {
        this.modalFormularioAbierto = false;
        this.propietarioForm.reset();
    }

    guardarPropietario(): void {
        if (this.propietarioForm.invalid || this.guardando) {
            this.propietarioForm.markAllAsTouched();
            return;
        }

        this.guardando = true;
        const data = this.propietarioForm.getRawValue();

        if (this.modoEdicion && this.propietarioEditandoId) {
            this.propietarioService.actualizar(this.propietarioEditandoId, data).subscribe({
                next: () => {
                    this.guardando = false;
                    this.cerrarModalFormulario();
                    this.mostrarMensaje('¡Propietario actualizado con éxito!');
                    this.cargarPropietarios();
                },
                error: (err) => {
                    console.error('Error al actualizar:', err);
                    this.guardando = false;
                    alert('No se pudo actualizar el propietario.');
                    this.cdr.detectChanges();
                }
            });
        } else {
            this.propietarioService.crear(data).subscribe({
                next: () => {
                    this.guardando = false;
                    this.cerrarModalFormulario();
                    this.mostrarMensaje('¡Propietario registrado con éxito!');
                    this.cargarPropietarios();
                },
                error: (err) => {
                    console.error('Error al crear:', err);
                    this.guardando = false;
                    alert('No se pudo registrar el propietario. Verifica los datos.');
                    this.cdr.detectChanges();
                }
            });
        }
    }

    darDeBaja(id: number): void {
        if (!confirm('¿Estás seguro de eliminar este propietario?')) return;

        this.propietarioService.eliminarPropietario(id).subscribe({
            next: () => {
                this.mostrarMensaje('Propietario eliminado correctamente.');
                this.cargarPropietarios();
            },
            error: (err) => {
                console.error('Error al eliminar:', err);
                alert('No se pudo eliminar el propietario. Es posible que tenga pacientes vinculados.');
                this.cdr.detectChanges();
            }
        });
    }

    cambiarPagina(nuevaPagina: number): void {
        if (nuevaPagina >= 1 && nuevaPagina <= this.totalPaginas) {
            this.paginaActual = nuevaPagina;
            this.cargarPropietarios();
        }
    }

    mostrarMensaje(msg: string): void {
        this.exitoMensaje = msg;
        setTimeout(() => {
            this.exitoMensaje = '';
            this.cdr.detectChanges();
        }, 3500);
    }
}