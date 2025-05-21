import React from 'react';
import { Container, Typography } from '@mui/material';
import TicketList from './TicketList';

const Tickets = () => {
  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        Tickets
      </Typography>
      <TicketList />
    </Container>
  );
};

export default Tickets; 