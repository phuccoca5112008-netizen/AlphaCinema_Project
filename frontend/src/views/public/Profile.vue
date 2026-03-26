<template>
  <div class="profile-page container">
    <div class="profile-layout glass-panel">
      <!-- Sidebar -->
      <aside class="profile-sidebar">
        <div class="user-summary">
          <div class="avatar-large">{{ authStore.user?.hoTen?.charAt(0) || 'U' }}</div>
          <h3 class="user-fullname">{{ authStore.user?.hoTen }}</h3>
          <p class="user-email">{{ authStore.user?.email }}</p>
          <div class="points-badge">
            ⭐ {{ profileData?.diemTichLuy || 0 }} Điểm
          </div>
        </div>

        <nav class="profile-nav">
          <button 
            class="nav-btn" 
            :class="{ active: currentTab === 'info' }" 
            @click="currentTab = 'info'"
          >
            Thông Tin Cá Nhân
          </button>
          <button 
            class="nav-btn" 
            :class="{ active: currentTab === 'history' }" 
            @click="currentTab = 'history'"
          >
            Lịch Sử Đặt Vé
          </button>
          <button class="nav-btn text-error text-left mt-4" @click="logout">
            Đăng Xuất
          </button>
        </nav>
      </aside>

      <!-- Content Area -->
      <main class="profile-content">
        <!-- TAB: INFO -->
        <section v-if="currentTab === 'info'" class="tab-pane">
          <h2 class="tab-title">Cập Nhật Hồ Sơ</h2>
          <form @submit.prevent="updateProfile" class="profile-form mt-4">
            <div class="form-group">
              <label>Họ Tên</label>
              <input type="text" v-model="form.hoTen" class="form-input" required />
            </div>

            <h4 class="mt-4 mb-2" style="color:#aaa;">Đổi Mật Khẩu (Để trống nếu không đổi)</h4>
            <div class="form-group">
              <label>Mật Khẩu Cũ</label>
              <input type="password" v-model="form.matKhauCu" class="form-input" />
            </div>
            <div class="form-group">
              <label>Mật Khẩu Mới</label>
              <input type="password" v-model="form.matKhauMoi" class="form-input" />
            </div>

            <button type="submit" class="btn btn-primary mt-3" :disabled="loading">
              {{ loading ? 'Đang lưu...' : 'Lưu Thay Đổi' }}
            </button>
            <div v-if="updateMsg" class="mt-2 text-success">{{ updateMsg }}</div>
            <div v-if="updateErr" class="mt-2 text-error">{{ updateErr }}</div>
          </form>
        </section>

        <!-- TAB: HISTORY -->
        <section v-if="currentTab === 'history'" class="tab-pane">
          <h2 class="tab-title">Vé Đã Đặt</h2>
          <div v-if="loadingHistory" class="text-center p-4">Đang tải lịch sử giao dịch...</div>
          <div v-else class="history-list mt-4">
            <div v-if="invoices.length === 0" class="text-center" style="color:#aaa;">
              Bạn chưa có giao dịch nào.
            </div>
            
            <div v-for="inv in invoices" :key="inv.maHoaDon" class="invoice-card glass-panel">
              <div class="inv-header">
                <div>
                  <span class="inv-id">#{{ inv.maHoaDon }}</span>
                  <span class="inv-date">{{ new Date(inv.ngayGiaoDich).toLocaleString('vi-VN') }}</span>
                </div>
                <div class="inv-total">{{ inv.tongTien.toLocaleString() }}đ</div>
              </div>
              <div class="inv-body">
                <div><strong>Phương thức:</strong> {{ inv.phuongThucThanhToan }}</div>
                <div v-if="inv.tenKhuyenMai"><strong>Khuyến mãi:</strong> <span class="vc-badge">{{ inv.tenKhuyenMai }}</span></div>
                
                <div class="inv-movie-detail mt-3">
                  <div class="inv-movie-name">{{ inv.tenPhim }}</div>
                  <div class="inv-movie-seats">Ghế: {{ inv.danhSachGhe }}</div>
                </div>

                <div class="inv-access-code mt-3" v-if="inv.maVaoCong">
                  <span class="label">MÃ VÀO CỔNG:</span>
                  <span class="code">{{ inv.maVaoCong }}</span>
                </div>
              </div>
              <!-- Note: To show precise movie details we would need to fetch Ticket details of this invoice -->
              <div class="inv-footer">
                <span class="badge" :class="inv.trangThai === 'Đã sử dụng' ?'badge-used' : 'badge-success'">
                  {{ inv.trangThai || 'Thành công' }}
                </span>
              </div>
            </div>
          </div>
        </section>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useAuthStore } from '../../stores/auth';
