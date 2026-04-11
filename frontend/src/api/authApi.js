import api from './axios';

export const authApi = {
  login: (email, password) => api.post('/auth/login', { email, matKhau: password }),
  register: (name, email, password) => api.post('/auth/register', { hoTen: name, email, matKhau: password }),
  getProfile: () => api.get('/nguoi-dung/me'),
  updateProfile: (data) => api.put('/nguoi-dung/me', data),
};
