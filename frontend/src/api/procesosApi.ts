import api from './axiosConfig';
import type { ProcesoListItem, ProcesoDetalle, ProcesoCreate, PaginatedResult, ProcesoFiltros } from '../types/proceso';

export const procesosApi = {
  listar: async (filtros: ProcesoFiltros = {}): Promise<PaginatedResult<ProcesoListItem>> => {
    const params = new URLSearchParams();
    if (filtros.busqueda) params.set('busqueda', filtros.busqueda);
    if (filtros.ciudad) params.set('ciudad', filtros.ciudad);
    if (filtros.claseProceso) params.set('claseProceso', filtros.claseProceso);
    if (filtros.estado) params.set('estado', filtros.estado);
    if (filtros.ingresadoPor) params.set('ingresadoPor', filtros.ingresadoPor);
    if (filtros.ordenarPor) params.set('ordenarPor', filtros.ordenarPor);
    if (filtros.ordenDescendente !== undefined) params.set('ordenDescendente', String(filtros.ordenDescendente));
    if (filtros.pagina) params.set('pagina', String(filtros.pagina));
    if (filtros.porPagina) params.set('porPagina', String(filtros.porPagina));
    const { data } = await api.get(`/procesos?${params.toString()}`);
    return data;
  },

  obtenerPorId: async (id: string): Promise<ProcesoDetalle> => {
    const { data } = await api.get(`/procesos/${id}`);
    return data;
  },

  crear: async (proceso: ProcesoCreate): Promise<ProcesoDetalle> => {
    const { data } = await api.post('/procesos', proceso);
    return data;
  },

  actualizar: async (id: string, proceso: ProcesoCreate): Promise<ProcesoDetalle> => {
    const { data } = await api.put(`/procesos/${id}`, proceso);
    return data;
  },

  eliminar: async (id: string): Promise<void> => {
    await api.delete(`/procesos/${id}`);
  },
};
