import React, { useMemo, useState } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import Layout from './components/Layout';
import TicketList from './pages/TicketList';
import TicketDetail from './pages/TicketDetail';
import CreateTicket from './pages/CreateTicket';
import Login from './pages/Login';
import AddEmployee from './pages/AddEmployee';
import PrivateRoute from './components/PrivateRoute';
import AdminRoute from './components/AdminRoute';
import Dashboard from './pages/Dashboard';
import Tickets from './pages/Tickets';
import EmployeeRecords from './pages/EmployeeRecords';
import Users from './pages/Users';
import AuditLogs from './pages/AuditLogs';
import { AuthProvider } from './contexts/AuthContext';
import './services/axiosConfig';

// Create ThemeContext
export const ColorModeContext = React.createContext({ toggleColorMode: () => {} });

function App() {
  const [mode, setMode] = useState('light');

  const colorMode = useMemo(
    () => ({
      toggleColorMode: () => {
        setMode((prevMode) => (prevMode === 'light' ? 'dark' : 'light'));
      },
    }),
    [],
  );

  const theme = useMemo(
    () =>
      createTheme({
        palette: {
          mode,
          ...(mode === 'light'
            ? {
                // Light mode
                primary: {
                  main: '#2196f3',
                  light: '#64b5f6',
                  dark: '#1976d2',
                  contrastText: '#fff',
                },
                secondary: {
                  main: '#f50057',
                  light: '#ff4081',
                  dark: '#c51162',
                  contrastText: '#fff',
                },
                background: {
                  default: '#f8f9fa',
                  paper: '#ffffff',
                },
                text: {
                  primary: '#2c3e50',
                  secondary: '#636e72',
                },
                divider: 'rgba(0,0,0,0.08)',
              }
            : {
                // Dark mode
                primary: {
                  main: '#90caf9',
                  light: '#e3f2fd',
                  dark: '#42a5f5',
                  contrastText: '#000',
                },
                secondary: {
                  main: '#f48fb1',
                  light: '#fce4ec',
                  dark: '#f06292',
                  contrastText: '#000',
                },
                background: {
                  default: '#121212',
                  paper: '#1e1e1e',
                },
                text: {
                  primary: '#fff',
                  secondary: '#b3b3b3',
                },
                divider: 'rgba(255,255,255,0.08)',
              }),
        },
        typography: {
          fontFamily: '"Inter", "Roboto", "Helvetica", "Arial", sans-serif',
          h1: {
            fontWeight: 600,
            fontSize: '2.5rem',
          },
          h2: {
            fontWeight: 600,
            fontSize: '2rem',
          },
          h3: {
            fontWeight: 600,
            fontSize: '1.75rem',
          },
          h4: {
            fontWeight: 600,
            fontSize: '1.5rem',
          },
          h5: {
            fontWeight: 600,
            fontSize: '1.25rem',
          },
          h6: {
            fontWeight: 600,
            fontSize: '1rem',
          },
          button: {
            textTransform: 'none',
            fontWeight: 500,
          },
        },
        shape: {
          borderRadius: 12,
        },
        components: {
          MuiButton: {
            styleOverrides: {
              root: {
                borderRadius: 8,
                textTransform: 'none',
                fontWeight: 500,
                padding: '8px 16px',
              },
              contained: {
                boxShadow: 'none',
                '&:hover': {
                  boxShadow: '0px 2px 4px rgba(0,0,0,0.1)',
                },
              },
            },
          },
          MuiPaper: {
            styleOverrides: {
              root: {
                borderRadius: 12,
                boxShadow: '0px 2px 4px rgba(0,0,0,0.05)',
              },
            },
          },
          MuiCard: {
            styleOverrides: {
              root: {
                borderRadius: 12,
                boxShadow: '0px 2px 4px rgba(0,0,0,0.05)',
              },
            },
          },
          MuiTextField: {
            styleOverrides: {
              root: {
                '& .MuiOutlinedInput-root': {
                  borderRadius: 8,
                },
              },
            },
          },
        },
      }),
    [mode],
  );

  return (
    <ColorModeContext.Provider value={colorMode}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <AuthProvider>
          <Router>
            <Routes>
              <Route path="/login" element={<Login />} />
              <Route path="/" element={
                <PrivateRoute>
                  <Layout />
                </PrivateRoute>
              }>
                <Route index element={<Dashboard />} />
                <Route path="tickets" element={<Tickets />} />
                <Route path="employee-records" element={
                  <AdminRoute>
                    <EmployeeRecords />
                  </AdminRoute>
                } />
                <Route path="add-employee" element={
                  <AdminRoute>
                    <AddEmployee />
                  </AdminRoute>
                } />
                <Route path="users" element={
                  <AdminRoute>
                    <Users />
                  </AdminRoute>
                } />
                <Route path="audit-logs" element={
                  <AdminRoute>
                    <AuditLogs />
                  </AdminRoute>
                } />
                <Route path="tickets/create" element={<CreateTicket />} />
                <Route path="tickets/:id" element={<TicketDetail />} />
              </Route>
              <Route path="*" element={<Navigate to="/login" replace />} />
            </Routes>
          </Router>
        </AuthProvider>
      </ThemeProvider>
    </ColorModeContext.Provider>
  );
}

export default App; 