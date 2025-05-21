import React from 'react';
import { Container, Typography, Grid, Paper, Box } from '@mui/material';

const Dashboard = () => {
  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        Dashboard
      </Typography>
      <Grid container spacing={3}>
        <Grid item xs={12} md={8}>
          <Paper sx={{ p: 2, display: 'flex', flexDirection: 'column' }}>
            <Typography variant="h6" gutterBottom>
              Welcome to the Helpdesk Dashboard
            </Typography>
            <Typography variant="body1">
              This is your central hub for managing tickets and monitoring system status.
            </Typography>
          </Paper>
        </Grid>
        <Grid item xs={12} md={4}>
          <Paper sx={{ p: 2, display: 'flex', flexDirection: 'column' }}>
            <Typography variant="h6" gutterBottom>
              Quick Stats
            </Typography>
            <Box>
              <Typography variant="body2">
                • Active Tickets: 0
              </Typography>
              <Typography variant="body2">
                • Pending Tickets: 0
              </Typography>
              <Typography variant="body2">
                • Resolved Tickets: 0
              </Typography>
            </Box>
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
};

export default Dashboard; 