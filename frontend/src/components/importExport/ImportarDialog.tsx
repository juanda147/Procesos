import { useState } from 'react';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button, Typography, Box, Alert, CircularProgress } from '@mui/material';
import { Upload } from '@mui/icons-material';
import { useQueryClient } from '@tanstack/react-query';
import { importExportApi } from '../../api/importExportApi';

interface Props {
  open: boolean;
  onClose: () => void;
}

export default function ImportarDialog({ open, onClose }: Props) {
  const [file, setFile] = useState<File | null>(null);
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState<{ importados: number; errores: string[] } | null>(null);
  const qc = useQueryClient();

  const handleImport = async () => {
    if (!file) return;
    setLoading(true);
    try {
      const res = await importExportApi.importar(file);
      setResult(res);
      qc.invalidateQueries({ queryKey: ['procesos'] });
      qc.invalidateQueries({ queryKey: ['dashboard'] });
      qc.invalidateQueries({ queryKey: ['filtros'] });
    } catch {
      setResult({ importados: 0, errores: ['Error al importar el archivo'] });
    }
    setLoading(false);
  };

  const handleClose = () => {
    setFile(null);
    setResult(null);
    onClose();
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>Importar desde Excel</DialogTitle>
      <DialogContent>
        {result ? (
          <Box>
            <Alert severity={result.errores.length > 0 ? 'warning' : 'success'} sx={{ mb: 2 }}>
              {result.importados} procesos importados exitosamente.
            </Alert>
            {result.errores.length > 0 && (
              <Box>
                <Typography variant="subtitle2" color="error">Errores:</Typography>
                {result.errores.map((e, i) => <Typography key={i} variant="body2" color="error">{e}</Typography>)}
              </Box>
            )}
          </Box>
        ) : (
          <Box>
            <Typography variant="body2" sx={{ mb: 2 }}>
              Seleccione un archivo Excel (.xlsx) con el formato de procesos para importar.
            </Typography>
            <Button variant="outlined" component="label" startIcon={<Upload />} fullWidth>
              {file ? file.name : 'Seleccionar archivo'}
              <input type="file" hidden accept=".xlsx,.xls" onChange={(e) => setFile(e.target.files?.[0] || null)} />
            </Button>
          </Box>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>{result ? 'Cerrar' : 'Cancelar'}</Button>
        {!result && (
          <Button variant="contained" onClick={handleImport} disabled={!file || loading}>
            {loading ? <CircularProgress size={20} /> : 'Importar'}
          </Button>
        )}
      </DialogActions>
    </Dialog>
  );
}
