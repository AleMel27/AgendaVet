export interface HistoriaClinica {
    idHistoria?: number;
    idCita?: number | null;
    idPaciente: number;
    idVeterinario?: number;
    fechaAtencion: string;
    pesoKg?: number;
    peso?: number;
    temperatura?: number;
    frecuenciaCardiaca?: number;
    diagnostico: string;
    tratamiento?: string;
    observaciones?: string;
}