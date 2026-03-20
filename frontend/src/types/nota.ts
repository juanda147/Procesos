export interface Nota {
  id: string;
  procesoId: string;
  contenido: string;
  fechaCreacion: string;
  fechaActualizacion: string;
}

export interface NotaCreate {
  contenido: string;
}
