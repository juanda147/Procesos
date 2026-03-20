export interface Pago {
  id: string;
  procesoId: string;
  fecha: string;
  monto: number;
  concepto: string | null;
  metodoPago: string | null;
  fechaCreacion: string;
}

export interface PagoCreate {
  fecha: string;
  monto: number;
  concepto?: string;
  metodoPago?: string;
}
