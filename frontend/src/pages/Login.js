import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import {
  Container,
  Box,
  Typography,
  TextField,
  Button,
  Paper,
  Alert,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Grid,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  IconButton,
  InputAdornment,
  Fade,
  useTheme,
  alpha,
} from '@mui/material';
import {
  Visibility,
  VisibilityOff,
  Person,
  Lock,
  Email,
  Badge,
  AccountCircle,
} from '@mui/icons-material';
import axios from '../services/axiosConfig';
import AuditService from '../services/AuditService';

function Login() {
  const theme = useTheme();
  const navigate = useNavigate();
  const location = useLocation();
  const [formData, setFormData] = useState({
    username: '',
    password: '',
  });
  const [error, setError] = useState('');
  const [openCreateAccount, setOpenCreateAccount] = useState(false);
  const [createAccountData, setCreateAccountData] = useState({
    username: '',
    password: '',
    fullName: '',
    email: '',
    employeeId: '',
  });
  const [createAccountError, setCreateAccountError] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [showCreatePassword, setShowCreatePassword] = useState(false);

  // Check if user is already logged in
  useEffect(() => {
    const token = localStorage.getItem('token');
    const userStr = localStorage.getItem('user');
    if (token && userStr) {
      try {
        JSON.parse(userStr);
        navigate('/');
      } catch (e) {
        // If user data is invalid, clear it
        localStorage.removeItem('token');
        localStorage.removeItem('user');
      }
    }
  }, [navigate]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleCreateAccountChange = (e) => {
    const { name, value } = e.target;
    setCreateAccountData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    
    try {
      const response = await axios.post('/api/Auth/login', formData);
      console.log('Login response:', response.data);
      
      // Create a user object with ALL necessary data
      const user = {
        id: response.data.userId,
        username: formData.username.trim(),
        role: response.data.role.trim(),
        departmentId: response.data.departmentId
      };

      // Validate required fields
      if (!user.id || !user.username || !user.role) {
        console.error('Missing required user data from server:', response.data);
        setError('Invalid user data received from server');
        return;
      }

      // Validate field lengths
      if (user.username.length > 100 || user.role.length > 50) {
        console.error('Invalid field lengths:', { username: user.username.length, role: user.role.length });
        setError('Invalid user data received from server');
        return;
      }
      
      // Store auth data
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('user', JSON.stringify(user));
      
      // Log the login action after user data is stored
      try {
        await AuditService.logAction(`User logged in: ${user.username}`);
      } catch (auditError) {
        console.error('Failed to log audit action:', auditError);
        // Don't block login if audit fails
      }
      
      // Redirect to the attempted page or home
      const from = location.state?.from?.pathname || '/';
      navigate(from, { replace: true });
    } catch (err) {
      console.error('Login error:', err);
      setError(err.response?.data?.message || 'Failed to login. Please check your credentials.');
    }
  };

  const handleCreateAccount = async (e) => {
    e.preventDefault();
    setCreateAccountError('');

    try {
      const response = await axios.post('/api/Users/create-account', createAccountData);
      
      setOpenCreateAccount(false);
      setError('Account created successfully. Please login.');
    } catch (err) {
      console.error('Create account error:', err);
      setCreateAccountError(err.response?.data?.message || 'Failed to create account');
    }
  };

  const handleClickShowPassword = () => setShowPassword(!showPassword);
  const handleClickShowCreatePassword = () => setShowCreatePassword(!showCreatePassword);

  return (
    <Box
      sx={{
        minHeight: '100vh',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        backgroundColor: 'background.default',
        backgroundImage: `linear-gradient(45deg, ${alpha(theme.palette.primary.main, 0.05)} 0%, ${alpha(theme.palette.primary.main, 0.1)} 100%)`,
      }}
    >
      <Container maxWidth="sm">
        <Paper
          elevation={0}
          sx={{
            p: 4,
            borderRadius: 3,
            boxShadow: '0px 4px 20px rgba(0, 0, 0, 0.08)',
            border: '1px solid',
            borderColor: 'divider',
          }}
        >
          <Box
            component="form"
            onSubmit={handleSubmit}
            sx={{
              display: 'flex',
              flexDirection: 'column',
              gap: 3,
            }}
          >
            <Box sx={{ textAlign: 'center', mb: 2 }}>
              <Box
                component="img"
                src="/logo.png"
                alt="Logo"
                sx={{
                  height: '120px',
                  width: '250px',
                  mb: 3,
                  objectFit: 'contain'
                }}
              />
              <Typography
                variant="h4"
                component="h1"
                sx={{
                  fontWeight: 600,
                  color: 'primary.main',
                  mb: 1,
                }}
              >
                Helpdesk Login
              </Typography>
              <Typography
                variant="body1"
                color="text.secondary"
              >
                Welcome back! Please login to your account.
              </Typography>
            </Box>

            {error && (
              <Alert
                severity="error"
                sx={{
                  borderRadius: 2,
                }}
              >
                {error}
              </Alert>
            )}

            <TextField
              required
              fullWidth
              name="username"
              label="Username"
              value={formData.username}
              onChange={handleChange}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <Person sx={{ color: 'text.secondary' }} />
                  </InputAdornment>
                ),
              }}
            />

            <TextField
              required
              fullWidth
              name="password"
              label="Password"
              type={showPassword ? 'text' : 'password'}
              value={formData.password}
              onChange={handleChange}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <Lock sx={{ color: 'text.secondary' }} />
                  </InputAdornment>
                ),
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton
                      onClick={handleClickShowPassword}
                      edge="end"
                    >
                      {showPassword ? <VisibilityOff /> : <Visibility />}
                    </IconButton>
                  </InputAdornment>
                ),
              }}
            />

            <Button
              type="submit"
              variant="contained"
              size="large"
              sx={{
                py: 1.5,
                fontSize: '1rem',
                fontWeight: 500,
                boxShadow: 'none',
                '&:hover': {
                  boxShadow: '0px 4px 8px rgba(0, 0, 0, 0.1)',
                },
              }}
            >
              Sign In
            </Button>

            <Box sx={{ display: 'flex', justifyContent: 'center', mt: 2 }}>
              <Typography variant="body2" color="text.secondary" sx={{ mr: 1 }}>
                Don't have an account?
              </Typography>
              <Button
                color="primary"
                onClick={() => setOpenCreateAccount(true)}
                sx={{
                  textTransform: 'none',
                  p: 0,
                  '&:hover': {
                    background: 'none',
                    textDecoration: 'underline',
                  },
                }}
              >
                Sign Up
              </Button>
            </Box>
          </Box>
        </Paper>
      </Container>

      <Dialog 
        open={openCreateAccount} 
        onClose={() => setOpenCreateAccount(false)} 
        maxWidth="sm" 
        fullWidth
        PaperProps={{
          sx: {
            borderRadius: 2,
          },
        }}
      >
        <DialogTitle sx={{ 
          textAlign: 'center',
          color: theme.palette.primary.main,
          fontWeight: 'bold',
        }}>
          Create Account
        </DialogTitle>
        <DialogContent>
          {createAccountError && (
            <Alert severity="error" sx={{ mb: 2, borderRadius: 1 }}>
              {createAccountError}
            </Alert>
          )}
          <Box component="form" onSubmit={handleCreateAccount} noValidate sx={{ mt: 2 }}>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <TextField
                  required
                  fullWidth
                  label="Employee ID"
                  name="employeeId"
                  value={createAccountData.employeeId}
                  onChange={handleCreateAccountChange}
                  InputProps={{
                    startAdornment: (
                      <InputAdornment position="start">
                        <Badge color="action" />
                      </InputAdornment>
                    ),
                  }}
                  sx={{
                    '& .MuiOutlinedInput-root': {
                      borderRadius: 2,
                    },
                  }}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  required
                  fullWidth
                  label="Username"
                  name="username"
                  value={createAccountData.username}
                  onChange={handleCreateAccountChange}
                  InputProps={{
                    startAdornment: (
                      <InputAdornment position="start">
                        <AccountCircle color="action" />
                      </InputAdornment>
                    ),
                  }}
                  sx={{
                    '& .MuiOutlinedInput-root': {
                      borderRadius: 2,
                    },
                  }}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  required
                  fullWidth
                  label="Password"
                  name="password"
                  type={showCreatePassword ? 'text' : 'password'}
                  value={createAccountData.password}
                  onChange={handleCreateAccountChange}
                  InputProps={{
                    startAdornment: (
                      <InputAdornment position="start">
                        <Lock color="action" />
                      </InputAdornment>
                    ),
                    endAdornment: (
                      <InputAdornment position="end">
                        <IconButton
                          aria-label="toggle password visibility"
                          onClick={handleClickShowCreatePassword}
                          edge="end"
                        >
                          {showCreatePassword ? <VisibilityOff /> : <Visibility />}
                        </IconButton>
                      </InputAdornment>
                    ),
                  }}
                  sx={{
                    '& .MuiOutlinedInput-root': {
                      borderRadius: 2,
                    },
                  }}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  required
                  fullWidth
                  label="Full Name"
                  name="fullName"
                  value={createAccountData.fullName}
                  onChange={handleCreateAccountChange}
                  InputProps={{
                    startAdornment: (
                      <InputAdornment position="start">
                        <Person color="action" />
                      </InputAdornment>
                    ),
                  }}
                  sx={{
                    '& .MuiOutlinedInput-root': {
                      borderRadius: 2,
                    },
                  }}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  required
                  fullWidth
                  label="Email"
                  name="email"
                  type="email"
                  value={createAccountData.email}
                  onChange={handleCreateAccountChange}
                  InputProps={{
                    startAdornment: (
                      <InputAdornment position="start">
                        <Email color="action" />
                      </InputAdornment>
                    ),
                  }}
                  sx={{
                    '& .MuiOutlinedInput-root': {
                      borderRadius: 2,
                    },
                  }}
                />
              </Grid>
            </Grid>
          </Box>
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 3 }}>
          <Button 
            onClick={() => setOpenCreateAccount(false)}
            sx={{ 
              borderRadius: 2,
              textTransform: 'none',
            }}
          >
            Cancel
          </Button>
          <Button 
            onClick={handleCreateAccount} 
            variant="contained"
            sx={{ 
              borderRadius: 2,
              textTransform: 'none',
            }}
          >
            Create Account
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

export default Login; 