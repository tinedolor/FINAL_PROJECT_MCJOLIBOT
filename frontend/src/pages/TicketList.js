import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Paper,
  Typography,
  Box,
  Button,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  TextField,
  InputAdornment,
  CircularProgress,
  Alert,
  Chip,
} from '@mui/material';
import {
  Visibility as VisibilityIcon,
  Add as AddIcon,
  Search as SearchIcon,
  FilterList as FilterListIcon,
} from '@mui/icons-material';
import { DataGrid } from '@mui/x-data-grid';
import axios from '../services/axiosConfig';

function TicketList() {
  const [tickets, setTickets] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [departments, setDepartments] = useState([]);
  const [filters, setFilters] = useState({
    department: '',
    severity: '',
    search: '',
  });
  const [showFilters, setShowFilters] = useState(false);
  const navigate = useNavigate();
  const user = JSON.parse(localStorage.getItem('user') || '{}');
  const isPrivileged = user.role === 'Admin' || user.role === 'Supervisor';

  useEffect(() => {
    fetchTickets();
    fetchDepartments();
  }, []);

  const fetchTickets = async () => {
    try {
      const response = await axios.get('/api/Tickets');
      setTickets(response.data);
      setLoading(false);
    } catch (err) {
      setError('Failed to fetch tickets');
      setLoading(false);
    }
  };

  const fetchDepartments = async () => {
    try {
      const response = await axios.get('/api/Departments');
      setDepartments(response.data);
    } catch (err) {
      console.error('Failed to fetch departments:', err);
    }
  };

  const handleFilterChange = (e) => {
    const { name, value } = e.target;
    setFilters(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSearchChange = (e) => {
    setFilters(prev => ({
      ...prev,
      search: e.target.value
    }));
  };

  const clearFilters = () => {
    setFilters({
      department: '',
      severity: '',
      search: '',
    });
  };

  const filteredTickets = tickets.filter(ticket => {
    const departmentMatch = isPrivileged
      ? true
      : ticket.departmentId === user.departmentId;
    const matchesDepartment = !filters.department || ticket.departmentId === parseInt(filters.department);
    const matchesSeverity = !filters.severity || ticket.severity === filters.severity;
    const matchesSearch = !filters.search || 
      ticket.title.toLowerCase().includes(filters.search.toLowerCase()) ||
      ticket.description.toLowerCase().includes(filters.search.toLowerCase());
    return departmentMatch && matchesDepartment && matchesSeverity && matchesSearch;
  });

  const columns = [
    { field: 'id', headerName: 'ID', width: 80 },
    { field: 'title', headerName: 'Title', width: 200 },
    { field: 'status', headerName: 'Status', width: 120, renderCell: (params) => <Chip label={params.value} size="small" color={getStatusColor(params.value)} /> },
    { field: 'severity', headerName: 'Severity', width: 120, renderCell: (params) => <Chip label={params.value} size="small" color={getSeverityColor(params.value)} /> },
    { field: 'department', headerName: 'Department', width: 160, valueGetter: (params) => params.row.department?.name || 'Not assigned' },
    { field: 'createdAt', headerName: 'Created At', width: 140, valueGetter: (params) => new Date(params.value).toLocaleDateString() },
    { field: 'actions', headerName: 'Actions', width: 100, sortable: false, filterable: false, renderCell: (params) => (
      <Button size="small" onClick={() => navigate(`/tickets/${params.row.id}`)} startIcon={<VisibilityIcon />}>
        View
      </Button>
    ) },
  ];

  function getSeverityColor(severity) {
    switch ((severity || '').toLowerCase()) {
      case 'high':
      case 'critical':
        return 'error';
      case 'medium':
        return 'warning';
      case 'low':
        return 'success';
      default:
        return 'default';
    }
  }

  function getStatusColor(status) {
    switch ((status || '').toLowerCase()) {
      case 'open':
        return 'info';
      case 'in progress':
        return 'warning';
      case 'resolved':
        return 'success';
      case 'closed':
        return 'default';
      default:
        return 'default';
    }
  }

  if (loading) return <Typography>Loading...</Typography>;
  if (error) return <Typography color="error">{error}</Typography>;

  return (
    <Box>
      <Box sx={{ mb: 3, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h5">
          Tickets
        </Typography>
        <Box>
          <Button
            variant="outlined"
            startIcon={<FilterListIcon />}
            onClick={() => setShowFilters(!showFilters)}
            sx={{ mr: 1 }}
          >
            Filters
          </Button>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() => navigate('/tickets/create')}
          >
            New Ticket
          </Button>
        </Box>
      </Box>
      {showFilters && (
        <Paper sx={{ p: 2, mb: 3 }}>
          <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap' }}>
            <TextField
              size="small"
              label="Search"
              value={filters.search}
              onChange={handleSearchChange}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon />
                  </InputAdornment>
                ),
              }}
              sx={{ minWidth: 200 }}
            />
            <FormControl size="small" sx={{ minWidth: 200 }}>
              <InputLabel>Department</InputLabel>
              <Select
                name="department"
                value={filters.department}
                label="Department"
                onChange={handleFilterChange}
              >
                <MenuItem value="">All Departments</MenuItem>
                {departments.map(dept => (
                  <MenuItem key={dept.id} value={dept.id}>
                    {dept.name}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
            <FormControl size="small" sx={{ minWidth: 200 }}>
              <InputLabel>Severity</InputLabel>
              <Select
                name="severity"
                value={filters.severity}
                label="Severity"
                onChange={handleFilterChange}
              >
                <MenuItem value="">All Severities</MenuItem>
                <MenuItem value="Low">Low</MenuItem>
                <MenuItem value="Medium">Medium</MenuItem>
                <MenuItem value="High">High</MenuItem>
                <MenuItem value="Critical">Critical</MenuItem>
              </Select>
            </FormControl>
            <Button
              variant="outlined"
              onClick={clearFilters}
            >
              Clear Filters
            </Button>
          </Box>
        </Paper>
      )}
      <Paper>
        <Box sx={{ width: '100%' }}>
          <DataGrid
            rows={filteredTickets}
            columns={columns}
            getRowId={(row) => row.id}
            pageSize={10}
            rowsPerPageOptions={[10, 25, 50]}
            disableSelectionOnClick
            autoHeight
            sx={{ borderRadius: 2, backgroundColor: 'background.paper' }}
          />
        </Box>
      </Paper>
    </Box>
  );
}

export default TicketList; 