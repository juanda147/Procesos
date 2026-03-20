export interface Recordatorio {
  id: string;
  procesoId: string;
  titulo: string;
  descripcion: string | null;
  fechaVencimiento: string;
  completado: boolean;
  fechaCreacion: string;
  correoNotificacion: string | null;
  demandanteDelProceso?: string;
  radicadoDelProceso?: string;
}

export interface RecordatorioCreate {
  titulo: string;
  descripcion?: string;
  fechaVencimiento: string;
  correoNotificacion?: string;
}
