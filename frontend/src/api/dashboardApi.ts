import api from './axiosConfig';
import type { Dashboard, FiltrosDisponibles } from '../types/dashboard';

export const dashboardApi = {
  obtenerEstadisticas: async (): Promise<Dashboard> => {
    const { data } = await api.get('/dashboard');
    return data;
  },

  obtenerFiltros: async (): Promise<FiltrosDisponibles> => {
    const { data } = await api.get('/dashboard/filtros');
    return data;
  },
};
