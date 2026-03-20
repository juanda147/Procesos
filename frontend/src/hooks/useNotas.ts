import { useMutation, useQueryClient } from '@tanstack/react-query';
import { notasApi } from '../api/notasApi';
import type { NotaCreate } from '../types/nota';

export function useCrearNota(procesoId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (nota: NotaCreate) => notasApi.crear(procesoId, nota),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['proceso', procesoId] });
    },
  });
}

export function useActualizarNota(procesoId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, nota }: { id: string; nota: NotaCreate }) =>
      notasApi.actualizar(procesoId, id, nota),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['proceso', procesoId] });
    },
  });
}

export function useEliminarNota(procesoId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => notasApi.eliminar(procesoId, id),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['proceso', procesoId] });
    },
  });
}
