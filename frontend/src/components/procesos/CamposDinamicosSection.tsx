import { Box, Typography, TextField, IconButton, Grid, Divider, Button } from '@mui/material';
import { Add, Delete } from '@mui/icons-material';
import { useCamposGlobales } from '../../hooks/useCamposGlobales';
import type { CampoPropio } from '../../types/proceso';

interface Props {
  camposGlobales: Record<string, string>;
  camposPropios: CampoPropio[];
  onChangeCamposGlobales: (campos: Record<string, string>) => void;
  onChangeCamposPropios: (campos: CampoPropio[]) => void;
}

export default function CamposDinamicosSection({ camposGlobales, camposPropios, onChangeCamposGlobales, onChangeCamposPropios }: Props) {
  const { data: definiciones = [] } = useCamposGlobales();

  const handleGlobalChange = (nombre: string, valor: string) => {
    onChangeCamposGlobales({ ...camposGlobales, [nombre]: valor });
  };

  const handlePropioChange = (index: number, field: 'nombre' | 'valor', value: string) => {
    const updated = [...camposPropios];
    updated[index] = { ...updated[index], [field]: value };
    onChangeCamposPropios(updated);
  };

  const addCampoPropio = () => {
    onChangeCamposPropios([...camposPropios, { nombre: '', valor: '' }]);
  };

  const removeCampoPropio = (index: number) => {
    onChangeCamposPropios(camposPropios.filter((_, i) => i !== index));
  };

  const getInputType = (tipo: string) => {
    if (tipo === 'fecha') return 'date';
    if (tipo === 'numero') return 'number';
    return 'text';
  };

  return (
    <Grid size={{ xs: 12 }}>
      {definiciones.length > 0 && (
        <>
          <Divider sx={{ my: 2 }} />
          <Typography variant="subtitle2" color="text.secondary" sx={{ mb: 1.5 }}>
            Campos Adicionales
          </Typography>
          <Grid container spacing={2}>
            {definiciones.map((def) => (
              <Grid size={{ xs: 12, md: 4 }} key={def.id}>
                <TextField
                  fullWidth
                  label={def.nombre}
                  type={getInputType(def.tipo)}
                  value={camposGlobales[def.nombre] || ''}
                  onChange={(e) => handleGlobalChange(def.nombre, e.target.value)}
                  slotProps={def.tipo === 'fecha' ? { inputLabel: { shrink: true } } : undefined}
                />
              </Grid>
            ))}
          </Grid>
        </>
      )}

      <Divider sx={{ my: 2 }} />
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1.5 }}>
        <Typography variant="subtitle2" color="text.secondary">
          Campos Propios de este Proceso
        </Typography>
        <Button size="small" startIcon={<Add />} onClick={addCampoPropio}>
          Agregar Campo
        </Button>
      </Box>

      {camposPropios.length === 0 ? (
        <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
          No hay campos propios. Use "Agregar Campo" para añadir campos exclusivos de este proceso.
        </Typography>
      ) : (
        <Grid container spacing={2}>
          {camposPropios.map((campo, index) => (
            <Grid size={{ xs: 12 }} key={index}>
              <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
                <TextField
                  size="small" label="Nombre" value={campo.nombre}
                  onChange={(e) => handlePropioChange(index, 'nombre', e.target.value)}
                  sx={{ flex: 1 }}
                />
                <TextField
                  size="small" label="Valor" value={campo.valor}
                  onChange={(e) => handlePropioChange(index, 'valor', e.target.value)}
                  sx={{ flex: 2 }}
                />
                <IconButton size="small" color="error" onClick={() => removeCampoPropio(index)}>
                  <Delete fontSize="small" />
                </IconButton>
              </Box>
            </Grid>
          ))}
        </Grid>
      )}
    </Grid>
  );
}
