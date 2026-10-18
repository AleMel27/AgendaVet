
export interface LoginRequest {
  email: string;
  password: string;
}

export interface UsuarioAuth {
  idUsuario: number;
  idRol: number;
  rolNombre: string;
  nombre: string;
  apellido: string;
  email: string;
  telefono?: string | null;
  activo: boolean;
  fechaRegistro?: string;
}

export interface LoginResponse {
  token: string;
  usuario: UsuarioAuth;
}
