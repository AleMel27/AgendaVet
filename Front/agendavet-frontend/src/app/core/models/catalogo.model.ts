export interface Especie {
  idEspecie: number;
  nombre: string;
}

export interface Raza {
  idRaza: number;
  idEspecie: number;
  nombre: string;
  especieNombre?: string;
}

export interface Servicio {
  idServicio: number;
  nombre: string;
  descripcion?: string | null;
  precio: number;
  duracionEstimadaMin: number;
  activo: boolean;
}

export interface Propietario {
  idPropietario: number;
  nombre: string;
  apellido: string;
  dniCe: string;
  telefono: string;
  email?: string | null;
  direccion?: string | null;
  fechaRegistro?: string;
}

export interface Veterinario {
  idUsuario: number;
  idRol: number;
  rolNombre: string;
  nombre: string;
  apellido: string;
  email: string;
  telefono?: string | null;
  activo: boolean;
}

export interface PaginatedResponse<T> {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  items: T[];
}
