import { useState } from 'react';
import { Box, Typography, Button, Card, CardContent, Checkbox, IconButton, Dialog, DialogTitle, DialogContent, DialogActions, TextField, Grid, Chip } from '@mui/material';
import { Add, Delete, Email } from '@mui/icons-material';
import { useCrearRecordatorio, useCompletarRecordatorio, useEliminarRecordatorio } from '../../hooks/useRecordatorios';
import { formatDate } from '../../utils/formatters';
import type { Recordatorio, RecordatorioCreate } from '../../types/recordatorio';

interface Props {
  procesoId: string;
  recordatorios: Recordatorio[];
}

const emptyForm: RecordatorioCreate = { titulo: '', fechaVencimiento: new Date().toISOString().split('T')[0] };

function getStatusColor(r: Recordatorio): 'error' | 'warning' | 'success' | 'default' {
  if (r.completado) return 'success';
  const days = Math.ceil((new Date(r.fechaVencimiento).getTime() - Date.now()) / (1000 * 60 * 60 * 24));
  if (days < 0) return 'error';
  if (days <= 3) return 'warning';
  return 'default';
}

export default function RecordatoriosTab({ procesoId, recordatorios }: Props) {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [form, setForm] = useState<RecordatorioCreate>(emptyForm);

  const crear = useCrearRecordatorio(procesoId);
  const completar = useCompletarRecordatorio(procesoId);
  const eliminar = useEliminarRecordatorio(procesoId);

  const handleSave = async () => {
    await crear.mutateAsync(form);
    setDialogOpen(false);
    setForm(emptyForm);
  };

  return (
    <Card>
      <CardContent>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Typography variant="h6">Recordatorios</Typography>
          <Button variant="contained" size="small" startIcon={<Add />} onClick={() => setDialogOpen(true)}>
            Agregar Recordatorio
          </Button>
        </Box>

        {recordatorios.length === 0 ? (
          <Typography color="text.secondary">No hay recordatorios</Typography>
        ) : (
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
            {recordatorios.map((r) => (
              <Box key={r.id} sx={{ display: 'flex', alignItems: 'center', gap: 1, p: 1, borderRadius: 1, '&:hover': { bgcolor: 'action.hover' } }}>
                <Checkbox checked={r.completado} onChange={() => completar.mutate(r.id)} />
                <Box sx={{ flex: 1 }}>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                    <Typography variant="body1" sx={{ textDecoration: r.completado ? 'line-through' : 'none' }}>
                      {r.titulo}
                    </Typography>
                    {r.correoNotificacion && <Email sx={{ fontSize: 16, color: 'text.secondary' }} titleAccess={r.correoNotificacion} />}
                  </Box>
                  {r.descripcion && <Typography variant="body2" color="text.secondary">{r.descripcion}</Typography>}
                </Box>
                <Chip label={formatDate(r.fechaVencimiento)} size="small" color={getStatusColor(r)} variant="outlined" />
                <IconButton size="small" color="error" onClick={() => eliminar.mutate(r.id)}>
                  <Delete fontSize="small" />
                </IconButton>
              </Box>
            ))}
          </Box>
        )}

        <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="sm" fullWidth>
          <DialogTitle>Nuevo Recordatorio</DialogTitle>
          <DialogContent>
            <Grid container spacing={2} sx={{ mt: 1 }}>
              <Grid size={{ xs: 12 }}>
                <TextField fullWidth label="Titulo" value={form.titulo}
                  onChange={(e) => setForm({ ...form, titulo: e.target.value })} />
              </Grid>
              <Grid size={{ xs: 12 }}>
                <TextField fullWidth label="Descripcion" multiline rows={2} value={form.descripcion || ''}
                  onChange={(e) => setForm({ ...form, descripcion: e.target.value })} />
              </Grid>
              <Grid size={{ xs: 12 }}>
                <TextField fullWidth label="Fecha de Vencimiento" type="date" value={form.fechaVencimiento}
                  onChange={(e) => setForm({ ...form, fechaVencimiento: e.target.value })}
                  slotProps={{ inputLabel: { shrink: true } }} />
              </Grid>
              <Grid size={{ xs: 12 }}>
                <TextField fullWidth label="Correo de notificación (opcional)" type="email"
                  placeholder="ejemplo@gmail.com"
                  value={form.correoNotificacion || ''}
                  onChange={(e) => setForm({ ...form, correoNotificacion: e.target.value || undefined })}
                  helperText="Si no se especifica, se usa el correo por defecto configurado en el sistema" />
              </Grid>
            </Grid>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setDialogOpen(false)}>Cancelar</Button>
            <Button variant="contained" onClick={handleSave} disabled={!form.titulo.trim()}>Guardar</Button>
          </DialogActions>
        </Dialog>
      </CardContent>
    </Card>
  );
}
