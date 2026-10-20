import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PropietarioService, PropietarioCreateRequest } from '../../core/services/propietario.service';
import { Propietario } from '../../core/models/catalogo.model';
import { PaginatedResponse } from '../../core/models/catalogo.model';

@Component({
    selector: 'app-propietarios',
    standalone: true,
    imports: [CommonModule, FormsModule, ReactiveFormsModule],
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

    // Búsqueda
    terminoBusqueda = '';

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
        const buscar = this.terminoBusqueda.trim() || undefined;
        this.propietarioService.obtenerPaginado(this.paginaActual, this.elementosPorPagina, buscar).subscribe({
            next: (res: PaginatedResponse<Propietario>) => {
                this.propietarios = res.items;
                this.totalPaginas = res.totalPages || 1;
                this.totalRegistros = res.totalCount;
                this.cargando = false;
                this.cdr.detectChanges();
            },
            error: (err) => {
                console.error('Error al cargar propietarios:', err);
                this.errorMensaje = 'No se pudieron cargar los propietarios.';
                this.cargando = false;
                this.cdr.detectChanges();
            }
        });
    }

    buscar(): void {
        this.paginaActual = 1;
        this.cargarPropietarios();
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
        const data = this.propietarioForm.getRawValue() as PropietarioCreateRequest;

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
                    this.errorMensaje = 'No se pudo actualizar el propietario.';
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
                    console.error('Error al crear propietario:', err);
                    this.guardando = false;
                    this.errorMensaje = 'No se pudo registrar el propietario. Verifica los datos.';
                    this.cdr.detectChanges();
                }
            });
        }
    }

    darDeBaja(id: number): void {
        if (!confirm('¿Estás seguro de eliminar este propietario?')) return;

        this.propietarioService.eliminar(id).subscribe({
            next: () => {
                this.mostrarMensaje('Propietario eliminado correctamente.');
                this.cargarPropietarios();
            },
            error: (err) => {
                console.error('Error al eliminar:', err);
                this.errorMensaje = 'No se pudo eliminar el propietario. Es posible que tenga pacientes vinculados.';
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
        this.errorMensaje = '';
        setTimeout(() => {
            this.exitoMensaje = '';
            this.cdr.detectChanges();
        }, 3500);
    }
}