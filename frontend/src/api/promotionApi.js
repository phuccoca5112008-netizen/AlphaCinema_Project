import api from './axios';

export const promotionApi = {
  getPromotions: () => api.get('/khuyen-mai'),
  getRewards: () => api.get('/thanh-vien/rewards'),
  redeemReward: (maPhanThuong) => api.post('/thanh-vien/redeem', { maPhanThuong }),
  getRewardHistory: () => api.get('/thanh-vien/reward-history'),
  applyPromo: (maCode, tongTienGoc) => api.post('/khuyen-mai/ap-dung', { maCode, tongTienGoc }),
  createPromotion: (data) => api.post('/khuyen-mai', data),
  updatePromotion: (id, data) => api.put(`/khuyen-mai/${id}`, data),
  deletePromotion: (id) => api.delete(`/khuyen-mai/${id}`),
};
