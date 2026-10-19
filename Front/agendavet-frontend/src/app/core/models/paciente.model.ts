export interface Paciente {
  idPaciente: number;
  idPropietario: number;
  propietarioNombreCompleto: string;
  idEspecie: number;
  especieNombre: string;
  idRaza?: number | null;
  razaNombre?: string | null;
  nombre: string;
  fechaNacimiento?: string | null;
  sexo: string; // 'Macho' | 'Hembra'
  color?: string | null;
  esterilizado: boolean;
  observaciones?: string | null;
  fechaRegistro: string;
}

export interface PacienteCrearRequest {
  idPropietario: number;
  idEspecie: number;
  idRaza?: number | null;
  nombre: string;
  sexo: string;
  fechaNacimiento?: string | null;
  color?: string | null;
  esterilizado: boolean;
  observaciones?: string | null;
}
