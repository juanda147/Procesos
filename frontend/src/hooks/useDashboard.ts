import { useQuery } from '@tanstack/react-query';
import { dashboardApi } from '../api/dashboardApi';

export function useDashboard() {
  return useQuery({
    queryKey: ['dashboard'],
    queryFn: () => dashboardApi.obtenerEstadisticas(),
  });
}

export function useFiltrosDisponibles() {
  return useQuery({
    queryKey: ['filtros'],
    queryFn: () => dashboardApi.obtenerFiltros(),
  });
}
