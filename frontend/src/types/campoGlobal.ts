export interface CampoGlobal {
  id: string;
  nombre: string;
  tipo: string; // "texto" | "fecha" | "numero"
  orden: number;
}

export interface CampoGlobalCreate {
  nombre: string;
  tipo: string;
}

export interface CampoGlobalUpdate {
  nombre: string;
  tipo: string;
}
