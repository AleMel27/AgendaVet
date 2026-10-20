export interface Usuario {
  idUsuario: number;
  idRol: number;
  rolNombre: string;
  nombre: string;
  apellido: string;
  email: string;
  telefono?: string | null;
  activo: boolean;
  fechaRegistro: string;
}

export interface UsuarioCreateRequest {
  idRol: number;
  nombre: string;
  apellido: string;
  email: string;
  password: string;
  telefono?: string | null;
  activo: boolean;
}

export interface UsuarioUpdateRequest {
  idRol: number;
  nombre: string;
  apellido: string;
  telefono?: string | null;
  activo: boolean;
}

export interface Rol {
  idRol: number;
  nombre: string;
  descripcion?: string | null;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}
