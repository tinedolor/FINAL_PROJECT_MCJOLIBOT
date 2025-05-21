import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Paper,
  Typography,
  Chip,
  Button,
  TextField,
  Grid,
  Divider,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Alert,
  IconButton,
  Tooltip,
} from '@mui/material';
import {
  Assignment as AssignmentIcon,
  Business as BusinessIcon,
  Edit as EditIcon,
} from '@mui/icons-material';
import axios from '../services/axiosConfig';

function TicketDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [ticket, setTicket] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [newRemark, setNewRemark] = useState('');
  const [users, setUsers] = useState([]);
  const [departments, setDepartments] = useState([]);
  const [updateData, setUpdateData] = useState({
    status: '',
    assignedTo: null,
    departmentId: null,
    severity: '',
  });
  const [isEditing, setIsEditing] = useState(false);
  const user = JSON.parse(localStorage.getItem('user') || '{}');

  useEffect(() => {
    fetchTicket();
    fetchUsers();
    fetchDepartments();
  }, [id]);

  const fetchTicket = async () => {
    try {
      const response = await axios.get(`/api/Tickets/${id}`);
      setTicket(response.data);
      setUpdateData({
        status: response.data.status,
        assignedTo: response.data.assignedTo,
        departmentId: response.data.departmentId,
        severity: response.data.severity,
      });
      setLoading(false);
    } catch (err) {
      setError('Failed to fetch ticket details');
      setLoading(false);
    }
  };

  const fetchUsers = async () => {
    try {
      const response = await axios.get('/api/Users');
      setUsers(response.data);
    } catch (err) {
      console.error('Failed to fetch users:', err);
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

  const handleStatusChange = async (e) => {
    try {
      const { value } = e.target;
      setUpdateData(prev => ({ ...prev, status: value }));
      await axios.put(`/api/Tickets/${id}`, {
        ...ticket,
        status: value,
      });
      fetchTicket();
    } catch (err) {
      setError('Failed to update ticket status');
    }
  };

  const handleAssignmentChange = async (e) => {
    try {
      const { value } = e.target;
      setUpdateData(prev => ({ ...prev, assignedTo: value }));
      await axios.put(`/api/Tickets/${id}/assign`, {
        assignedTo: value
      });
      fetchTicket();
    } catch (err) {
      setError('Failed to assign ticket');
    }
  };

  const handleDepartmentChange = async (e) => {
    try {
      const { value } = e.target;
      setUpdateData(prev => ({ ...prev, departmentId: value }));
      await axios.put(`/api/Tickets/${id}/reassign`, {
        newDepartmentId: value
      });
      fetchTicket();
    } catch (err) {
      setError('Failed to reassign ticket to department');
    }
  };

  const handleSeverityChange = async (e) => {
    try {
      const { value } = e.target;
      setUpdateData(prev => ({ ...prev, severity: value }));
      await axios.put(`/api/Tickets/${id}`, {
        ...ticket,
        severity: value,
      });
      fetchTicket();
    } catch (err) {
      setError('Failed to update ticket severity');
    }
  };

  const handleRemarkSubmit = async (e) => {
    e.preventDefault();
    try {
      await axios.post(`/api/Tickets/${id}/remarks`, {
        ticketId: parseInt(id),
        userId: user.id,
        comment: newRemark,
      });
      setNewRemark('');
      fetchTicket();
    } catch (err) {
      setError('Failed to add remark');
    }
  };

  const canAssignTicket = user.role === 'Admin' || user.role === 'Supervisor';
  const canReassignDepartment = user.role === 'Admin' || user.role === 'Supervisor';

  const canWorkOnTicket =
    user.role === 'Admin' ||
    (user.role === 'Officer' && user.departmentId === ticket?.departmentId) ||
    (user.role === 'JuniorOfficer' &&
      user.departmentId === ticket?.departmentId &&
      (ticket?.severity !== 'Critical' || ticket?.assignedTo === user.id));

  if (loading) return <Typography>Loading...</Typography>;
  if (error) return <Typography color="error">{error}</Typography>;
  if (!ticket) return <Typography>Ticket not found</Typography>;

  return (
    <Box>
      <Box sx={{ mb: 3, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h5">
          Ticket #{ticket.id}: {ticket.title}
        </Typography>
        <Box>
          {canWorkOnTicket && (
            <Tooltip title="Edit Ticket">
              <IconButton onClick={() => setIsEditing(!isEditing)}>
                <EditIcon />
              </IconButton>
            </Tooltip>
          )}
          <Button variant="outlined" onClick={() => navigate('/')}>
            Back to List
          </Button>
        </Box>
      </Box>

      <Grid container spacing={3}>
        <Grid item xs={12} md={8}>
          <Paper sx={{ p: 3, mb: 3 }}>
            <Typography variant="h6" gutterBottom>Details</Typography>
            <Box sx={{ mb: 2 }}>
              <Typography variant="body2" color="text.secondary">Description</Typography>
              <Typography>{ticket.description}</Typography>
            </Box>
            
            <Grid container spacing={2}>
              <Grid item xs={6} sm={3}>
                <Typography variant="body2" color="text.secondary">Status</Typography>
                <FormControl fullWidth size="small" sx={{ mt: 1 }}>
                  <Select
                    value={updateData.status}
                    onChange={handleStatusChange}
                    disabled={!canWorkOnTicket}
                  >
                    <MenuItem value="Open">Open</MenuItem>
                    <MenuItem value="In Progress">In Progress</MenuItem>
                    <MenuItem value="Resolved">Resolved</MenuItem>
                    <MenuItem value="Closed">Closed</MenuItem>
                  </Select>
                </FormControl>
              </Grid>
              <Grid item xs={6} sm={3}>
                <Typography variant="body2" color="text.secondary">Severity</Typography>
                {isEditing ? (
                  <FormControl fullWidth size="small" sx={{ mt: 1 }}>
                    <Select
                      value={updateData.severity}
                      onChange={handleSeverityChange}
                    >
                      <MenuItem value="Low">Low</MenuItem>
                      <MenuItem value="Medium">Medium</MenuItem>
                      <MenuItem value="High">High</MenuItem>
                      <MenuItem value="Critical">Critical</MenuItem>
                    </Select>
                  </FormControl>
                ) : (
                  <Chip
                    label={ticket.severity}
                    color={
                      ticket.severity === 'High' ? 'error' :
                      ticket.severity === 'Medium' ? 'warning' : 'success'
                    }
                    size="small"
                    sx={{ mt: 1 }}
                  />
                )}
              </Grid>
              <Grid item xs={6} sm={3}>
                <Typography variant="body2" color="text.secondary">Created</Typography>
                <Typography>
                  {new Date(ticket.createdAt).toLocaleDateString()}
                </Typography>
              </Grid>
              <Grid item xs={6} sm={3}>
                <Typography variant="body2" color="text.secondary">Department</Typography>
                {isEditing && canReassignDepartment ? (
                  <FormControl fullWidth size="small" sx={{ mt: 1 }}>
                    <Select
                      value={updateData.departmentId}
                      onChange={handleDepartmentChange}
                    >
                      {departments.map(dept => (
                        <MenuItem key={dept.id} value={dept.id}>
                          {dept.name}
                        </MenuItem>
                      ))}
                    </Select>
                  </FormControl>
                ) : (
                  <Typography>
                    {ticket.department?.name || 'Not assigned'}
                  </Typography>
                )}
              </Grid>
            </Grid>
          </Paper>

          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>Remarks</Typography>
            <Box component="form" onSubmit={handleRemarkSubmit} sx={{ mb: 3 }}>
              <TextField
                fullWidth
                multiline
                rows={2}
                placeholder="Add a remark..."
                value={newRemark}
                onChange={(e) => setNewRemark(e.target.value)}
                sx={{ mb: 1 }}
              />
              <Button
                type="submit"
                variant="contained"
                disabled={!newRemark.trim()}
              >
                Add Remark
              </Button>
            </Box>
            <Divider sx={{ my: 2 }} />
            {ticket.remarks?.map((remark) => (
              <Box key={remark.id} sx={{ mb: 2 }}>
                <Typography variant="body2" color="text.secondary">
                  {remark.user?.fullName || 'Unknown'} - {new Date(remark.createdAt).toLocaleString()}
                </Typography>
                <Typography>{remark.comment}</Typography>
                <Divider sx={{ mt: 2 }} />
              </Box>
            ))}
          </Paper>
        </Grid>

        <Grid item xs={12} md={4}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>Assignment</Typography>
            <Box sx={{ mb: 2 }}>
              <Typography variant="body2" color="text.secondary">Created by</Typography>
              <Typography>
                {ticket.createdByUser?.fullName || 'Unknown'}
              </Typography>
            </Box>
            <Box>
              <Typography variant="body2" color="text.secondary">Assigned to</Typography>
              {isEditing && canAssignTicket ? (
                <FormControl fullWidth size="small" sx={{ mt: 1 }}>
                  <Select
                    value={updateData.assignedTo || ''}
                    onChange={handleAssignmentChange}
                  >
                    <MenuItem value="">Unassigned</MenuItem>
                    {users
                      .filter(u => u.departmentId === ticket.departmentId)
                      .map(user => (
                        <MenuItem key={user.id} value={user.id}>
                          {user.fullName}
                        </MenuItem>
                      ))}
                  </Select>
                </FormControl>
              ) : (
                <Typography>
                  {ticket.assignedToUser?.fullName || 'Not assigned'}
                </Typography>
              )}
            </Box>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
}

export default TicketDetail; 