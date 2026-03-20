import { Drawer, List, ListItemButton, ListItemIcon, ListItemText, Toolbar, Typography, Box, Divider } from '@mui/material';
import { Dashboard, Gavel, NotificationsActive, Settings } from '@mui/icons-material';
import { useNavigate, useLocation } from 'react-router-dom';
import JusticeLadyIcon from './JusticeLadyIcon';

const DRAWER_WIDTH = 240;

const menuItems = [
  { text: 'Panel', icon: <Dashboard />, path: '/dashboard' },
  { text: 'Procesos', icon: <Gavel />, path: '/procesos' },
  { text: 'Recordatorios', icon: <NotificationsActive />, path: '/recordatorios' },
  { text: 'Configuracion', icon: <Settings />, path: '/configuracion' },
];

export default function Sidebar() {
  const navigate = useNavigate();
  const location = useLocation();

  return (
    <Drawer
      variant="permanent"
      sx={{
        width: DRAWER_WIDTH,
        flexShrink: 0,
        '& .MuiDrawer-paper': {
          width: DRAWER_WIDTH,
          boxSizing: 'border-box',
          backgroundColor: '#2d3436',
          borderRight: '1px solid #3d4446',
        },
      }}
    >
      <Toolbar sx={{ py: 2, minHeight: 80 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, width: '100%' }}>
          <JusticeLadyIcon sx={{ color: '#a8c896', fontSize: 40 }} />
          <Box>
            <Typography variant="h6" noWrap sx={{ color: '#a8c896', lineHeight: 1.2 }} fontWeight="bold">
              Procesos
            </Typography>
            <Typography variant="caption" noWrap sx={{ color: '#7a8a6e', letterSpacing: 0.5 }}>
              Sistema Legal
            </Typography>
          </Box>
        </Box>
      </Toolbar>
      <Divider sx={{ borderColor: '#3d4446' }} />
      <List>
        {menuItems.map((item) => (
          <ListItemButton
            key={item.path}
            selected={location.pathname.startsWith(item.path)}
            onClick={() => navigate(item.path)}
            sx={{
              color: '#b2bec3',
              '&:hover': {
                backgroundColor: '#3d4446',
                color: '#dfe6e9',
              },
              '&.Mui-selected': {
                backgroundColor: '#4a6741',
                color: '#ffffff',
                '&:hover': {
                  backgroundColor: '#5a7a4f',
                },
              },
              '& .MuiListItemIcon-root': {
                color: 'inherit',
              },
            }}
          >
            <ListItemIcon>{item.icon}</ListItemIcon>
            <ListItemText primary={item.text} />
          </ListItemButton>
        ))}
      </List>
    </Drawer>
  );
}

export { DRAWER_WIDTH };
