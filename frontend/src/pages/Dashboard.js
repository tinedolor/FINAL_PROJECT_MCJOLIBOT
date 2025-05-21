import React, { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Grid,
  Paper,
  Box,
  Card,
  CardContent,
  CircularProgress,
  Alert,
  Avatar,
} from '@mui/material';
import PeopleIcon from '@mui/icons-material/People';
import BadgeIcon from '@mui/icons-material/Badge';
import ConfirmationNumberIcon from '@mui/icons-material/ConfirmationNumber';
import axios from '../services/axiosConfig';

const Dashboard = () => {
  const [users, setUsers] = useState([]);
  const [employees, setEmployees] = useState([]);
  const [tickets, setTickets] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [usersRes, employeesRes, ticketsRes] = await Promise.all([
          axios.get('/api/Users'),
          axios.get('/api/Employees'),
          axios.get('/api/Tickets'),
        ]);
        setUsers(usersRes.data);
        setEmployees(employeesRes.data);
        setTickets(ticketsRes.data);
        setLoading(false);
      } catch (err) {
        setError('Failed to load dashboard data');
        setLoading(false);
      }
    };
    fetchData();
  }, []);

  const getTicketCount = (status) =>
    tickets.filter((t) => t.status && t.status.toLowerCase() === status).length;

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Grid container spacing={3}>
        <Grid item xs={12}>
          <Card sx={{ p: 3, borderRadius: 3, background: 'linear-gradient(90deg, #f8fafc 60%, #e3f2fd 100%)', mb: 2 }}>
            <CardContent>
              <Typography variant="h4" fontWeight={700} gutterBottom sx={{ color: 'primary.main' }}>
                Welcome to the Helpdesk System
              </Typography>
              <Typography variant="body1" color="text.secondary">
                The Helpdesk System is your modern, centralized platform for managing support tickets, employee records, and user access. Track issues, assign tasks, and monitor your organization's support workflow with ease and efficiency.
              </Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} md={4}>
          <Card sx={{ display: 'flex', alignItems: 'center', p: 2, borderRadius: 3, boxShadow: 3 }}>
            <Avatar sx={{ bgcolor: 'primary.main', mr: 2 }}>
              <PeopleIcon />
            </Avatar>
            <Box>
              <Typography variant="h6">Total Users</Typography>
              {loading ? <CircularProgress size={24} /> : <Typography variant="h4">{users.length}</Typography>}
            </Box>
          </Card>
        </Grid>
        <Grid item xs={12} md={4}>
          <Card sx={{ display: 'flex', alignItems: 'center', p: 2, borderRadius: 3, boxShadow: 3 }}>
            <Avatar sx={{ bgcolor: 'success.main', mr: 2 }}>
              <BadgeIcon />
            </Avatar>
            <Box>
              <Typography variant="h6">Total Employees</Typography>
              {loading ? <CircularProgress size={24} /> : <Typography variant="h4">{employees.length}</Typography>}
            </Box>
          </Card>
        </Grid>
        <Grid item xs={12} md={4}>
          <Card sx={{ display: 'flex', alignItems: 'center', p: 2, borderRadius: 3, boxShadow: 3 }}>
            <Avatar sx={{ bgcolor: 'info.main', mr: 2 }}>
              <ConfirmationNumberIcon />
            </Avatar>
            <Box>
              <Typography variant="h6">Total Tickets</Typography>
              {loading ? <CircularProgress size={24} /> : <Typography variant="h4">{tickets.length}</Typography>}
            </Box>
          </Card>
        </Grid>
        <Grid item xs={12} md={6}>
          <Card sx={{ p: 3, borderRadius: 3, boxShadow: 2 }}>
            <Typography variant="h6" gutterBottom>Ticket Status Overview</Typography>
            {loading ? <CircularProgress size={24} /> : (
              <Box>
                <Typography variant="body1">Active Tickets: <b>{getTicketCount('open')}</b></Typography>
                <Typography variant="body1">Pending Tickets: <b>{getTicketCount('in progress')}</b></Typography>
                <Typography variant="body1">Resolved Tickets: <b>{getTicketCount('resolved')}</b></Typography>
                <Typography variant="body1">Closed Tickets: <b>{getTicketCount('closed')}</b></Typography>
              </Box>
            )}
          </Card>
        </Grid>
        <Grid item xs={12} md={6}>
          <Card sx={{ p: 3, borderRadius: 3, boxShadow: 2, height: '100%' }}>
            <Typography variant="h6" gutterBottom>System Functionality</Typography>
            <Typography variant="body2" color="text.secondary">
              <ul style={{ margin: 0, paddingLeft: 18 }}>
                <li>Monitor and manage all support tickets in real time</li>
                <li>View and manage employee and user records</li>
                <li>Assign, escalate, and resolve tickets efficiently</li>
                <li>Role-based access for secure operations</li>
                <li>Comprehensive audit logs for transparency</li>
              </ul>
            </Typography>
          </Card>
        </Grid>
        {error && (
          <Grid item xs={12}>
            <Alert severity="error">{error}</Alert>
          </Grid>
        )}
      </Grid>
    </Container>
  );
};

export default Dashboard; 