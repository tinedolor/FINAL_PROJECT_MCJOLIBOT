import React, { useState, useEffect } from 'react';
import {
  Container,
  Paper,
  Typography,
  Box,
  Alert,
  CircularProgress,
} from '@mui/material';
import { DataGrid } from '@mui/x-data-grid';
import PersonIcon from '@mui/icons-material/Person';
import AdminIcon from '@mui/icons-material/AdminPanelSettings';
import SupportIcon from '@mui/icons-material/Support';
import BadgeIcon from '@mui/icons-material/Badge';
import DomainIcon from '@mui/icons-material/Domain';
import axios from '../services/axiosConfig';

function Users() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const response = await axios.get('/api/Users');
        setUsers(response.data);
        setLoading(false);
      } catch (error) {
        setError('Failed to fetch user records');
        setLoading(false);
      }
    };
    fetchUsers();
  }, []);

  const columns = [
    { field: 'id', headerName: 'User ID', width: 90 },
    { field: 'username', headerName: 'Username', width: 150 },
    { field: 'fullName', headerName: 'Full Name', width: 180 },
    { field: 'email', headerName: 'Email', width: 200 },
    {
      field: 'role',
      headerName: 'Role',
      width: 130,
      renderCell: (params) => {
        const role = params.value;
        if (role.toLowerCase() === 'admin') return <AdminIcon color="error" fontSize="small" />;
        if (role.toLowerCase() === 'support') return <SupportIcon color="info" fontSize="small" />;
        return <PersonIcon color="success" fontSize="small" />;
      },
    },
    { field: 'employeeId', headerName: 'Employee ID', width: 120, renderCell: (params) => <><BadgeIcon fontSize="small" sx={{ mr: 0.5 }} />{params.value}</> },
    { field: 'departmentId', headerName: 'Department', width: 120, renderCell: (params) => <><DomainIcon fontSize="small" sx={{ mr: 0.5 }} />{params.value}</> },
  ];

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="80vh" sx={{ backgroundColor: 'background.default' }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Container maxWidth={false} disableGutters sx={{ mt: 4, mb: 4, px: 0, width: '100vw' }}>
      <Paper elevation={0} sx={{ p: 3, borderRadius: 2, backgroundColor: 'background.paper', border: '1px solid', borderColor: 'divider', width: '100%' }}>
        <Box display="flex" justifyContent="space-between" alignItems="center" mb={3} sx={{ pb: 2, borderBottom: '1px solid', borderColor: 'divider' }}>
          <Box display="flex" alignItems="center" gap={1}>
            <PersonIcon sx={{ color: 'primary.main', fontSize: 32 }} />
            <Typography variant="h4" component="h1" sx={{ fontWeight: 600, color: 'text.primary' }}>
              User Records
            </Typography>
          </Box>
          <Typography variant="body2" color="text.secondary">
            Total Users: {users.length}
          </Typography>
        </Box>
        {error && (
          <Alert severity="error" sx={{ mb: 2, borderRadius: 1 }}>{error}</Alert>
        )}
        <Box sx={{ width: '100%' }}>
          <DataGrid
            rows={users}
            columns={columns}
            pageSize={10}
            rowsPerPageOptions={[10, 25, 50]}
            disableSelectionOnClick
            autoHeight
            sx={{ borderRadius: 2, backgroundColor: 'background.paper' }}
          />
        </Box>
      </Paper>
    </Container>
  );
}

export default Users; 