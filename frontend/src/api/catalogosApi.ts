import api from './axiosConfig';
import type { Catalogo, CatalogoCreate, CatalogoUpdate } from '../types/catalogo';

export const catalogosApi = {
  listarPorTipo: async (tipo: string): Promise<Catalogo[]> => {
    const { data } = await api.get(`/catalogos?tipo=${tipo}`);
    return data;
  },

  crear: async (catalogo: CatalogoCreate): Promise<Catalogo> => {
    const { data } = await api.post('/catalogos', catalogo);
    return data;
  },

  actualizar: async (id: string, catalogo: CatalogoUpdate): Promise<Catalogo> => {
    const { data } = await api.put(`/catalogos/${id}`, catalogo);
    return data;
  },

  eliminar: async (id: string): Promise<void> => {
    await api.delete(`/catalogos/${id}`);
  },
};
