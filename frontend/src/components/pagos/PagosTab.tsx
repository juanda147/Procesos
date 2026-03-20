import { useState } from 'react';
import { Box, Typography, Button, Card, CardContent, Table, TableHead, TableBody, TableRow, TableCell, IconButton, Dialog, DialogTitle, DialogContent, DialogActions, TextField, Grid } from '@mui/material';
import { Add, Edit, Delete } from '@mui/icons-material';
import { useCrearPago, useActualizarPago, useEliminarPago } from '../../hooks/usePagos';
import { formatDate, formatCurrency, formatDateForInput } from '../../utils/formatters';
import type { Pago, PagoCreate } from '../../types/pago';

interface Props {
  procesoId: string;
  pagos: Pago[];
}

const emptyPago: PagoCreate = { fecha: new Date().toISOString().split('T')[0], monto: 0 };

export default function PagosTab({ procesoId, pagos }: Props) {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [form, setForm] = useState<PagoCreate>(emptyPago);

  const crear = useCrearPago(procesoId);
  const actualizar = useActualizarPago(procesoId);
  const eliminar = useEliminarPago(procesoId);

  const total = pagos.reduce((sum, p) => sum + p.monto, 0);

  const openNew = () => { setForm(emptyPago); setEditingId(null); setDialogOpen(true); };
  const openEdit = (pago: Pago) => {
    setForm({ fecha: formatDateForInput(pago.fecha), monto: pago.monto, concepto: pago.concepto || '', metodoPago: pago.metodoPago || '' });
    setEditingId(pago.id);
    setDialogOpen(true);
  };

  const handleSave = async () => {
    if (editingId) await actualizar.mutateAsync({ id: editingId, pago: form });
    else await crear.mutateAsync(form);
    setDialogOpen(false);
  };

  return (
    <Card>
      <CardContent>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Box>
            <Typography variant="h6">Pagos</Typography>
            <Typography variant="body2" color="text.secondary">Total: {formatCurrency(total)}</Typography>
          </Box>
          <Button variant="contained" size="small" startIcon={<Add />} onClick={openNew}>Agregar Pago</Button>
        </Box>

        {pagos.length === 0 ? (
          <Typography color="text.secondary">No hay pagos registrados</Typography>
        ) : (
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Fecha</TableCell>
                <TableCell>Monto</TableCell>
                <TableCell>Concepto</TableCell>
                <TableCell>Metodo</TableCell>
                <TableCell width={100}>Acciones</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {pagos.map((p) => (
                <TableRow key={p.id}>
                  <TableCell>{formatDate(p.fecha)}</TableCell>
                  <TableCell>{formatCurrency(p.monto)}</TableCell>
                  <TableCell>{p.concepto || '-'}</TableCell>
                  <TableCell>{p.metodoPago || '-'}</TableCell>
                  <TableCell>
                    <IconButton size="small" onClick={() => openEdit(p)}><Edit fontSize="small" /></IconButton>
                    <IconButton size="small" color="error" onClick={() => eliminar.mutate(p.id)}><Delete fontSize="small" /></IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}

        <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="sm" fullWidth>
          <DialogTitle>{editingId ? 'Editar Pago' : 'Nuevo Pago'}</DialogTitle>
          <DialogContent>
            <Grid container spacing={2} sx={{ mt: 1 }}>
              <Grid size={{ xs: 6 }}>
                <TextField fullWidth label="Fecha" type="date" value={form.fecha}
                  onChange={(e) => setForm({ ...form, fecha: e.target.value })}
                  slotProps={{ inputLabel: { shrink: true } }} />
              </Grid>
              <Grid size={{ xs: 6 }}>
                <TextField fullWidth label="Monto" type="number" value={form.monto}
                  onChange={(e) => setForm({ ...form, monto: Number(e.target.value) })} />
              </Grid>
              <Grid size={{ xs: 6 }}>
                <TextField fullWidth label="Concepto" value={form.concepto || ''}
                  onChange={(e) => setForm({ ...form, concepto: e.target.value })} />
              </Grid>
              <Grid size={{ xs: 6 }}>
                <TextField fullWidth label="Metodo de Pago" value={form.metodoPago || ''}
                  onChange={(e) => setForm({ ...form, metodoPago: e.target.value })} />
              </Grid>
            </Grid>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setDialogOpen(false)}>Cancelar</Button>
            <Button variant="contained" onClick={handleSave}>Guardar</Button>
          </DialogActions>
        </Dialog>
      </CardContent>
    </Card>
  );
}
