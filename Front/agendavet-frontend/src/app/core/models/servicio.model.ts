export interface Servicio {
  idServicio: number;
  nombre: string;
  descripcion?: string | null;
  precio: number;
  duracionEstimadaMin: number;
  activo: boolean;
}

export interface ServicioRequest {
  nombre: string;
  descripcion?: string | null;
  precio: number;
  duracionEstimadaMin: number;
  activo: boolean;
}
