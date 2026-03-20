import api from './axiosConfig';

export const importExportApi = {
  importar: async (archivo: File): Promise<{ importados: number; errores: string[] }> => {
    const formData = new FormData();
    formData.append('archivo', archivo);
    const { data } = await api.post('/importar', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return data;
  },

  exportar: async (ids?: number[]): Promise<Blob> => {
    const params = ids?.length ? `?ids=${ids.join(',')}` : '';
    const { data } = await api.get(`/exportar${params}`, { responseType: 'blob' });
    return data;
  },
};