import { useRouter } from 'vue-router';
import api from '../../api/axios';

const authStore = useAuthStore();
const router = useRouter();

const currentTab = ref('info');
const profileData = ref(null);
const invoices = ref([]);

const loading = ref(false);
const loadingHistory = ref(false);
const updateMsg = ref('');
const updateErr = ref('');

const form = ref({
  hoTen: '',
  matKhauCu: '',
  matKhauMoi: ''
});

const loadProfile = async () => {
  try {
    const res = await api.get('/nguoi-dung/me');
    if (res.success) {
      profileData.value = res.data;
      form.value.hoTen = profileData.value.hoTen;
    }
  } catch (e) {
    console.error(e);
  }
};

const loadHistory = async () => {
  loadingHistory.value = true;
  try {
    const res = await api.get('/hoa-don/my');
    if (res.success) {
      invoices.value = res.data.sort((a,b) => new Date(b.ngayGiaoDich) - new Date(a.ngayGiaoDich));
    }
  } catch (e) {
    console.error(e);
  } finally {
    loadingHistory.value = false;
  }
};

const updateProfile = async () => {
  loading.value = true;
  updateMsg.value = '';
  updateErr.value = '';
  try {
    const payload = { hoTen: form.value.hoTen };
    if (form.value.matKhauMoi && form.value.matKhauCu) {
      payload.matKhauMoi = form.value.matKhauMoi;
      payload.matKhauCu = form.value.matKhauCu;
    }
    
    const res = await api.put('/nguoi-dung/me', payload);
    if (res.success) {
      updateMsg.value = 'Cập nhật thông tin thành công!';
      authStore.user.hoTen = form.value.hoTen; // Update local store display
      form.value.matKhauCu = '';
      form.value.matKhauMoi = '';
    }
  } catch (error) {
    updateErr.value = error.message || 'Cập nhật thất bại. Vui lòng kiểm tra lại mật khẩu cũ.';
  } finally {
    loading.value = false;
  }
};

const logout = () => {
  authStore.logout();
  router.push('/login');
};

onMounted(() => {
  loadProfile();
  loadHistory();
});
</script>

