import { useState } from 'react';
import { Box, Typography, TextField, Button, Grid, Card, CardContent } from '@mui/material';
import { Save, ArrowBack } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { useCrearProceso } from '../../hooks/useProcesos';
import DynamicAutocomplete from '../common/DynamicAutocomplete';
import ComisionesEditor from './ComisionesEditor';
import CamposDinamicosSection from './CamposDinamicosSection';
import type { ProcesoCreate } from '../../types/proceso';

const emptyForm: ProcesoCreate = {
  fecha: new Date().toISOString().split('T')[0],
  demandante: '', demandado: '', radicado: '', juzgado: '', ciudad: '', claseProceso: '',
  comisiones: [], camposGlobales: {}, camposPropios: [],
};

export default function ProcesoForm() {
  const navigate = useNavigate();
  const crear = useCrearProceso();
  const [form, setForm] = useState<ProcesoCreate>(emptyForm);

  const handleChange = (field: keyof ProcesoCreate, value: string) => {
    setForm((prev) => ({ ...prev, [field]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const result = await crear.mutateAsync(form);
    navigate(`/procesos/${result.id}`);
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 3 }}>
        <Button startIcon={<ArrowBack />} onClick={() => navigate(-1)}>Volver</Button>
        <Typography variant="h5" fontWeight="bold">Nuevo Proceso</Typography>
      </Box>

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
                <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end' }}>
                  <Button variant="outlined" onClick={() => navigate(-1)}>Cancelar</Button>
                  <Button type="submit" variant="contained" startIcon={<Save />} disabled={crear.isPending}>
                    {crear.isPending ? 'Guardando...' : 'Crear Proceso'}
                  </Button>
                </Box>
              </Grid>
            </Grid>
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
}
