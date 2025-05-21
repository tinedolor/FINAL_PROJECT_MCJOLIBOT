import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';

function AdminRoute({ children }) {
  const location = useLocation();
  const userStr = localStorage.getItem('user');
  
  try {
    const user = JSON.parse(userStr || '{}');
    if (user?.role !== 'Admin') {
      // Redirect to dashboard if user is not an admin
      return <Navigate to="/" replace />;
    }
    return children;
  } catch (e) {
    // If user data is invalid, redirect to dashboard
    return <Navigate to="/" replace />;
  }
}

export default AdminRoute; 