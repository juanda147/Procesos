import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { recordatoriosApi } from '../api/recordatoriosApi';
import type { RecordatorioCreate } from '../types/recordatorio';

export function useRecordatorios(filtro?: string) {
  return useQuery({
    queryKey: ['recordatorios', filtro],
    queryFn: () => recordatoriosApi.listarTodos(filtro),
  });
}

export function useCrearRecordatorio(procesoId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (recordatorio: RecordatorioCreate) => recordatoriosApi.crear(procesoId, recordatorio),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['proceso', procesoId] });
      qc.invalidateQueries({ queryKey: ['recordatorios'] });
      qc.invalidateQueries({ queryKey: ['dashboard'] });
    },
  });
}

export function useActualizarRecordatorio(procesoId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, recordatorio }: { id: string; recordatorio: RecordatorioCreate }) =>
      recordatoriosApi.actualizar(procesoId, id, recordatorio),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['proceso', procesoId] });
      qc.invalidateQueries({ queryKey: ['recordatorios'] });
      qc.invalidateQueries({ queryKey: ['dashboard'] });
    },
  });
}

export function useCompletarRecordatorio(procesoId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => recordatoriosApi.completar(procesoId, id),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['recordatorios'] });
      qc.invalidateQueries({ queryKey: ['dashboard'] });
      qc.invalidateQueries({ queryKey: ['proceso', procesoId] });
    },
  });
}

export function useEliminarRecordatorio(procesoId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => recordatoriosApi.eliminar(procesoId, id),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['recordatorios'] });
      qc.invalidateQueries({ queryKey: ['dashboard'] });
      qc.invalidateQueries({ queryKey: ['proceso', procesoId] });
    },
  });
}
