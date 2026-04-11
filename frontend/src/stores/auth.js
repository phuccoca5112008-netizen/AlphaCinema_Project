import { defineStore } from 'pinia';
import { authApi } from '../api/authApi';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: JSON.parse(localStorage.getItem('user')) || null,
    token: localStorage.getItem('token') || null,
  }),
  getters: {
    isAuthenticated: (state) => !!state.token,
    isAdmin: (state) => ['Admin', 'Staff'].includes(state.user?.vaiTro),
    isOnlyAdmin: (state) => state.user?.vaiTro === 'Admin',
    isCustomer: (state) => state.user?.vaiTro === 'Customer',
  },
  actions: {
    async login(email, password) {
      try {
        const response = await authApi.login(email, password);
        if (response.success) {
          const data = response.data;
          this.token = data.token;
          this.user = {
            maNguoiDung: data.maNguoiDung,
            email: data.email,
            hoTen: data.hoTen,
            vaiTro: data.vaiTro
          };
          localStorage.setItem('token', this.token);
          localStorage.setItem('user', JSON.stringify(this.user));
          return { success: true };
        }
      } catch (error) {
        return { success: false, message: error.message || 'Đăng nhập thất bại', errors: error.errors };
      }
    },
    async register(name, email, password) {
      try {
        const response = await authApi.register(name, email, password);
        if (response.success) {
          const data = response.data;
          this.token = data.token;
          this.user = {
            maNguoiDung: data.maNguoiDung,
            email: data.email,
            hoTen: data.hoTen,
            vaiTro: data.vaiTro
          };
          localStorage.setItem('token', this.token);
          localStorage.setItem('user', JSON.stringify(this.user));
          return { success: true };
        }
      } catch (error) {
        return { success: false, message: error.message || 'Đăng ký thất bại', errors: error.errors };
      }
    },
    logout() {
      this.token = null;
      this.user = null;
      localStorage.removeItem('token');
      localStorage.removeItem('user');
    }
  }
});

