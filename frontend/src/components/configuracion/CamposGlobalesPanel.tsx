import { useState } from 'react';
import { Box, Typography, TextField, Button, IconButton, List, ListItem, ListItemText, ListItemSecondaryAction, Chip, CircularProgress, MenuItem } from '@mui/material';
import { Add, Delete, Edit, Check, Close } from '@mui/icons-material';
import { useCamposGlobales, useCrearCampoGlobal, useActualizarCampoGlobal, useEliminarCampoGlobal } from '../../hooks/useCamposGlobales';

const TIPOS = [
  { value: 'texto', label: 'Texto' },
  { value: 'fecha', label: 'Fecha' },
  { value: 'numero', label: 'Número' },
];

const tipoChipColor = (tipo: string): 'default' | 'primary' | 'secondary' => {
  if (tipo === 'fecha') return 'secondary';
  if (tipo === 'numero') return 'primary';
  return 'default';
};

export default function CamposGlobalesPanel() {
  const { data: campos = [], isLoading } = useCamposGlobales();
  const crear = useCrearCampoGlobal();
  const actualizar = useActualizarCampoGlobal();
  const eliminar = useEliminarCampoGlobal();

  const [nuevoNombre, setNuevoNombre] = useState('');
  const [nuevoTipo, setNuevoTipo] = useState('texto');
  const [editingId, setEditingId] = useState<string | null>(null);
  const [editingNombre, setEditingNombre] = useState('');
  const [editingTipo, setEditingTipo] = useState('');

  const handleAdd = async () => {
    if (!nuevoNombre.trim()) return;
    await crear.mutateAsync({ nombre: nuevoNombre.trim(), tipo: nuevoTipo });
    setNuevoNombre('');
    setNuevoTipo('texto');
  };

  const startEdit = (id: string, nombre: string, tipo: string) => {
    setEditingId(id);
    setEditingNombre(nombre);
    setEditingTipo(tipo);
  };

  const cancelEdit = () => {
    setEditingId(null);
    setEditingNombre('');
    setEditingTipo('');
  };

  const saveEdit = async () => {
    if (!editingId || !editingNombre.trim()) return;
    await actualizar.mutateAsync({ id: editingId, campo: { nombre: editingNombre.trim(), tipo: editingTipo } });
    cancelEdit();
  };

  if (isLoading) return <CircularProgress size={24} />;

  return (
    <Box>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
        Los campos definidos aquí aparecerán automáticamente en todos los procesos.
      </Typography>

      <Box sx={{ display: 'flex', gap: 1, mb: 2 }}>
        <TextField
          size="small" fullWidth label="Nombre del campo"
          value={nuevoNombre}
          onChange={(e) => setNuevoNombre(e.target.value)}
          onKeyDown={(e) => e.key === 'Enter' && handleAdd()}
        />
        <TextField
          select size="small" label="Tipo" value={nuevoTipo}
          onChange={(e) => setNuevoTipo(e.target.value)}
          sx={{ minWidth: 120 }}
        >
          {TIPOS.map((t) => <MenuItem key={t.value} value={t.value}>{t.label}</MenuItem>)}
        </TextField>
        <Button variant="contained" startIcon={<Add />} onClick={handleAdd}
          disabled={!nuevoNombre.trim() || crear.isPending} sx={{ minWidth: 100 }}>
          Agregar
        </Button>
      </Box>

      {campos.length === 0 ? (
        <Typography color="text.secondary" variant="body2">No hay campos adicionales definidos</Typography>
      ) : (
        <List dense>
          {campos.map((c) => (
            <ListItem key={c.id} sx={{ borderBottom: '1px solid', borderColor: 'divider' }}>
              {editingId === c.id ? (
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, flex: 1, mr: 1 }}>
                  <TextField
                    size="small" fullWidth value={editingNombre}
                    onChange={(e) => setEditingNombre(e.target.value)}
                    onKeyDown={(e) => {
                      if (e.key === 'Enter') saveEdit();
                      if (e.key === 'Escape') cancelEdit();
                    }}
                    autoFocus
                  />
                  <TextField
                    select size="small" value={editingTipo}
                    onChange={(e) => setEditingTipo(e.target.value)}
                    sx={{ minWidth: 100 }}
                  >
                    {TIPOS.map((t) => <MenuItem key={t.value} value={t.value}>{t.label}</MenuItem>)}
                  </TextField>
                  <IconButton size="small" color="primary" onClick={saveEdit}
                    disabled={!editingNombre.trim() || actualizar.isPending}>
                    <Check fontSize="small" />
                  </IconButton>
                  <IconButton size="small" onClick={cancelEdit}>
                    <Close fontSize="small" />
                  </IconButton>
                </Box>
              ) : (
                <>
                  <ListItemText
                    primary={c.nombre}
                    secondary={<Chip label={TIPOS.find(t => t.value === c.tipo)?.label || c.tipo} size="small" color={tipoChipColor(c.tipo)} variant="outlined" />}
                  />
                  <ListItemSecondaryAction>
                    <IconButton edge="end" size="small" onClick={() => startEdit(c.id, c.nombre, c.tipo)} sx={{ mr: 0.5 }}>
                      <Edit fontSize="small" />
                    </IconButton>
                    <IconButton edge="end" size="small" color="error" onClick={() => eliminar.mutate(c.id)}>
                      <Delete fontSize="small" />
                    </IconButton>
                  </ListItemSecondaryAction>
                </>
              )}
            </ListItem>
          ))}
        </List>
      )}
    </Box>
  );
}
