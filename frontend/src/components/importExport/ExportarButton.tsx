import { useState } from 'react';
import { Button, CircularProgress } from '@mui/material';
import { Download } from '@mui/icons-material';
import { importExportApi } from '../../api/importExportApi';

export default function ExportarButton() {
  const [loading, setLoading] = useState(false);

  const handleExport = async () => {
    setLoading(true);
    try {
      const blob = await importExportApi.exportar();
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `Procesos_${new Date().toISOString().split('T')[0]}.xlsx`;
      a.click();
      URL.revokeObjectURL(url);
    } catch {
      alert('Error al exportar');
    }
    setLoading(false);
  };

  return (
    <Button variant="outlined" startIcon={loading ? <CircularProgress size={16} /> : <Download />}
      onClick={handleExport} disabled={loading}>
      Exportar Excel
    </Button>
  );
}
