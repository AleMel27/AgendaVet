export interface Propietario {
    idPropietario: number;
    nombre: string;
    apellido: string;
    dniCe: string;
    telefono: string;
    email?: string;
    direccion?: string;
    fechaRegistro?: string;
}

export interface RespuestaPropietariosPaginada {
    items: Propietario[];
    totalRecords: number;
    totalPages: number;
    currentPage: number;
}