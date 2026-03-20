import { Box, Typography, TextField, IconButton, Button, Paper } from '@mui/material';
import { Add, Delete } from '@mui/icons-material';
import type { Comision } from '../../types/proceso';

interface Props {
  value: Comision[];
  onChange: (comisiones: Comision[]) => void;
}

export default function ComisionesEditor({ value, onChange }: Props) {
  const handleAdd = () => {
    onChange([...value, { persona: '', porcentaje: '' }]);
  };

  const handleChange = (index: number, field: keyof Comision, val: string) => {
    const updated = value.map((c, i) => i === index ? { ...c, [field]: val } : c);
    onChange(updated);
  };

  const handleRemove = (index: number) => {
    onChange(value.filter((_, i) => i !== index));
  };

  return (
    <Paper variant="outlined" sx={{ p: 2 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1.5 }}>
        <Typography variant="subtitle2" color="text.secondary">Comisiones</Typography>
        <Button size="small" startIcon={<Add />} onClick={handleAdd}>Agregar</Button>
      </Box>

      {value.length === 0 ? (
        <Typography variant="body2" color="text.secondary">No hay comisiones configuradas</Typography>
      ) : (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.5 }}>
          {value.map((c, i) => (
            <Box key={i} sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
              <TextField
                size="small" label="Persona" value={c.persona}
                onChange={(e) => handleChange(i, 'persona', e.target.value)}
                sx={{ flex: 1 }}
              />
              <TextField
                size="small" label="Porcentaje" value={c.porcentaje}
                onChange={(e) => handleChange(i, 'porcentaje', e.target.value)}
                sx={{ width: 130 }}
              />
              <IconButton size="small" color="error" onClick={() => handleRemove(i)}>
                <Delete fontSize="small" />
              </IconButton>
            </Box>
          ))}
        </Box>
      )}
    </Paper>
  );
}
