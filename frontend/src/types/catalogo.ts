export interface Catalogo {
  id: string;
  tipo: string;
  valor: string;
  orden: number;
}

export interface CatalogoCreate {
  tipo: string;
  valor: string;
}

export interface CatalogoUpdate {
  valor: string;
}
