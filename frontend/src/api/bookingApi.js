import api from './axios';

export const bookingApi = {
  // Showtimes & Seats
  getShowtimes: (params = {}) => {
    let url = '/suat-chieu?';
    if (params.maPhim) url += `maPhim=${params.maPhim}&`;
    if (params.ngay) url += `ngay=${params.ngay}&`;
    return api.get(url);
  },
  getShowtimeSeats: (id) => api.get(`/suat-chieu/${id}/ghe`),
  createShowtime: (data) => api.post('/suat-chieu', data),
  updateShowtime: (id, data) => api.put(`/suat-chieu/${id}`, data),
  deleteShowtime: (id) => api.delete(`/suat-chieu/${id}`),
  
  // Booking operations
  getConcessions: () => api.get('/dat-ve/concessions'),
  lockSeats: (maSuatChieu, maGheIds) => api.post('/dat-ve/lock', { maSuatChieu, maGheIds }),
  confirmBooking: (data) => api.post('/dat-ve', data),
  checkIn: (code) => api.post('/dat-ve/check-in', { code }),
  
  // Invoices (Tickets)
  getMyTickets: () => api.get('/hoa-don/my'),
  getInvoices: () => api.get('/hoa-don'),
  deleteInvoice: (id) => api.delete(`/hoa-don/${id}`),
};
