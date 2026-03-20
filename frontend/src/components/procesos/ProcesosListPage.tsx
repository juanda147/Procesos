import { useState } from 'react';
import { Box, Typography, Button, TextField, MenuItem, Grid, IconButton, InputAdornment } from '@mui/material';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { Add, Search, Visibility, Delete, Clear } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { useProcesos, useEliminarProceso } from '../../hooks/useProcesos';
import { useFiltrosDisponibles } from '../../hooks/useDashboard';
import { formatDate } from '../../utils/formatters';
import ConfirmDialog from '../common/ConfirmDialog';
import ImportarDialog from '../importExport/ImportarDialog';
import ExportarButton from '../importExport/ExportarButton';
import type { ProcesoFiltros } from '../../types/proceso';

const estadoBadge = (estado: string | null | undefined) => {
  if (!estado) return null;
  return (
    <span className="text-blue-700 text-xs font-medium leading-snug" style={{ whiteSpace: 'normal', wordBreak: 'break-word', display: '-webkit-box', WebkitLineClamp: 3, WebkitBoxOrient: 'vertical', overflow: 'hidden' }} title={estado}>
      {estado}
    </span>
  );
};

export default function ProcesosListPage() {
  const navigate = useNavigate();
  const [filtros, setFiltros] = useState<ProcesoFiltros>({ pagina: 1, porPagina: 20, ordenDescendente: true });
  const [busquedaInput, setBusquedaInput] = useState('');
  const [deleteId, setDeleteId] = useState<string | null>(null);
  const [importOpen, setImportOpen] = useState(false);

  const { data, isLoading } = useProcesos(filtros);
  const { data: filtrosDisponibles } = useFiltrosDisponibles();
  const eliminar = useEliminarProceso();

  const handleSearch = () => {
    setFiltros({ ...filtros, busqueda: busquedaInput || undefined, pagina: 1 });
  };

  const clearFilters = () => {
    setFiltros({ pagina: 1, porPagina: 20, ordenDescendente: true });
    setBusquedaInput('');
  };

  const columns: GridColDef[] = [
    { field: 'fecha', headerName: 'Fecha', width: 110, valueFormatter: (value: string) => formatDate(value) },
    { field: 'demandante', headerName: 'Demandante', flex: 0.8, minWidth: 120 },
    { field: 'demandado', headerName: 'Demandado', flex: 0.8, minWidth: 120 },
    { field: 'radicado', headerName: 'Radicado', width: 130 },
    { field: 'ciudad', headerName: 'Ciudad', width: 110 },
    {
      field: 'claseProceso', headerName: 'Tipo', width: 150,
      renderCell: (params) => params.value ? (
        <span style={{ whiteSpace: 'normal', wordBreak: 'break-word', lineHeight: 1.4 }}>
          {params.value}
        </span>
      ) : null,
    },
    {
      field: 'estadoActual', headerName: 'Estado', flex: 1, minWidth: 180,
      renderCell: (params) => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 0.5 }}>
          {params.row.terminado && <span className="text-green-700 text-xs font-bold">✓ Terminado</span>}
          {estadoBadge(params.value)}
        </Box>
      ),
    },
    {
      field: 'actions', headerName: 'Acciones', width: 100, sortable: false,
      renderCell: (params) => (
        <Box>
          <IconButton size="small" onClick={() => navigate(`/procesos/${params.row.id}`)}><Visibility fontSize="small" /></IconButton>
          <IconButton size="small" color="error" onClick={() => setDeleteId(params.row.id)}><Delete fontSize="small" /></IconButton>
        </Box>
      ),
    },
  ];

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h5" fontWeight="bold">Procesos</Typography>
        <Box sx={{ display: 'flex', gap: 1 }}>
          <ExportarButton />
          <Button variant="outlined" onClick={() => setImportOpen(true)}>Importar Excel</Button>
          <Button variant="contained" startIcon={<Add />} onClick={() => navigate('/procesos/nuevo')}>
            Nuevo Proceso
          </Button>
        </Box>
      </Box>

      <Grid container spacing={2} sx={{ mb: 2 }}>
        <Grid size={{ xs: 12, md: 4 }}>
          <TextField
            fullWidth size="small" placeholder="Buscar por demandante, demandado, radicado..."
            value={busquedaInput}
            onChange={(e) => setBusquedaInput(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
            slotProps={{
              input: {
                startAdornment: <InputAdornment position="start"><Search /></InputAdornment>,
              },
            }}
          />
        </Grid>
        <Grid size={{ xs: 6, md: 2 }}>
          <TextField
            select fullWidth size="small" label="Ciudad" value={filtros.ciudad || ''}
            onChange={(e) => setFiltros({ ...filtros, ciudad: e.target.value || undefined, pagina: 1 })}
          >
            <MenuItem value="">Todas</MenuItem>
            {filtrosDisponibles?.ciudades.map((c) => <MenuItem key={c} value={c}>{c}</MenuItem>)}
          </TextField>
        </Grid>
        <Grid size={{ xs: 6, md: 2 }}>
          <TextField
            select fullWidth size="small" label="Tipo" value={filtros.claseProceso || ''}
            onChange={(e) => setFiltros({ ...filtros, claseProceso: e.target.value || undefined, pagina: 1 })}
          >
            <MenuItem value="">Todos</MenuItem>
            {filtrosDisponibles?.clasesProceso.map((c) => <MenuItem key={c} value={c}>{c}</MenuItem>)}
          </TextField>
        </Grid>
        <Grid size={{ xs: 6, md: 2 }}>
          <TextField
            select fullWidth size="small" label="Ingresado por" value={filtros.ingresadoPor || ''}
            onChange={(e) => setFiltros({ ...filtros, ingresadoPor: e.target.value || undefined, pagina: 1 })}
          >
            <MenuItem value="">Todos</MenuItem>
            {filtrosDisponibles?.ingresadoPor.map((c) => <MenuItem key={c} value={c}>{c}</MenuItem>)}
          </TextField>
        </Grid>
        <Grid size={{ xs: 6, md: 2 }}>
          <Button variant="text" startIcon={<Clear />} onClick={clearFilters}>Limpiar</Button>
        </Grid>
      </Grid>

      <DataGrid
        rows={data?.items || []}
        columns={columns}
        loading={isLoading}
        rowCount={data?.totalCount || 0}
        paginationMode="server"
        paginationModel={{ page: (filtros.pagina || 1) - 1, pageSize: filtros.porPagina || 20 }}
        onPaginationModelChange={(model) => setFiltros({ ...filtros, pagina: model.page + 1, porPagina: model.pageSize })}
        pageSizeOptions={[10, 20, 50]}
        disableRowSelectionOnClick
        autoHeight
        rowHeight={72}
        onRowDoubleClick={(params) => navigate(`/procesos/${params.id}`)}
        sx={{
          bgcolor: 'background.paper',
          borderColor: '#d5cec4',
          '& .MuiDataGrid-columnHeaders': { backgroundColor: '#ede8e0' },
          '& .MuiDataGrid-cell': { display: 'flex', alignItems: 'center', py: 1, borderColor: '#e8e2d9' },
          '& .MuiDataGrid-row:hover': { backgroundColor: '#f5f0e8' },
          '& .MuiDataGrid-cell--withRenderer .MuiDataGrid-cellContent': { overflow: 'hidden' },
        }}
      />

      <ConfirmDialog
        open={deleteId !== null}
        title="Eliminar Proceso"
        message="Esta segura de que desea eliminar este proceso? El proceso sera archivado y no aparecera en la lista."
        onConfirm={() => { if (deleteId) eliminar.mutate(deleteId); setDeleteId(null); }}
        onCancel={() => setDeleteId(null)}
      />

      <ImportarDialog open={importOpen} onClose={() => setImportOpen(false)} />
    </Box>
  );
}
