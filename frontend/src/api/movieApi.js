import api from './axios';

export const movieApi = {
  getMovies: (params = {}) => {
    let url = '/phim?';
    if (params.tuKhoa) url += `tuKhoa=${params.tuKhoa}&`;
    if (params.trangThai) url += `trangThai=${params.trangThai}&`;
    return api.get(url);
  },
  getMovieDetail: (id) => api.get(`/phim/${id}`),
  createMovie: (data) => api.post('/phim', data),
  updateMovie: (id, data) => api.put(`/phim/${id}`, data),
  deleteMovie: (id) => api.delete(`/phim/${id}`),
  seedMovies: () => api.post('/phim/seed'),
  
  // Reviews
  getReviews: () => api.get('/danh-gia'),
  getMovieReviews: (movieId) => api.get(`/danh-gia/phim/${movieId}`),
  checkReviewEligibility: (movieId) => api.get(`/danh-gia/check-eligibility/${movieId}`),
  submitReview: (data) => api.post('/danh-gia', data),
  deleteReview: (id) => api.delete(`/danh-gia/${id}`),
};
