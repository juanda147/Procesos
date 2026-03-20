import { useMutation, useQueryClient } from '@tanstack/react-query';
import { pagosApi } from '../api/pagosApi';
import type { PagoCreate } from '../types/pago';

export function useCrearPago(procesoId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (pago: PagoCreate) => pagosApi.crear(procesoId, pago),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['proceso', procesoId] });
      qc.invalidateQueries({ queryKey: ['dashboard'] });
    },
  });
}

export function useActualizarPago(procesoId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, pago }: { id: string; pago: PagoCreate }) =>
      pagosApi.actualizar(procesoId, id, pago),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['proceso', procesoId] });
    },
  });
}

export function useEliminarPago(procesoId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => pagosApi.eliminar(procesoId, id),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['proceso', procesoId] });
      qc.invalidateQueries({ queryKey: ['dashboard'] });
    },
  });
}
