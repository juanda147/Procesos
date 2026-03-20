import api from './axiosConfig';
import type { Pago, PagoCreate } from '../types/pago';

export const pagosApi = {
  listarPorProceso: async (procesoId: string): Promise<Pago[]> => {
    const { data } = await api.get(`/procesos/${procesoId}/pagos`);
    return data;
  },

  crear: async (procesoId: string, pago: PagoCreate): Promise<Pago> => {
    const { data } = await api.post(`/procesos/${procesoId}/pagos`, pago);
    return data;
  },

  actualizar: async (procesoId: string, pagoId: string, pago: PagoCreate): Promise<Pago> => {
    const { data } = await api.put(`/procesos/${procesoId}/pagos/${pagoId}`, pago);
    return data;
  },

  eliminar: async (procesoId: string, pagoId: string): Promise<void> => {
    await api.delete(`/procesos/${procesoId}/pagos/${pagoId}`);
  },
};
