import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ThemeProvider, createTheme, CssBaseline } from '@mui/material';
import AppLayout from './components/layout/AppLayout';
import DashboardPage from './components/dashboard/DashboardPage';
import ProcesosListPage from './components/procesos/ProcesosListPage';
import ProcesoForm from './components/procesos/ProcesoForm';
import ProcesoDetallePage from './components/procesos/ProcesoDetallePage';
// ProcesoForm: solo para crear nuevos procesos
// ProcesoDetallePage: página unificada para ver/editar proceso con tabs de pagos, notas, recordatorios
import RecordatoriosPage from './components/recordatorios/RecordatoriosPage';
import ConfiguracionPage from './components/configuracion/ConfiguracionPage';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: { staleTime: 30_000, retry: 1 },
  },
});

const theme = createTheme({
  palette: {
    primary: { main: '#4a6741' },
    secondary: { main: '#8b7355' },
    background: {
      default: '#f0ebe3',
      paper: '#faf7f2',
    },
    text: {
      primary: '#2d3436',
      secondary: '#636e72',
    },
    divider: '#d5cec4',
  },
  typography: {
    fontFamily: '"Inter", "Roboto", "Helvetica", "Arial", sans-serif',
  },
  components: {
    MuiCard: {
      styleOverrides: {
        root: {
          backgroundColor: '#faf7f2',
          borderColor: '#d5cec4',
          boxShadow: '0 1px 3px rgba(0,0,0,0.06)',
        },
      },
    },
    MuiPaper: {
      styleOverrides: {
        root: {
          backgroundImage: 'none',
        },
      },
    },
    MuiChip: {
      styleOverrides: {
        root: {
          fontWeight: 500,
        },
      },
    },
  },
});

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <BrowserRouter>
          <Routes>
            <Route element={<AppLayout />}>
              <Route index element={<Navigate to="/dashboard" replace />} />
              <Route path="dashboard" element={<DashboardPage />} />
              <Route path="procesos" element={<ProcesosListPage />} />
              <Route path="procesos/nuevo" element={<ProcesoForm />} />
              <Route path="procesos/:id" element={<ProcesoDetallePage />} />
              <Route path="recordatorios" element={<RecordatoriosPage />} />
              <Route path="configuracion" element={<ConfiguracionPage />} />
            </Route>
          </Routes>
        </BrowserRouter>
      </ThemeProvider>
    </QueryClientProvider>
  );
}
