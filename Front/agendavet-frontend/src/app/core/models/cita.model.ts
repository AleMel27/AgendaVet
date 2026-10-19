export interface Cita {
  idCita: number;
  idPaciente: number;
  pacienteNombre: string;
  idVeterinario: number;
  veterinarioNombre: string;
  idServicio: number;
  servicioNombre: string;
  fechaHora: string;
  motivo?: string | null;
  estado: string; // 'Pendiente' | 'Confirmada' | 'EnAtencion' | 'Completada' | 'Cancelada'
  fechaCreacion: string;
}

export interface CitaCrearRequest {
  idPaciente: number;
  idVeterinario: number;
  idServicio: number;
  fechaHora: string;
  motivo?: string | null;
  notasAgendamiento?: string | null;
}

export interface CitaActualizarRequest {
  idVeterinario: number;
  idServicio: number;
  fechaHora: string;
  motivo?: string | null;
  estado: string;
}

export type EstadoCitaTipo = 'Pendiente' | 'Confirmada' | 'EnAtencion' | 'Completada' | 'Cancelada';
