import { useState } from 'react';
import { Box, Typography, Card, CardContent, Grid, TextField, Button, IconButton, List, ListItem, ListItemText, ListItemSecondaryAction, Tabs, Tab, CircularProgress } from '@mui/material';
import { Add, Delete, Edit, Check, Close } from '@mui/icons-material';
import { useCatalogos, useCrearCatalogo, useActualizarCatalogo, useEliminarCatalogo } from '../../hooks/useCatalogos';
import CamposGlobalesPanel from './CamposGlobalesPanel';

const CATALOG_TYPES = [
  { tipo: 'ciudad', label: 'Ciudades' },
  { tipo: 'claseProceso', label: 'Clases de Proceso' },
  { tipo: 'ingresadoPor', label: 'Ingresado Por' },
];

function CatalogoPanel({ tipo, label }: { tipo: string; label: string }) {
  const { data: catalogos = [], isLoading } = useCatalogos(tipo);
  const crear = useCrearCatalogo();
  const actualizar = useActualizarCatalogo();
  const eliminar = useEliminarCatalogo();
  const [nuevoValor, setNuevoValor] = useState('');
  const [editingId, setEditingId] = useState<string | null>(null);
  const [editingValor, setEditingValor] = useState('');

  const handleAdd = async () => {
    if (!nuevoValor.trim()) return;
    await crear.mutateAsync({ tipo, valor: nuevoValor.trim() });
    setNuevoValor('');
  };

  const startEdit = (id: string, valor: string) => {
    setEditingId(id);
    setEditingValor(valor);
  };

  const cancelEdit = () => {
    setEditingId(null);
    setEditingValor('');
  };

  const saveEdit = async () => {
    if (!editingId || !editingValor.trim()) return;
    await actualizar.mutateAsync({ id: editingId, valor: editingValor.trim(), tipo });
    setEditingId(null);
    setEditingValor('');
  };

  if (isLoading) return <CircularProgress size={24} />;

  return (
    <Box>
      <Box sx={{ display: 'flex', gap: 1, mb: 2 }}>
        <TextField
          size="small" fullWidth label={`Agregar ${label.toLowerCase()}`}
          value={nuevoValor}
          onChange={(e) => setNuevoValor(e.target.value)}
          onKeyDown={(e) => e.key === 'Enter' && handleAdd()}
        />
        <Button variant="contained" startIcon={<Add />} onClick={handleAdd}
          disabled={!nuevoValor.trim() || crear.isPending} sx={{ minWidth: 100 }}>
          Agregar
        </Button>
      </Box>

      {catalogos.length === 0 ? (
        <Typography color="text.secondary" variant="body2">No hay opciones configuradas</Typography>
      ) : (
        <List dense>
          {catalogos.map((c) => (
            <ListItem key={c.id} sx={{ borderBottom: '1px solid', borderColor: 'divider' }}>
              {editingId === c.id ? (
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, flex: 1, mr: 1 }}>
                  <TextField
                    size="small" fullWidth value={editingValor}
                    onChange={(e) => setEditingValor(e.target.value)}
                    onKeyDown={(e) => {
                      if (e.key === 'Enter') saveEdit();
                      if (e.key === 'Escape') cancelEdit();
                    }}
                    autoFocus
                  />
                  <IconButton size="small" color="primary" onClick={saveEdit}
                    disabled={!editingValor.trim() || actualizar.isPending}>
                    <Check fontSize="small" />
                  </IconButton>
                  <IconButton size="small" onClick={cancelEdit}>
                    <Close fontSize="small" />
                  </IconButton>
                </Box>
              ) : (
                <>
                  <ListItemText primary={c.valor} />
                  <ListItemSecondaryAction>
                    <IconButton edge="end" size="small" onClick={() => startEdit(c.id, c.valor)}
                      sx={{ mr: 0.5 }}>
                      <Edit fontSize="small" />
                    </IconButton>
                    <IconButton edge="end" size="small" color="error"
                      onClick={() => eliminar.mutate({ id: c.id, tipo })}>
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

export default function ConfiguracionPage() {
  const [tabIndex, setTabIndex] = useState(0);

  return (
    <Box>
      <Typography variant="h5" fontWeight="bold" gutterBottom>Configuracion</Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        Administre las opciones disponibles en los formularios de procesos.
      </Typography>

      <Card>
        <CardContent>
          <Tabs value={tabIndex} onChange={(_e, v) => setTabIndex(v)} sx={{ mb: 3 }}>
            {CATALOG_TYPES.map((ct) => <Tab key={ct.tipo} label={ct.label} />)}
            <Tab label="Campos Adicionales" />
          </Tabs>

          <Grid container>
            <Grid size={{ xs: 12, md: 8 }}>
              {tabIndex < CATALOG_TYPES.length ? (
                <CatalogoPanel
                  tipo={CATALOG_TYPES[tabIndex].tipo}
                  label={CATALOG_TYPES[tabIndex].label}
                />
              ) : (
                <CamposGlobalesPanel />
              )}
            </Grid>
          </Grid>
        </CardContent>
      </Card>
    </Box>
  );
}
