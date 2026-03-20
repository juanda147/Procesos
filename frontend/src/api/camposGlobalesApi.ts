import api from './axiosConfig';
import type { CampoGlobal, CampoGlobalCreate, CampoGlobalUpdate } from '../types/campoGlobal';

export const camposGlobalesApi = {
  listar: async (): Promise<CampoGlobal[]> => {
    const { data } = await api.get('/campos-globales');
    return data;
  },

  crear: async (campo: CampoGlobalCreate): Promise<CampoGlobal> => {
    const { data } = await api.post('/campos-globales', campo);
    return data;
  },

  actualizar: async (id: string, campo: CampoGlobalUpdate): Promise<CampoGlobal> => {
    const { data } = await api.put(`/campos-globales/${id}`, campo);
    return data;
  },

  eliminar: async (id: string): Promise<void> => {
    await api.delete(`/campos-globales/${id}`);
  },
};
