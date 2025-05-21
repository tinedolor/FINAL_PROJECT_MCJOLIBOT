import React, { useState, useEffect } from 'react';
import {
  Container,
  Paper,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Box,
  Alert,
  CircularProgress,
  Chip,
  useTheme,
  IconButton,
  Tooltip,
  Divider,
} from '@mui/material';
import {
  Person as PersonIcon,
  AdminPanelSettings as AdminIcon,
  Support as SupportIcon,
  Badge as BadgeIcon,
  Domain as DomainIcon,
} from '@mui/icons-material';
import axios from '../services/axiosConfig';

function Users() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const theme = useTheme();

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const response = await axios.get('/api/Users');
        setUsers(response.data);
        setLoading(false);
      } catch (error) {
        console.error('Error fetching users:', error);
        setError('Failed to fetch user records');
        setLoading(false);
      }
    };

    fetchUsers();
  }, []);

  const getRoleColor = (role) => {
    switch (role.toLowerCase()) {
      case 'admin':
        return {
          color: theme.palette.error.main,
          bgcolor: theme.palette.error.lighter || '#ffebee',
          icon: <AdminIcon fontSize="small" />
        };
      case 'support':
        return {
          color: theme.palette.info.main,
          bgcolor: theme.palette.info.lighter || '#e3f2fd',
          icon: <SupportIcon fontSize="small" />
        };
      default:
        return {
          color: theme.palette.success.main,
          bgcolor: theme.palette.success.lighter || '#e8f5e9',
          icon: <PersonIcon fontSize="small" />
        };
    }
  };

  if (loading) {
    return (
      <Box 
        display="flex" 
        justifyContent="center" 
        alignItems="center" 
        minHeight="80vh"
        sx={{ backgroundColor: 'background.default' }}
      >
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Paper 
        elevation={0}
        sx={{ 
          p: 3,
          borderRadius: 2,
          backgroundColor: 'background.paper',
          border: '1px solid',
          borderColor: 'divider',
        }}
      >
        <Box 
          display="flex" 
          justifyContent="space-between" 
          alignItems="center" 
          mb={3}
          sx={{
            pb: 2,
            borderBottom: '1px solid',
            borderColor: 'divider',
          }}
        >
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
          <Alert 
            severity="error" 
            sx={{ 
              mb: 2,
              borderRadius: 1,
            }}
          >
            {error}
          </Alert>
        )}

        <TableContainer>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell sx={{ fontWeight: 600, color: 'text.primary' }}>User ID</TableCell>
                <TableCell sx={{ fontWeight: 600, color: 'text.primary' }}>Username</TableCell>
                <TableCell sx={{ fontWeight: 600, color: 'text.primary' }}>Full Name</TableCell>
                <TableCell sx={{ fontWeight: 600, color: 'text.primary' }}>Email</TableCell>
                <TableCell sx={{ fontWeight: 600, color: 'text.primary' }}>Role</TableCell>
                <TableCell sx={{ fontWeight: 600, color: 'text.primary' }}>Employee ID</TableCell>
                <TableCell sx={{ fontWeight: 600, color: 'text.primary' }}>Department</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {users.map((user) => {
                const roleStyle = getRoleColor(user.role);
                return (
                  <TableRow 
                    key={user.id}
                    sx={{ 
                      '&:hover': { 
                        backgroundColor: 'action.hover',
                        transition: 'background-color 0.2s ease-in-out',
                      },
                      cursor: 'pointer',
                    }}
                  >
                    <TableCell>
                      <Typography variant="body2" color="text.secondary">#{user.id}</Typography>
                    </TableCell>
                    <TableCell>
                      <Box display="flex" alignItems="center" gap={1}>
                        <PersonIcon sx={{ color: 'primary.main', fontSize: 20 }} />
                        <Typography>{user.username}</Typography>
                      </Box>
                    </TableCell>
                    <TableCell>{user.fullName}</TableCell>
                    <TableCell>
                      <Typography variant="body2" color="text.secondary">
                        {user.email}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Chip
                        icon={roleStyle.icon}
                        label={user.role}
                        size="small"
                        sx={{
                          color: roleStyle.color,
                          bgcolor: roleStyle.bgcolor,
                          '& .MuiChip-icon': {
                            color: roleStyle.color,
                          },
                        }}
                      />
                    </TableCell>
                    <TableCell>
                      <Box display="flex" alignItems="center" gap={1}>
                        <BadgeIcon sx={{ color: 'text.secondary', fontSize: 20 }} />
                        <Typography variant="body2">{user.employeeId}</Typography>
                      </Box>
                    </TableCell>
                    <TableCell>
                      <Box display="flex" alignItems="center" gap={1}>
                        <DomainIcon sx={{ color: 'text.secondary', fontSize: 20 }} />
                        <Typography variant="body2">{user.departmentId}</Typography>
                      </Box>
                    </TableCell>
                  </TableRow>
                );
              })}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>
    </Container>
  );
}

export default Users; 