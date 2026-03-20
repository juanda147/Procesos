import api from './axiosConfig';
import type { Recordatorio, RecordatorioCreate } from '../types/recordatorio';

export const recordatoriosApi = {
  listarTodos: async (filtro?: string): Promise<Recordatorio[]> => {
    const params = filtro ? `?filtro=${filtro}` : '';
    const { data } = await api.get(`/recordatorios${params}`);
    return data;
  },

  listarPorProceso: async (procesoId: string): Promise<Recordatorio[]> => {
    const { data } = await api.get(`/procesos/${procesoId}/recordatorios`);
    return data;
  },

  crear: async (procesoId: string, recordatorio: RecordatorioCreate): Promise<Recordatorio> => {
    const { data } = await api.post(`/procesos/${procesoId}/recordatorios`, recordatorio);
    return data;
  },

  actualizar: async (procesoId: string, recordatorioId: string, recordatorio: RecordatorioCreate): Promise<Recordatorio> => {
    const { data } = await api.put(`/procesos/${procesoId}/recordatorios/${recordatorioId}`, recordatorio);
    return data;
  },

  completar: async (procesoId: string, recordatorioId: string): Promise<void> => {
    await api.put(`/procesos/${procesoId}/recordatorios/${recordatorioId}/completar`);
  },

  eliminar: async (procesoId: string, recordatorioId: string): Promise<void> => {
    await api.delete(`/procesos/${procesoId}/recordatorios/${recordatorioId}`);
  },
};
