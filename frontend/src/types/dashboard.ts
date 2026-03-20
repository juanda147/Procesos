import type { Recordatorio } from './recordatorio';

export interface Dashboard {
  totalProcesos: number;
  procesosActivos: number;
  recordatoriosPendientes: number;
  recordatoriosVencidos: number;
  procesosPorTipo: ConteoItem[];
  procesosPorCiudad: ConteoItem[];
  procesosPorIngresadoPor: ConteoItem[];
  proximosRecordatorios: Recordatorio[];
}

export interface ConteoItem {
  nombre: string;
  cantidad: number;
}

export interface FiltrosDisponibles {
  ciudades: string[];
  clasesProceso: string[];
  ingresadoPor: string[];
  estados: string[];
}
