import api from './axiosConfig';
import type { Nota, NotaCreate } from '../types/nota';

export const notasApi = {
  listarPorProceso: async (procesoId: string): Promise<Nota[]> => {
    const { data } = await api.get(`/procesos/${procesoId}/notas`);
    return data;
  },

  crear: async (procesoId: string, nota: NotaCreate): Promise<Nota> => {
    const { data } = await api.post(`/procesos/${procesoId}/notas`, nota);
    return data;
  },

  actualizar: async (procesoId: string, notaId: string, nota: NotaCreate): Promise<Nota> => {
    const { data } = await api.put(`/procesos/${procesoId}/notas/${notaId}`, nota);
    return data;
  },

  eliminar: async (procesoId: string, notaId: string): Promise<void> => {
    await api.delete(`/procesos/${procesoId}/notas/${notaId}`);
  },
};
