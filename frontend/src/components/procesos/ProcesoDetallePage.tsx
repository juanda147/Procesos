import { useState, useEffect } from 'react';
import {
  Box, Typography, TextField, Button, Grid, Card, CardContent,
  Chip, Tabs, Tab, CircularProgress, Divider, FormControlLabel, Checkbox,
} from '@mui/material';
import { ArrowBack, Save } from '@mui/icons-material';
import { useNavigate, useParams } from 'react-router-dom';
import { useProceso, useActualizarProceso } from '../../hooks/useProcesos';
import { formatDateForInput, formatDate } from '../../utils/formatters';
import DynamicAutocomplete from '../common/DynamicAutocomplete';
import ComisionesEditor from './ComisionesEditor';
import CamposDinamicosSection from './CamposDinamicosSection';
import PagosTab from '../pagos/PagosTab';
import NotasTab from '../notas/NotasTab';
import RecordatoriosTab from '../recordatorios/RecordatoriosTab';
import type { ProcesoCreate } from '../../types/proceso';

export default function ProcesoDetallePage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { data: proceso, isLoading } = useProceso(id || '');
  const actualizar = useActualizarProceso();
  const [tab, setTab] = useState(0);

  const [form, setForm] = useState<ProcesoCreate>({
    fecha: '', demandante: '', demandado: '', radicado: '', juzgado: '', ciudad: '', claseProceso: '',
    comisiones: [], camposGlobales: {}, camposPropios: [],
  });

  useEffect(() => {
    if (proceso) {
      setForm({
        fecha: formatDateForInput(proceso.fecha),
        demandante: proceso.demandante,
        demandado: proceso.demandado,
        radicado: proceso.radicado,
        juzgado: proceso.juzgado,
        ciudad: proceso.ciudad,
        claseProceso: proceso.claseProceso,
        representamos: proceso.representamos || '',
        procesoIngresadoPor: proceso.procesoIngresadoPor || '',
        honorarios: proceso.honorarios || '',
        comisiones: proceso.comisiones || [],
        estadoActual: proceso.estadoActual || '',
        camposGlobales: proceso.camposGlobales || {},
        camposPropios: proceso.camposPropios || [],
        terminado: proceso.terminado || false,
        notaTerminacion: proceso.notaTerminacion || '',
      });
    }
  }, [proceso]);

  const handleChange = (field: keyof ProcesoCreate, value: string) => {
    setForm((prev) => ({ ...prev, [field]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (id) {
      await actualizar.mutateAsync({ id, proceso: form });
    }
  };

  if (isLoading) return <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}><CircularProgress /></Box>;
  if (!proceso) return <Typography>Proceso no encontrado</Typography>;

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
          <Button startIcon={<ArrowBack />} onClick={() => navigate('/procesos')}>Volver</Button>
          <Box>
            <Typography variant="h5" fontWeight="bold">{proceso.demandante}</Typography>
            <Typography variant="body2" color="text.secondary">vs. {proceso.demandado}</Typography>
          </Box>
        </Box>
      </Box>

      <Box sx={{ display: 'flex', gap: 1, mb: 2, flexWrap: 'wrap' }}>
        <Chip label={proceso.claseProceso} color="primary" />
        <Chip label={proceso.ciudad} variant="outlined" />
        {proceso.terminado && <Chip label="Terminado" color="success" />}
        <Chip label={`Creado: ${formatDate(proceso.fechaCreacion)}`} variant="outlined" size="small" />
        <Chip label={`Actualizado: ${formatDate(proceso.fechaActualizacion)}`} variant="outlined" size="small" />
      </Box>

      <Tabs value={tab} onChange={(_e, v) => setTab(v)} sx={{ mb: 2 }}>
        <Tab label="Información" />
        <Tab label={`Pagos (${proceso.pagos.length})`} />
        <Tab label={`Notas (${proceso.notas.length})`} />
        <Tab label={`Recordatorios (${proceso.recordatorios.filter(r => !r.completado).length})`} />
      </Tabs>

      {tab === 0 && (
        <Card>
          <CardContent>
            <Box component="form" onSubmit={handleSubmit}>
              <Grid container spacing={2}>
                <Grid size={{ xs: 12, md: 4 }}>
                  <TextField required fullWidth label="Fecha" type="date" value={form.fecha}
                    onChange={(e) => handleChange('fecha', e.target.value)} slotProps={{ inputLabel: { shrink: true } }} />
                </Grid>
                <Grid size={{ xs: 12, md: 4 }}>
                  <TextField required fullWidth label="Demandante" value={form.demandante}
                    onChange={(e) => handleChange('demandante', e.target.value)} />
                </Grid>
                <Grid size={{ xs: 12, md: 4 }}>
                  <TextField required fullWidth label="Demandado" value={form.demandado}
                    onChange={(e) => handleChange('demandado', e.target.value)} />
                </Grid>
                <Grid size={{ xs: 12, md: 4 }}>
                  <TextField required fullWidth label="Radicado" value={form.radicado}
                    onChange={(e) => handleChange('radicado', e.target.value)} />
                </Grid>
                <Grid size={{ xs: 12, md: 4 }}>
                  <TextField required fullWidth label="Juzgado" value={form.juzgado}
                    onChange={(e) => handleChange('juzgado', e.target.value)} />
                </Grid>
                <Grid size={{ xs: 12, md: 4 }}>
                  <DynamicAutocomplete tipo="ciudad" label="Ciudad" required
                    value={form.ciudad} onChange={(val) => handleChange('ciudad', val)} />
                </Grid>
                <Grid size={{ xs: 12, md: 4 }}>
                  <DynamicAutocomplete tipo="claseProceso" label="Clase de Proceso" required
                    value={form.claseProceso} onChange={(val) => handleChange('claseProceso', val)} />
                </Grid>
                <Grid size={{ xs: 12, md: 4 }}>
                  <TextField fullWidth label="Representamos" value={form.representamos || ''}
                    onChange={(e) => handleChange('representamos', e.target.value)} />
                </Grid>
                <Grid size={{ xs: 12, md: 4 }}>
                  <DynamicAutocomplete tipo="ingresadoPor" label="Ingresado Por"
                    value={form.procesoIngresadoPor || ''} onChange={(val) => handleChange('procesoIngresadoPor', val)} />
                </Grid>
                <Grid size={{ xs: 12, md: 4 }}>
                  <TextField fullWidth label="Honorarios" value={form.honorarios || ''}
                    onChange={(e) => handleChange('honorarios', e.target.value)} />
                </Grid>
                <Grid size={{ xs: 12, md: 8 }}>
                  <ComisionesEditor value={form.comisiones} onChange={(c) => setForm((prev) => ({ ...prev, comisiones: c }))} />
                </Grid>
                <CamposDinamicosSection
                  camposGlobales={form.camposGlobales || {}}
                  camposPropios={form.camposPropios || []}
                  onChangeCamposGlobales={(campos) => setForm((prev) => ({ ...prev, camposGlobales: campos }))}
                  onChangeCamposPropios={(campos) => setForm((prev) => ({ ...prev, camposPropios: campos }))}
                />
                <Grid size={{ xs: 12 }}>
                  <TextField fullWidth multiline rows={3} label="Estado Actual" value={form.estadoActual || ''}
                    onChange={(e) => handleChange('estadoActual', e.target.value)} />
                </Grid>
                <Grid size={{ xs: 12 }}>
                  <Divider sx={{ my: 1 }} />
                  <FormControlLabel
                    control={
                      <Checkbox
                        checked={form.terminado || false}
                        onChange={(e) => setForm((prev) => ({ ...prev, terminado: e.target.checked, notaTerminacion: e.target.checked ? prev.notaTerminacion : '' }))}
                        color="success"
                      />
                    }
                    label={<Typography fontWeight="medium">Marcar proceso como terminado</Typography>}
                  />
                  {form.terminado && (
                    <TextField
                      fullWidth multiline rows={2} label="Nota de terminación"
                      placeholder="Explique el motivo de la terminación..."
                      value={form.notaTerminacion || ''}
                      onChange={(e) => handleChange('notaTerminacion', e.target.value)}
                      sx={{ mt: 1 }}
                    />
                  )}
                </Grid>
                <Grid size={{ xs: 12 }}>
                  <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end' }}>
                    <Button type="submit" variant="contained" startIcon={<Save />} disabled={actualizar.isPending}>
                      {actualizar.isPending ? 'Guardando...' : 'Guardar Cambios'}
                    </Button>
                  </Box>
                </Grid>
              </Grid>
            </Box>
          </CardContent>
        </Card>
      )}

      {tab === 1 && <PagosTab procesoId={proceso.id} pagos={proceso.pagos} />}
      {tab === 2 && <NotasTab procesoId={proceso.id} notas={proceso.notas} />}
      {tab === 3 && <RecordatoriosTab procesoId={proceso.id} recordatorios={proceso.recordatorios} />}
    </Box>
  );
}