<style scoped>
.profile-page {
  padding: 4rem 1rem;
  min-height: calc(100vh - 100px);
}
.profile-layout {
  display: flex;
  max-width: 1000px;
  margin: 0 auto;
  border-radius: var(--radius-lg);
  overflow: hidden;
  box-shadow: 0 10px 30px rgba(0,0,0,0.5);
}
.profile-sidebar {
  width: 280px;
  background: rgba(0,0,0,0.4);
  padding: 2.5rem 1.5rem;
  border-right: 1px solid rgba(255,255,255,0.05);
}
.user-summary {
  text-align: center;
  margin-bottom: 2rem;
  padding-bottom: 2rem;
  border-bottom: 1px solid rgba(255,255,255,0.05);
}
.avatar-large {
  width: 80px; height: 80px;
  border-radius: 50%; background: var(--color-primary);
  margin: 0 auto 1rem;
  display: flex; align-items: center; justify-content: center;
  font-size: 2.5rem; font-weight: bold; color: white;
}
.user-fullname { font-size: 1.2rem; font-weight: bold; margin-bottom: 0.3rem; }
.user-email { font-size: 0.9rem; color: #888; margin-bottom: 1rem; }
.points-badge {
  display: inline-block;
  background: rgba(255, 193, 7, 0.15); border: 1px solid rgba(255,193,7,0.4);
  color: #ffc107; padding: 5px 15px; border-radius: 20px;
  font-weight: 600; font-size: 0.9rem;
}

.profile-nav { display: flex; flex-direction: column; gap: 0.5rem; }
.nav-btn {
  background: none; border: none; padding: 1rem 1.5rem;
  text-align: left; font-size: 1rem; color: #aaa;
  border-radius: 8px; cursor: pointer; transition: 0.3s; font-weight: 500;
}
.nav-btn:hover { background: rgba(255,255,255,0.05); color: white; }
.nav-btn.active { background: rgba(255, 51, 102, 0.1); color: var(--color-primary); font-weight: bold; }
.text-left { text-align: left; }

.profile-content { flex: 1; padding: 2.5rem; background: var(--color-bg-base); }
.tab-title { font-size: 1.6rem; font-weight: 700; border-bottom: 2px solid var(--color-primary); display: inline-block; padding-bottom: 0.5rem; }
.profile-form { max-width: 500px; }

/* Invoices */
.invoice-card { padding: 1.2rem; border-radius: 8px; margin-bottom: 1.2rem; background: rgba(255,255,255,0.02); }
.inv-header { display: flex; justify-content: space-between; border-bottom: 1px solid rgba(255,255,255,0.05); padding-bottom: 0.8rem; margin-bottom: 0.8rem; }
.inv-id { font-weight: bold; font-size: 1.1rem; margin-right: 1rem; color: white; }
.inv-date { font-size: 0.85rem; color: #888; }
.inv-total { font-size: 1.2rem; font-weight: bold; color: var(--color-accent); }
.inv-body { font-size: 0.9rem; color: #ddd; margin-bottom: 1rem; line-height: 1.6; }
.inv-footer { display: flex; justify-content: flex-end; }
.vc-badge { background: rgba(255,255,255,0.1); padding: 2px 6px; border-radius: 4px; font-family: monospace; color: #1e90ff;}
.badge-success { background: rgba(46, 213, 115, 0.15); color: #2ed573; padding: 5px 12px; border-radius: 4px; font-weight: bold; font-size: 0.8rem;}
.text-success { color: #2ed573; font-weight: 500; }
.text-error { color: var(--color-error); font-weight: 500; }
.mt-2 { margin-top: 0.5rem; }
.mt-3 { margin-top: 1rem; }
.mt-4 { margin-top: 1.5rem; }
.mb-2 { margin-bottom: 0.5rem; }

/* History Details */
.inv-movie-name { font-size: 1.1rem; font-weight: 800; color: var(--color-primary); }
.inv-movie-seats { color: #4fc3f7; font-size: 0.9rem; font-weight: 600; }

.inv-access-code {
  background: rgba(255,255,255,0.05); padding: 0.75rem 1rem; border-radius: 8px;
  display: flex; justify-content: space-between; align-items: center;
  border: 1px dashed rgba(255,255,255,0.1);
}
.inv-access-code .label { font-size: 0.7rem; font-weight: 800; color: #888; letter-spacing: 1px; }
.inv-access-code .code { font-family: monospace; font-size: 1.1rem; font-weight: 900; color: #fff; letter-spacing: 2px; }

.badge-used { background: rgba(255,255,255,0.1); color: #888; border: 1px solid rgba(255,255,255,0.1); padding: 5px 12px; border-radius: 4px; font-weight: bold; font-size: 0.8rem; }

@media (max-width: 768px) {
  .profile-layout { flex-direction: column; }
  .profile-sidebar { width: 100%; border-right: none; border-bottom: 1px solid rgba(255,255,255,0.05); }
}
</style>
