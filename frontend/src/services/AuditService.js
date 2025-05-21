import axios from './axiosConfig';

class AuditService {
  static async logAction(action) {
    try {
      const userStr = localStorage.getItem('user');
      
      // If no user data, don't log
      if (!userStr) {
        console.log('No user data available');
        return;
      }

      let user;
      try {
        user = JSON.parse(userStr);
      } catch (e) {
        console.error('Invalid user data in localStorage');
        return;
      }

      // If we don't have the required user data, don't log
      if (!user || !user.id || !user.username || !user.role) {
        console.error('Missing required user data:', user);
        return;
      }

      // Validate and format data
      const userId = parseInt(user.id);
      if (isNaN(userId)) {
        console.error('Invalid user ID:', user.id);
        return;
      }

      // Ensure all required fields are strings and properly trimmed
      const username = user.username?.trim();
      const userRole = user.role?.trim();
      const actionText = action?.trim();
      const ipAddress = window.location.hostname || 'localhost';

      // Validate required fields
      if (!username || !userRole || !actionText) {
        console.error('Missing or invalid required fields:', {
          username,
          userRole,
          action: actionText
        });
        return;
      }

      // Validate field lengths according to model constraints
      if (username.length > 50 || userRole.length > 20 || actionText.length > 500 || ipAddress.length > 50) {
        console.error('Field length validation failed:', {
          username: username.length,
          userRole: userRole.length,
          action: actionText.length,
          ipAddress: ipAddress.length
        });
        return;
      }

      const auditData = {
        userId,
        username,
        userRole,
        action: actionText,
        timestamp: new Date().toISOString(),
        ipAddress
      };

      const response = await axios.post('/api/AuditLogs', auditData);
      return response.data;
    } catch (error) {
      console.error('Error logging action:', error);
      if (error.response) {
        console.error('Response data:', error.response.data);
        console.error('Response status:', error.response.status);
      }
      // Don't throw the error, just log it and continue
      return null;
    }
  }

  static async getAuditLogs() {
    try {
      const response = await axios.get('/api/AuditLogs');
      return response.data;
    } catch (error) {
      console.error('Error fetching audit logs:', error);
      if (error.response?.data?.message) {
        throw new Error(error.response.data.message);
      }
      throw error;
    }
  }
}

export default AuditService; 