import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { procesosApi } from '../api/procesosApi';
import type { ProcesoFiltros, ProcesoCreate } from '../types/proceso';

export function useProcesos(filtros: ProcesoFiltros = {}) {
  return useQuery({
    queryKey: ['procesos', filtros],
    queryFn: () => procesosApi.listar(filtros),
  });
}

export function useProceso(id: string) {
  return useQuery({
    queryKey: ['proceso', id],
    queryFn: () => procesosApi.obtenerPorId(id),
    enabled: !!id,
  });
}

export function useCrearProceso() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (proceso: ProcesoCreate) => procesosApi.crear(proceso),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['procesos'] });
      qc.invalidateQueries({ queryKey: ['dashboard'] });
    },
  });
}

export function useActualizarProceso() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, proceso }: { id: string; proceso: ProcesoCreate }) =>
      procesosApi.actualizar(id, proceso),
    onSuccess: (_data, variables) => {
      qc.invalidateQueries({ queryKey: ['procesos'] });
      qc.invalidateQueries({ queryKey: ['proceso', variables.id] });
      qc.invalidateQueries({ queryKey: ['dashboard'] });
    },
  });
}

export function useEliminarProceso() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => procesosApi.eliminar(id),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['procesos'] });
      qc.invalidateQueries({ queryKey: ['dashboard'] });
    },
  });
}
