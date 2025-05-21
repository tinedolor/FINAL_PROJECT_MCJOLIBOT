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
} from '@mui/material';
import axios from '../services/axiosConfig';

function TicketDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [ticket, setTicket] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [newRemark, setNewRemark] = useState('');
  const [updateData, setUpdateData] = useState({
    status: '',
    assignedTo: null,
  });

  useEffect(() => {
    fetchTicket();
  }, [id]);

  const fetchTicket = async () => {
    try {
      const response = await axios.get(`/api/Tickets/${id}`);
      setTicket(response.data);
      setUpdateData({
        status: response.data.status,
        assignedTo: response.data.assignedTo,
      });
      setLoading(false);
    } catch (err) {
      setError('Failed to fetch ticket details');
      setLoading(false);
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

  const handleRemarkSubmit = async (e) => {
    e.preventDefault();
    try {
      const user = JSON.parse(localStorage.getItem('user'));
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

  if (loading) return <Typography>Loading...</Typography>;
  if (error) return <Typography color="error">{error}</Typography>;
  if (!ticket) return <Typography>Ticket not found</Typography>;

  return (
    <Box>
      <Box sx={{ mb: 3, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h5">
          Ticket #{ticket.id}: {ticket.title}
        </Typography>
        <Button variant="outlined" onClick={() => navigate('/')}>
          Back to List
        </Button>
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
                <Chip
                  label={ticket.severity}
                  color={
                    ticket.severity === 'High' ? 'error' :
                    ticket.severity === 'Medium' ? 'warning' : 'success'
                  }
                  size="small"
                  sx={{ mt: 1 }}
                />
              </Grid>
              <Grid item xs={6} sm={3}>
                <Typography variant="body2" color="text.secondary">Created</Typography>
                <Typography>
                  {new Date(ticket.createdAt).toLocaleDateString()}
                </Typography>
              </Grid>
              <Grid item xs={6} sm={3}>
                <Typography variant="body2" color="text.secondary">Department</Typography>
                <Typography>
                  {ticket.department?.name || 'Not assigned'}
                </Typography>
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
              <Typography>
                {ticket.assignedToUser?.fullName || 'Not assigned'}
              </Typography>
            </Box>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
}

export default TicketDetail; 