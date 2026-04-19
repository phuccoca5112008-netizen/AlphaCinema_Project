import api from './axios';

export const adminApi = {
  // User Management
  getUsers: () => api.get('/nguoi-dung'),
  getUser: (id) => api.get(`/nguoi-dung/${id}`),
  updateUser: (id, data) => api.put(`/nguoi-dung/${id}`, data),
  deleteUser: (id) => api.delete(`/nguoi-dung/${id}`),
  
  // Room Management
  getRooms: () => api.get('/phong-chieu'),
  getRoomDetail: (id) => api.get(`/phong-chieu/${id}`),
  createRoom: (data) => api.post('/phong-chieu', data),
  updateRoom: (id, data) => api.put(`/phong-chieu/${id}`, data),
  deleteRoom: (id) => api.delete(`/phong-chieu/${id}`),
  
  // Seat Management
  generateSeats: (id, data) => api.post(`/phong-chieu/${id}/ghe/generate`, data),
  updateSeat: (id, data) => api.put(`/phong-chieu/ghe/${id}`, data),
  
  // Statistics/Dashboard
  getRevenueStats: (tuNgay, denNgay) => api.get(`/hoa-don/doanh-thu?tuNgay=${tuNgay}&denNgay=${denNgay}`),
};
