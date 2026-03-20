import type { Pago } from './pago';
import type { Nota } from './nota';
import type { Recordatorio } from './recordatorio';

export interface Comision {
  persona: string;
  porcentaje: string;
}

export interface CampoPropio {
  nombre: string;
  valor: string;
}

export interface ProcesoListItem {
  id: string;
  fecha: string;
  demandante: string;
  demandado: string;
  radicado: string;
  ciudad: string;
  claseProceso: string;
  estadoActual: string | null;
  procesoIngresadoPor: string | null;
  terminado: boolean;
  cantidadPagos: number;
  cantidadNotas: number;
  recordatoriosPendientes: number;
}

export interface ProcesoDetalle {
  id: string;
  fecha: string;
  demandante: string;
  demandado: string;
  radicado: string;
  juzgado: string;
  ciudad: string;
  claseProceso: string;
  representamos: string | null;
  procesoIngresadoPor: string | null;
  honorarios: string | null;
  comisiones: Comision[];
  estadoActual: string | null;
  camposGlobales: Record<string, string>;
  camposPropios: CampoPropio[];
  terminado: boolean;
  notaTerminacion: string | null;
  fechaTerminacion: string | null;
  fechaCreacion: string;
  fechaActualizacion: string;
  pagos: Pago[];
  notas: Nota[];
  recordatorios: Recordatorio[];
}

export interface ProcesoCreate {
  fecha: string;
  demandante: string;
  demandado: string;
  radicado: string;
  juzgado: string;
  ciudad: string;
  claseProceso: string;
  representamos?: string;
  procesoIngresadoPor?: string;
  honorarios?: string;
  comisiones: Comision[];
  estadoActual?: string;
  camposGlobales?: Record<string, string>;
  camposPropios?: CampoPropio[];
  terminado?: boolean;
  notaTerminacion?: string;
}

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  pagina: number;
  porPagina: number;
  totalPaginas: number;
}

export interface ProcesoFiltros {
  busqueda?: string;
  ciudad?: string;
  claseProceso?: string;
  estado?: string;
  ingresadoPor?: string;
  ordenarPor?: string;
  ordenDescendente?: boolean;
  pagina?: number;
  porPagina?: number;
}
