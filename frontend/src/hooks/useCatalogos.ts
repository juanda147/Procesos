import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { catalogosApi } from '../api/catalogosApi';
import type { CatalogoCreate } from '../types/catalogo';

export function useCatalogos(tipo: string) {
  return useQuery({
    queryKey: ['catalogos', tipo],
    queryFn: () => catalogosApi.listarPorTipo(tipo),
    enabled: !!tipo,
  });
}

export function useCrearCatalogo() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (catalogo: CatalogoCreate) => catalogosApi.crear(catalogo),
    onSuccess: (_data, variables) => {
      qc.invalidateQueries({ queryKey: ['catalogos', variables.tipo] });
    },
  });
}

export function useActualizarCatalogo() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, valor, tipo: _tipo }: { id: string; valor: string; tipo: string }) =>
      catalogosApi.actualizar(id, { valor }),
    onSuccess: (_data, variables) => {
      qc.invalidateQueries({ queryKey: ['catalogos', variables.tipo] });
    },
  });
}

export function useEliminarCatalogo() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, tipo: _tipo }: { id: string; tipo: string }) => catalogosApi.eliminar(id),
    onSuccess: (_data, variables) => {
      qc.invalidateQueries({ queryKey: ['catalogos', variables.tipo] });
    },
  });
}
