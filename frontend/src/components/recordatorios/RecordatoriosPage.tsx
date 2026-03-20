import { useState } from 'react';
import { Box, Typography, Tabs, Tab, Card, CardContent, Checkbox, IconButton, Chip, CircularProgress } from '@mui/material';
import { Delete, Email } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useRecordatorios } from '../../hooks/useRecordatorios';
import { recordatoriosApi } from '../../api/recordatoriosApi';
import { formatDate } from '../../utils/formatters';

const TABS = [
  { label: 'Pendientes', filtro: 'pendientes' },
  { label: 'Vencidos', filtro: 'vencidos' },
  { label: 'Completados', filtro: 'completados' },
];

export default function RecordatoriosPage() {
  const navigate = useNavigate();
  const [tabIndex, setTabIndex] = useState(0);
  const filtro = TABS[tabIndex].filtro;
  const { data: recordatorios, isLoading } = useRecordatorios(filtro);
  const qc = useQueryClient();

  const completar = useMutation({
    mutationFn: ({ procesoId, id }: { procesoId: string; id: string }) =>
      recordatoriosApi.completar(procesoId, id),
    onSuccess: (_data, variables) => {
      qc.invalidateQueries({ queryKey: ['recordatorios'] });
      qc.invalidateQueries({ queryKey: ['dashboard'] });
      qc.invalidateQueries({ queryKey: ['proceso', variables.procesoId] });
    },
  });

  const eliminar = useMutation({
    mutationFn: ({ procesoId, id }: { procesoId: string; id: string }) =>
      recordatoriosApi.eliminar(procesoId, id),
    onSuccess: (_data, variables) => {
      qc.invalidateQueries({ queryKey: ['recordatorios'] });
      qc.invalidateQueries({ queryKey: ['dashboard'] });
      qc.invalidateQueries({ queryKey: ['proceso', variables.procesoId] });
    },
  });

  return (
    <Box>
      <Typography variant="h5" fontWeight="bold" gutterBottom>Recordatorios</Typography>

      <Tabs value={tabIndex} onChange={(_e, v) => setTabIndex(v)} sx={{ mb: 2 }}>
        {TABS.map((t) => <Tab key={t.filtro} label={t.label} />)}
      </Tabs>

      {isLoading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}><CircularProgress /></Box>
      ) : !recordatorios?.length ? (
        <Typography color="text.secondary">No hay recordatorios en esta categoria</Typography>
      ) : (
        <Card>
          <CardContent>
            {recordatorios.map((r) => {
              const isOverdue = !r.completado && new Date(r.fechaVencimiento) < new Date();
              return (
                <Box key={r.id} sx={{ display: 'flex', alignItems: 'center', gap: 1, py: 1, borderBottom: '1px solid', borderColor: 'divider' }}>
                  <Checkbox checked={r.completado} onChange={() => completar.mutate({ procesoId: r.procesoId, id: r.id })} />
                  <Box sx={{ flex: 1, cursor: 'pointer' }} onClick={() => navigate(`/procesos/${r.procesoId}`)}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                      <Typography variant="body1" fontWeight="medium" sx={{ textDecoration: r.completado ? 'line-through' : 'none' }}>
                        {r.titulo}
                      </Typography>
                      {r.correoNotificacion && <Email sx={{ fontSize: 16, color: 'text.secondary' }} titleAccess={r.correoNotificacion} />}
                    </Box>
                    <Typography variant="body2" color="text.secondary">
                      {r.demandanteDelProceso} - {r.descripcion || ''}
                    </Typography>
                  </Box>
                  <Chip
                    label={formatDate(r.fechaVencimiento)}
                    size="small"
                    color={r.completado ? 'success' : isOverdue ? 'error' : 'warning'}
                    variant="outlined"
                  />
                  <IconButton size="small" color="error" onClick={() => eliminar.mutate({ procesoId: r.procesoId, id: r.id })}>
                    <Delete fontSize="small" />
                  </IconButton>
                </Box>
              );
            })}
          </CardContent>
        </Card>
      )}
    </Box>
  );
}
