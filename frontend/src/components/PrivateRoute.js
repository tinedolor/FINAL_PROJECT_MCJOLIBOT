import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';

function PrivateRoute({ children }) {
  const location = useLocation();
  const token = localStorage.getItem('token');
  const userStr = localStorage.getItem('user');
  
  // Check if we have both token and valid user data
  if (!token || !userStr) {
    // Redirect to login but save the attempted url
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  try {
    // Verify that user data is valid JSON
    JSON.parse(userStr);
    return children;
  } catch (e) {
    // If user data is invalid, clear it and redirect to login
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    return <Navigate to="/login" state={{ from: location }} replace />;
  }
}

export default PrivateRoute; 