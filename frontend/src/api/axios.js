import axios from 'axios';
import { useAuthStore } from '../stores/auth';

const api = axios.create({
  baseURL: 'http://localhost:5059/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use(
  (config) => {
    const authStore = useAuthStore();
    if (authStore.token) {
      config.headers['Authorization'] = `Bearer ${authStore.token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

api.interceptors.response.use(
  (response) => {
    // If our backend returns { success: true, data: ... }, we can unwrap here
    // or just return response.data
    return response.data;
  },
  (error) => {
    const authStore = useAuthStore();
    if (error.response && error.response.status === 401) {
      authStore.logout();
      window.location.href = '/login'; // Redirect to login
    }
    return Promise.reject(error.response?.data || error.message);
  }
);

export default api;
