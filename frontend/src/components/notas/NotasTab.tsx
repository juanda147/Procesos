import { useState } from 'react';
import { Box, Typography, Button, Card, CardContent, IconButton, Dialog, DialogTitle, DialogContent, DialogActions, TextField, Paper } from '@mui/material';
import { Add, Edit, Delete } from '@mui/icons-material';
import { useCrearNota, useActualizarNota, useEliminarNota } from '../../hooks/useNotas';
import { formatDateTime } from '../../utils/formatters';
import type { Nota } from '../../types/nota';

interface Props {
  procesoId: string;
  notas: Nota[];
}

export default function NotasTab({ procesoId, notas }: Props) {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [contenido, setContenido] = useState('');

  const crear = useCrearNota(procesoId);
  const actualizar = useActualizarNota(procesoId);
  const eliminar = useEliminarNota(procesoId);

  const openNew = () => { setContenido(''); setEditingId(null); setDialogOpen(true); };
  const openEdit = (nota: Nota) => { setContenido(nota.contenido); setEditingId(nota.id); setDialogOpen(true); };

  const handleSave = async () => {
    if (editingId) await actualizar.mutateAsync({ id: editingId, nota: { contenido } });
    else await crear.mutateAsync({ contenido });
    setDialogOpen(false);
  };

  return (
    <Card>
      <CardContent>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Typography variant="h6">Notas</Typography>
          <Button variant="contained" size="small" startIcon={<Add />} onClick={openNew}>Agregar Nota</Button>
        </Box>

        {notas.length === 0 ? (
          <Typography color="text.secondary">No hay notas registradas</Typography>
        ) : (
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
            {notas.map((nota) => (
              <Paper key={nota.id} variant="outlined" sx={{ p: 2 }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                  <Box sx={{ flex: 1 }}>
                    <Typography variant="body1" sx={{ whiteSpace: 'pre-wrap' }}>{nota.contenido}</Typography>
                    <Typography variant="caption" color="text.secondary" sx={{ mt: 1 }}>
                      {formatDateTime(nota.fechaCreacion)}
                      {nota.fechaActualizacion !== nota.fechaCreacion && ` (editada ${formatDateTime(nota.fechaActualizacion)})`}
                    </Typography>
                  </Box>
                  <Box>
                    <IconButton size="small" onClick={() => openEdit(nota)}><Edit fontSize="small" /></IconButton>
                    <IconButton size="small" color="error" onClick={() => eliminar.mutate(nota.id)}><Delete fontSize="small" /></IconButton>
                  </Box>
                </Box>
              </Paper>
            ))}
          </Box>
        )}

        <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="sm" fullWidth>
          <DialogTitle>{editingId ? 'Editar Nota' : 'Nueva Nota'}</DialogTitle>
          <DialogContent>
            <TextField fullWidth multiline rows={4} label="Contenido" value={contenido}
              onChange={(e) => setContenido(e.target.value)} sx={{ mt: 1 }} />
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setDialogOpen(false)}>Cancelar</Button>
            <Button variant="contained" onClick={handleSave} disabled={!contenido.trim()}>Guardar</Button>
          </DialogActions>
        </Dialog>
      </CardContent>
    </Card>
  );
}
