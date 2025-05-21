import React, { useMemo, useState } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { ThemeProvider } from '@mui/material/styles';
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
import Profile from './pages/Profile';
import { AuthProvider } from './contexts/AuthContext';
import theme from './theme';
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
                <Route path="profile" element={<Profile />} />
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