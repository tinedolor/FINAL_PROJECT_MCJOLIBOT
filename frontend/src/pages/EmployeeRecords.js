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
import axios from '../services/axiosConfig';

function EmployeeRecords() {
  const [employees, setEmployees] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchEmployees = async () => {
      try {
        const response = await axios.get('/api/Employees');
        setEmployees(response.data);
        setLoading(false);
      } catch (error) {
        setError('Failed to fetch employee records');
        setLoading(false);
      }
    };
    fetchEmployees();
  }, []);

  const columns = [
    { field: 'employeeId', headerName: 'Employee ID', width: 120 },
    { field: 'firstName', headerName: 'First Name', width: 140 },
    { field: 'lastName', headerName: 'Last Name', width: 140 },
    { field: 'email', headerName: 'Email', width: 200 },
    { field: 'phone', headerName: 'Phone', width: 140 },
    { field: 'hireDate', headerName: 'Hire Date', width: 130, valueGetter: (params) => new Date(params.value).toLocaleDateString() },
    { field: 'jobTitle', headerName: 'Job Title', width: 160 },
    { field: 'department', headerName: 'Department', width: 160 },
    { field: 'salary', headerName: 'Salary', width: 130, valueGetter: (params) => new Intl.NumberFormat('en-PH', { style: 'currency', currency: 'PHP' }).format(params.value) },
  ];

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="80vh">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Paper elevation={3} sx={{ p: 3 }}>
        <Box display="flex" flexDirection="column" alignItems="center" mb={3}>
          <img 
            src="/logo.png" 
            alt="McJolibee Logo" 
            style={{ 
              height: '30px',
              width: '250px',
              marginBottom: '1rem'
            }} 
          />
          <Typography variant="h4" component="h1" gutterBottom>
            Employee Records
          </Typography>
        </Box>
        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>
        )}
        <Box sx={{ width: '100%' }}>
          <DataGrid
            rows={employees}
            columns={columns}
            getRowId={(row) => row.employeeId}
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

export default EmployeeRecords; 