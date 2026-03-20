import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { camposGlobalesApi } from '../api/camposGlobalesApi';
import type { CampoGlobalCreate, CampoGlobalUpdate } from '../types/campoGlobal';

export function useCamposGlobales() {
  return useQuery({
    queryKey: ['camposGlobales'],
    queryFn: () => camposGlobalesApi.listar(),
  });
}

export function useCrearCampoGlobal() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (campo: CampoGlobalCreate) => camposGlobalesApi.crear(campo),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['camposGlobales'] });
    },
  });
}

export function useActualizarCampoGlobal() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, campo }: { id: string; campo: CampoGlobalUpdate }) =>
      camposGlobalesApi.actualizar(id, campo),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['camposGlobales'] });
    },
  });
}

export function useEliminarCampoGlobal() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => camposGlobalesApi.eliminar(id),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['camposGlobales'] });
    },
  });
}
