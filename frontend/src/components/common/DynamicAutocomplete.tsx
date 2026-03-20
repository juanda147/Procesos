import { useState } from 'react';
import { Autocomplete, TextField, IconButton, InputAdornment, Dialog, DialogTitle, DialogContent, DialogActions, Button } from '@mui/material';
import { Add } from '@mui/icons-material';
import { useCatalogos, useCrearCatalogo } from '../../hooks/useCatalogos';

interface Props {
  tipo: string;
  label: string;
  value: string;
  onChange: (value: string) => void;
  required?: boolean;
}

export default function DynamicAutocomplete({ tipo, label, value, onChange, required }: Props) {
  const { data: catalogos = [] } = useCatalogos(tipo);
  const crearCatalogo = useCrearCatalogo();
  const [dialogOpen, setDialogOpen] = useState(false);
  const [nuevoValor, setNuevoValor] = useState('');

  const opciones = catalogos.map((c) => c.valor);

  const handleAgregar = async () => {
    if (!nuevoValor.trim()) return;
    await crearCatalogo.mutateAsync({ tipo, valor: nuevoValor.trim() });
    onChange(nuevoValor.trim());
    setNuevoValor('');
    setDialogOpen(false);
  };

  return (
    <>
      <Autocomplete
        freeSolo
        options={opciones}
        value={value}
        onChange={(_e, val) => onChange(val || '')}
        onInputChange={(_e, val) => onChange(val)}
        renderInput={(params) => (
          <TextField
            {...params}
            required={required}
            label={label}
            slotProps={{
              input: {
                ...params.InputProps,
                endAdornment: (
                  <>
                    {params.InputProps.endAdornment}
                    <InputAdornment position="end">
                      <IconButton size="small" onClick={() => setDialogOpen(true)} title="Agregar opcion">
                        <Add fontSize="small" />
                      </IconButton>
                    </InputAdornment>
                  </>
                ),
              },
            }}
          />
        )}
      />

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="xs" fullWidth>
        <DialogTitle>Agregar {label}</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus fullWidth label={`Nuevo ${label.toLowerCase()}`} value={nuevoValor}
            onChange={(e) => setNuevoValor(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && handleAgregar()}
            sx={{ mt: 1 }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>Cancelar</Button>
          <Button variant="contained" onClick={handleAgregar} disabled={!nuevoValor.trim() || crearCatalogo.isPending}>
            Agregar
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
}
