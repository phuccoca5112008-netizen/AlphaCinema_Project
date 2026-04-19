<template>
  <div class="profile-page">
    <div class="profile-container container">
      <!-- Profile Header (Mobile Friendly Summary) -->
      <header class="profile-header glass-panel mb-4">
        <div class="user-info-banner">
          <div class="avatar-container">
            <div class="avatar-glow"></div>
            <div class="avatar-circle">
              {{ authStore.user?.hoTen?.charAt(0) || 'U' }}
            </div>
          </div>
          <div class="user-text">
            <h1 class="gradient-text">{{ authStore.user?.hoTen }}</h1>
            <p class="user-email">{{ authStore.user?.email }}</p>
            <div class="user-badges">
              <span class="badge user-tier-badge" :class="profileData?.hangThanhVien?.toLowerCase().replace(' ', '-')">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3"><path d="M6 9l6 6 6-6"/></svg>
                {{ profileData?.hangThanhVien || 'BẠC' }}
              </span>
              <span class="badge points-badge">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><circle cx="12" cy="12" r="8"/><path d="M12 8l1.5 3 3.5.5-2.5 2.5 1 3.5-3.5-2-3.5 2 1-3.5-2.5-2.5 3.5-.5z"/></svg>
                {{ profileData?.diemTichLuy || 0 }} ĐIỂM
              </span>
            </div>
          </div>
        </div>

        <nav class="profile-nav-tabs">
          <button @click="currentTab = 'info'" :class="{ active: currentTab === 'info' }">Cá nhân</button>
          <button @click="currentTab = 'history'" :class="{ active: currentTab === 'history' }">Lịch sử</button>
          <button @click="currentTab = 'rewards'" :class="{ active: currentTab === 'rewards' }">Ưu đãi</button>
        </nav>
      </header>

      <div class="profile-wrapper">
        <!-- Sidebar Navigation (Desktop) -->
        <aside class="profile-sidebar glass-panel">
          <div class="sidebar-top">
             <div class="sidebar-user-avatar">{{ authStore.user?.hoTen?.charAt(0) || 'U' }}</div>
             <h3>{{ authStore.user?.hoTen }}</h3>
             <small>{{ profileData?.hangThanhVien || 'Hạng Bạc' }}</small>
          </div>
          <nav class="sidebar-menu">
            <button class="menu-item" :class="{ active: currentTab === 'info' }" @click="currentTab = 'info'">
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path><circle cx="12" cy="7" r="4"></circle></svg>
              Hồ sơ của tôi
            </button>
            <button class="menu-item" :class="{ active: currentTab === 'history' }" @click="currentTab = 'history'">
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"></rect><line x1="16" y1="2" x2="16" y2="6"></line><line x1="8" y1="2" x2="8" y2="6"></line><line x1="3" y1="10" x2="21" y2="10"></line></svg>
              Lịch sử đặt vé
            </button>
            <button class="menu-item" :class="{ active: currentTab === 'rewards' }" @click="currentTab = 'rewards'">
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="20 12 20 22 4 22 4 12"></polyline><rect x="2" y="7" width="20" height="5"></rect><line x1="12" y1="22" x2="12" y2="7"></line><path d="M12 7H7.5a2.5 2.5 0 0 1 0-5C11 2 12 7 12 7z"></path><path d="M12 7h4.5a2.5 2.5 0 0 0 0-5C13 2 12 7 12 7z"></path></svg>
              Đổi quà & Ưu đãi
            </button>
            <div class="sidebar-divider"></div>
            <button class="menu-item logout-btn" @click="logout">
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"></path><polyline points="16 17 21 12 16 7"></polyline><line x1="21" y1="12" x2="9" y2="12"></line></svg>
              Đăng xuất
            </button>
          </nav>
        </aside>

        <!-- Main Content Area -->
        <main class="profile-content">
          <!-- SECTION: PERSONAL INFO -->
          <Transition name="slide-fade">
            <section v-if="currentTab === 'info'" class="content-section glass-panel p-4">
              <h2 class="section-title">Thiết lập tài khoản</h2>
              <form @submit.prevent="updateProfile" class="modern-form">
                <div class="form-grid">
                  <div class="form-group full-width">
                    <label class="form-label">Họ và tên</label>
                    <div class="input-wrapper">
                      <input type="text" v-model="form.hoTen" class="form-input" placeholder="Nhập tên của bạn..." required />
                      <svg class="input-icon" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></svg>
                    </div>
                  </div>
                  
                  <div class="form-divider text-muted uppercase">Bảo mật tài khoản</div>

                  <div class="form-group">
                    <label class="form-label">Mật khẩu hiện tại</label>
                    <div class="input-wrapper">
                      <input type="password" v-model="form.matKhauCu" class="form-input" placeholder="••••••••" />
                      <svg class="input-icon" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="11" width="18" height="11" rx="2" ry="2"/><path d="M7 11V7a5 5 0 0 1 10 0v4"/></svg>
                    </div>
                  </div>

                  <div class="form-group">
                    <label class="form-label">Mật khẩu mới</label>
                    <div class="input-wrapper">
                      <input type="password" v-model="form.matKhauMoi" class="form-input" placeholder="Tối thiểu 6 ký tự" />
                      <svg class="input-icon" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M21 2l-2 2m-7.61 7.61a5.5 5.5 0 1 1-7.778 7.778 5.5 5.5 0 0 1 7.778-7.778zM12 12l.93-2.015L15 9l1.07-2.015L18 6l-1.07-2.015L15 2 13.93 3.985 12 5z"/></svg>
                    </div>
                  </div>
                </div>

                <div class="form-actions mt-4">
                  <button type="submit" class="btn btn-primary" :disabled="loading">
                    <span v-if="loading" class="spinner-small"></span>
                    {{ loading ? 'Đang cập nhật...' : 'Cập nhật hồ sơ' }}
                  </button>
                </div>
                
                <div v-if="updateMsg" class="alert alert-success mt-3 animate-in">
                  <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>
                  {{ updateMsg }}
                </div>
                <div v-if="updateErr" class="alert alert-error mt-3 animate-in">
                  <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg>
                  {{ updateErr }}
                </div>
              </form>
            </section>
          </Transition>

          <!-- SECTION: BOOKING HISTORY -->
          <Transition name="slide-fade">
            <section v-if="currentTab === 'history'" class="content-section">
              <div class="section-top mb-4">
                <h2 class="section-title">Lịch sử giao dịch</h2>
                <span class="text-muted">{{ invoices.length }} giao dịch gần nhất</span>
              </div>

              <div v-if="loadingHistory" class="loading-state glass-panel p-5">
                <div class="spinner-large"></div>
                <p>Đang tìm dữ liệu vé của bạn...</p>
              </div>
              

              <div v-else class="history-grid">
                <div v-if="invoices.length === 0" class="empty-state glass-panel">
                  <div class="empty-icon">🎟️</div>
                  <p>Bạn chưa có cuộc phiêu lưu điện ảnh nào.</p>
                  <router-link to="/movies" class="btn btn-outline mt-3">Đặt vé ngay</router-link>
                </div>

        <!-- TICKET CARDS -->
        <article v-for="inv in invoices" :key="inv.maHoaDon" class="ticket-card" :class="{ 'status-used': inv.trangThai === 'Đã sử dụng' }">
          <div class="ticket-main">
            <div class="ticket-header">
              <div class="movie-info">
                <h3>{{ inv.tenPhim }}</h3>
                <span class="booking-date">{{ formatDate(inv.ngayGiaoDich) }}</span>
              </div>
              <div class="ticket-id">#{{ inv.maHoaDon }}</div>
            </div>
            <div class="ticket-body">
              <div class="ticket-info-row">
                <div class="info-item">
                  <label>Chỗ ngồi</label>
                  <span>{{ inv.danhSachGhe }}</span>
                </div>
                <div class="info-item">
                  <label>Tổng tiền</label>
                  <span class="price-text">{{ formatPrice(inv.tongTien) }}đ</span>
                </div>
              </div>

              <!-- New Concessions Row -->
              <div class="ticket-info-row" v-if="inv.danhSachDoAn">
                <div class="info-item full">
                  <label>Bắp nước đi kèm</label>
                  <span class="concession-text">🍿 {{ inv.danhSachDoAn }}</span>
                </div>
              </div>

              <div class="ticket-info-row" v-if="inv.maVaoCong">
                <div class="info-item full">
                  <label>Mã vào cổng (Để quét tại rạp)</label>
                  <div class="access-code-box clickable-qr" @click="openTicket(inv)">
                    <div class="qr-mini">
                      <qrcode-vue :value="inv.maVaoCong" :size="60" level="H" background="#fff" foreground="#000" />
                    </div>
                    <div class="code-info">
                      <span class="code-text">{{ inv.maVaoCong }}</span>
                      <small>Nhấn để phóng to QR</small>
                    </div>
                    <i class="fas fa-expand-arrows-alt expand-icon"></i>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="ticket-side">
            <div class="ticket-badge" :class="inv.trangThai === 'Đã sử dụng' ? 'used' : 'valid'">
              {{ inv.trangThai || 'Hợp lệ' }}
            </div>
            <div class="ticket-cutout-top"></div>
            <div class="ticket-cutout-bottom"></div>
          </div>
        </article>
      </div>

      <!-- TICKET MODAL -->
      <Transition name="modal">
        <div v-if="showTicketDetail" class="ticket-modal-overlay" @click.self="closeTicket">
          <div ref="ticketRef" class="ticket-detail-modal glass-panel animate-scale-up">
            <button class="close-ticket" @click="closeTicket"><i class="fas fa-times"></i></button>
            <div class="modal-ticket-content">
              <div class="modal-top">
                <h2 class="gradient-text">VÉ VÀO CỔNG</h2>
                <p>Vui lòng đưa mã này cho nhân viên soát vé</p>
              </div>
              
              <div class="qr-main-container">
                <div class="qr-border">
                  <qrcode-vue :value="activeTicket.maVaoCong" :size="240" level="H" render-as="svg" background="transparent" foreground="#fff" />
                </div>
                <div class="qr-code-text">{{ activeTicket.maVaoCong }}</div>
              </div>

              <div class="modal-ticket-info">
                <h3>{{ activeTicket.tenPhim }}</h3>
                <div class="info-badges">
                  <span><i class="far fa-calendar-alt"></i> {{ formatDate(activeTicket.ngayGiaoDich).split(' ')[1] }}</span>
                  <span><i class="far fa-clock"></i> {{ formatDate(activeTicket.ngayGiaoDich).split(' ')[0] }}</span>
                  <span><i class="fas fa-couch"></i> {{ activeTicket.danhSachGhe }}</span>
                </div>
              </div>
              
              <div class="modal-footer">
                <div class="footer-left">
                    <p>Giao dịch: #{{ activeTicket.maHoaDon }}</p>
                    <div class="brand-logo">ALPHA<span class="text-primary">CINEMA</span></div>
                </div>
                <button class="btn-download-ticket" @click="downloadTicket" :disabled="isSaving">
                    <i v-if="isSaving" class="fas fa-spinner fa-spin"></i>
                    <i v-else class="fas fa-download"></i>
                    {{ isSaving ? 'ĐANG LƯU...' : 'LƯU VÉ' }}
                </button>
              </div>
            </div>
          </div>
        </div>
      </Transition>
    </section>
  </Transition>

          <!-- SECTION: REWARDS -->
          <Transition name="slide-fade">
            <section v-if="currentTab === 'rewards'" class="content-section">
               <header class="rewards-intro glass-panel p-4 mb-4">
                 <div class="intro-text">
                   <h2 class="section-title">Alpha Cinema Rewards</h2>
                   <p class="text-sub">Đổi ngay điểm tích lũy của bạn để nhận những phần quà cực hấp dẫn: bắp, nước, voucher, vé miễn phí...</p>
                 </div>
                 <div class="user-points-card">
                    <span class="points-label">ĐIỂM HIỆN CÓ</span>
                    <span class="points-value">{{ profileData?.diemTichLuy || 0 }}</span>
                 </div>
               </header>

               <nav class="rewards-sub-nav mb-5">
                  <button class="sub-nav-item" :class="{ active: currentRewardSubTab === 'exchange' }" @click="currentRewardSubTab = 'exchange'">
                    <i class="fas fa-gift me-2"></i> Đổi Phần Thưởng
                  </button>
                  <button class="sub-nav-item" :class="{ active: currentRewardSubTab === 'history' }" @click="currentRewardSubTab = 'history'">
                    <i class="fas fa-history me-2"></i> Lịch Sử Ưu Đãi
                  </button>
               </nav>

               <div v-if="currentRewardSubTab === 'exchange'">
                 <div class="reward-tabs mb-4">
                   <button 
                    v-for="cat in ['Tất cả', 'Combo', 'Voucher', 'Ticket', 'Khác']" 
                    :key="cat" 
                    class="cat-chip" 
                    :class="{ active: selectedCategory === cat }"
                    @click="selectedCategory = cat"
                   >
                     {{ cat }}
                   </button>
                 </div>

                 <div v-if="loadingRewards" class="loading-state">
                    <div class="spinner-large"></div>
                 </div>
                 
                 <div v-else class="rewards-container">
                    <div v-for="reward in filteredRewards" :key="reward.maPhanThuong" class="reward-item-premium" :class="{ 'disabled': profileData?.diemTichLuy < reward.diemYeuCau }">
                      <div class="reward-image-box">
                        <img :src="reward.hinhAnh" :alt="reward.tenPhanThuong">
                        <div class="reward-floating-cost">{{ reward.diemYeuCau }} <small>Pts</small></div>
                        <div class="reward-type-tag">{{ reward.loaiPhanThuong }}</div>
                      </div>
                      <div class="reward-info">
                        <h3>{{ reward.tenPhanThuong }}</h3>
                        <p>{{ reward.moTa }}</p>
                        
                        <div class="reward-footer">
                          <div class="reward-progress" v-if="profileData?.diemTichLuy < reward.diemYeuCau">
                             <div class="progress-track">
                               <div class="progress-fill" :style="{ width: (profileData?.diemTichLuy / reward.diemYeuCau * 100) + '%' }"></div>
                             </div>
                             <span class="progress-note">Cần thêm {{ reward.diemYeuCau - profileData?.diemTichLuy }} điểm</span>
                          </div>
                          <button 
                            class="btn-redeem" 
                            :disabled="profileData?.diemTichLuy < reward.diemYeuCau || redeemingId === reward.maPhanThuong"
                            @click="handleRedeem(reward)"
                          >
                            {{ redeemingId === reward.maPhanThuong ? 'Đang quy đổi...' : (profileData?.diemTichLuy >= reward.diemYeuCau ? 'ĐỔI NGAY' : 'CHƯA ĐỦ ĐIỂM') }}
                          </button>
                        </div>
                      </div>
                    </div>
                 </div>
               </div>

               <!-- REWARD HISTORY SUBTAB -->
               <div v-else class="reward-history-section">
                  <div v-if="loadingHistory" class="loading-state">
                    <div class="spinner-large"></div>
                  </div>
                  <div v-else-if="rewardHistory.length === 0" class="empty-state glass-panel p-5">
                    <i class="fas fa-ticket-alt opacity-20 fa-3x mb-3"></i>
                    <p>Bạn chưa có mã ưu đãi nào. Hãy dùng điểm để đổi nhé!</p>
                  </div>
                  <div v-else class="reward-history-grid">
                    <div v-for="item in rewardHistory" :key="item.maKhuyenMai" class="history-promo-card glass-panel">
                       <div class="promo-header">
                          <div class="promo-type">
                            <span v-if="item.giaTriGiam > 0">🧧 {{ item.loaiGiamGia === 'PhanTram' ? 'PHẦN TRĂM' : 'TIỀM MẶT' }}</span>
                            <span v-else>🎁 QUÀ TẶNG</span>
                          </div>
                          <div class="promo-date">HSD: {{ formatDate(item.ngayKetThuc).split(' ')[1] }}</div>
                       </div>
                       <h3 class="promo-title">{{ item.tenKhuyenMai }}</h3>
                       <p class="promo-desc">{{ item.moTa }}</p>
                       <div class="promo-code-container">
                          <span class="code-label">MÃ QUÀ TẶNG:</span>
                          <div class="code-box">
                             <span class="code-val">{{ item.maCodeGiamGia }}</span>
                             <button class="btn-copy" @click="copyCode(item.maCodeGiamGia)" title="Sao chép">
                                <i class="far fa-copy"></i>
                             </button>
                          </div>
                       </div>
                       <div class="promo-value" v-if="item.giaTriGiam > 0">
                          <span class="v-label">GIÁ TRỊ:</span>
                          <span class="v-amount">{{ item.giaTriGiam.toLocaleString() }}{{ item.loaiGiamGia === 'PhanTram' ? '%' : 'đ' }}</span>
                       </div>
                    </div>
                  </div>
               </div>

               <section class="points-guide glass-panel p-4 mt-5">
                 <h3>Làm thế nào để tích điểm?</h3>
                 <div class="guide-steps">
                    <div class="guide-step">
                      <div class="step-icon">🍿</div>
                      <div class="step-content">
                        <h4>Xem phim thả ga</h4>
                        <p>Nhận ngay 10 điểm cho mỗi 100.000đ khi mua vé.</p>
                      </div>
                    </div>
                    <div class="guide-step">
                      <div class="step-icon">🥤</div>
                      <div class="step-content">
                        <h4>Ăn bắp uống nước</h4>
                        <p>Dịch vụ Concession cũng được tích điểm tương tự.</p>
                      </div>
                    </div>
                    <div class="guide-step">
                      <div class="step-icon">🌟</div>
                      <div class="step-content">
                        <h4>Thăng hạng hội viên</h4>
                        <p>Hạng càng cao, tỉ lệ nhận điểm bonus càng lớn.</p>
                      </div>
                    </div>
                 </div>
               </section>
            </section>
          </Transition>
        </main>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useAuthStore } from '../../stores/auth';
import { useToastStore } from '../../stores/toast';
import { useRouter } from 'vue-router';
import { authApi } from '../../api/authApi';
import { bookingApi } from '../../api/bookingApi';
import { promotionApi } from '../../api/promotionApi';
import QrcodeVue from 'qrcode.vue';
import html2canvas from 'html2canvas';

const authStore = useAuthStore();
const toast = useToastStore();
const router = useRouter();

const currentTab = ref('info');
const currentRewardSubTab = ref('exchange'); // exchange | history
const profileData = ref(null);
const invoices = ref([]);
const rewards = ref([]);
const rewardHistory = ref([]);
const selectedCategory = ref('Tất cả');

const loading = ref(false);
const loadingHistory = ref(false);
const loadingRewards = ref(false);
const redeemingId = ref(null);
const updateMsg = ref('');
const updateErr = ref('');
const isSaving = ref(false);

// Ticket Modal
const showTicketDetail = ref(false);
const activeTicket = ref(null);
const ticketRef = ref(null);

const openTicket = (inv) => {
  activeTicket.value = inv;
  showTicketDetail.value = true;
};

const closeTicket = () => {
  showTicketDetail.value = false;
};

const downloadTicket = async () => {
  if (!ticketRef.value) return;
  isSaving.value = true;
  try {
    const canvas = await html2canvas(ticketRef.value, {
      scale: 3, // Higher scale for scanning clarity
      useCORS: true,
      backgroundColor: "#111",
      logging: false
    });
    const link = document.createElement('a');
    link.download = `AlphaCinema-Ticket-${activeTicket.value.maHoaDon}.png`;
    link.href = canvas.toDataURL('image/png');
    link.click();
  } catch (err) {
    console.error("Download error:", err);
    alert("Không thể tải ảnh vé. Vui lòng thử lại!");
  } finally {
    isSaving.value = false;
  }
};

const form = ref({
  hoTen: '',
  matKhauCu: '',
  matKhauMoi: ''
});

const filteredRewards = computed(() => {
  if (selectedCategory.value === 'Tất cả') return rewards.value;
  if (selectedCategory.value === 'Khác') {
    return rewards.value.filter(r => !['Combo', 'Voucher', 'Ticket'].includes(r.loaiPhanThuong));
  }
  return rewards.value.filter(r => r.loaiPhanThuong === selectedCategory.value);
});

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  });
};

const formatPrice = (price) => {
  return price?.toLocaleString('vi-VN');
};

const loadProfile = async () => {
  try {
    const res = await authApi.getProfile();
    if (res.success) {
      profileData.value = res.data;
      form.value.hoTen = profileData.value.hoTen;
    }
  } catch (e) { console.error(e); }
};

const loadHistory = async () => {
  loadingHistory.value = true;
  try {
    const res = await bookingApi.getMyTickets();
    if (res.success) {
      invoices.value = res.data.sort((a,b) => new Date(b.ngayGiaoDich) - new Date(a.ngayGiaoDich));
    }
  } catch (e) { console.error(e); }
  finally { loadingHistory.value = false; }
};

const loadRewards = async () => {
  loadingRewards.value = true;
  try {
    const res = await promotionApi.getRewards();
    if (res.success) { rewards.value = res.data; }
  } catch (e) { console.error(e); }
  finally { loadingRewards.value = false; }
};

const handleRedeem = async (reward) => {
  if (!confirm(`Bạn có chắc chắn muốn dùng ${reward.diemYeuCau} điểm để đổi "${reward.tenPhanThuong}"?`)) return;
  redeemingId.value = reward.maPhanThuong;
  try {
    const res = await promotionApi.redeemReward(reward.maPhanThuong);
    if (res.success) {
      toast.add(`Quy đổi thành công: ${reward.tenPhanThuong}! Mã quả tặng đã được lưu vào lịch sử.`, 'success');
      loadProfile();
      loadRewardHistory();
      currentRewardSubTab.value = 'history';
    } else { toast.add(res.message, 'error'); }
  } catch (e) { toast.add(e.message || 'Có lỗi xảy ra khi quy đổi.', 'error'); }
  finally { redeemingId.value = null; }
};

const loadRewardHistory = async () => {
    try {
        const res = await promotionApi.getRewardHistory();
        if (res.success) {
            rewardHistory.value = res.data;
        }
    } catch (e) { console.error(e); }
};

const copyCode = (code) => {
    navigator.clipboard.writeText(code);
    toast.add('Đã sao chép mã ưu đãi!', 'success');
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
    const res = await authApi.updateProfile(payload);
    if (res.success) {
      updateMsg.value = 'Cập nhật thông tin thành công!';
      authStore.user.hoTen = form.value.hoTen;
      form.value.matKhauCu = '';
      form.value.matKhauMoi = '';
    }
  } catch (error) { updateErr.value = error.message || 'Thất bại. Kiểm tra lại mật khẩu cũ.'; }
  finally { loading.value = false; }
};

const logout = () => {
  authStore.logout();
  router.push('/login');
};

onMounted(() => {
  loadProfile();
  loadHistory();
  loadRewards();
  loadRewardHistory();
});
</script>

<style scoped>
.profile-page {
  padding: 6rem 0;
  min-height: 100vh;
  background: radial-gradient(circle at 10% 20%, rgba(232, 136, 42, 0.05) 0%, transparent 40%),
              radial-gradient(circle at 90% 80%, rgba(214, 48, 49, 0.05) 0%, transparent 40%);
}

.profile-container {
  display: flex;
  flex-direction: column;
}

/* ─── Profile Header (Mobile Layout) ─── */
.profile-header {
  padding: 1.5rem;
  display: none; /* Only visible on smaller screens or if specifically wanted */
  border-radius: var(--radius-lg);
}

.user-info-banner {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  margin-bottom: 1.5rem;
}

.avatar-container {
  position: relative;
  width: 70px;
  height: 70px;
}

.avatar-circle {
  width: 100%; height: 100%;
  background: var(--color-primary);
  border-radius: 50%;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.8rem; font-weight: 800; color: white;
  position: relative; z-index: 2;
}

.avatar-glow {
  position: absolute; inset: -5px;
  background: var(--color-primary);
  filter: blur(15px); opacity: 0.3;
  border-radius: 50%;
}

.user-text h1 { font-size: 1.4rem; margin-bottom: 0.2rem; }
.user-email { color: var(--color-text-sub); font-size: 0.9rem; margin-bottom: 0.5rem; }
.user-badges { display: flex; gap: 0.5rem; }

.badge {
  display: inline-flex; align-items: center; gap: 4px;
  padding: 4px 10px; border-radius: 6px; font-size: 0.75rem; font-weight: 800;
}
.points-badge { background: rgba(249, 202, 36, 0.1); color: var(--color-accent); border: 1px solid var(--color-accent); }
.user-tier-badge { background: rgba(255, 255, 255, 0.1); color: white; border: 1px solid rgba(255, 255, 255, 0.2); }
.user-tier-badge.vàng { color: gold; border-color: gold; }
.user-tier-badge.kim-cương { color: #00d4ff; border-color: #00d4ff; }

/* Desktop Wrapper Layout */
.profile-wrapper {
  display: grid;
  grid-template-columns: 280px 1fr;
  gap: 2rem;
  align-items: start;
}

/* ─── Sidebar ─── */
.profile-sidebar {
  padding: 2rem 0;
  height: fit-content;
  position: sticky; top: 100px;
}

.sidebar-top {
  text-align: center;
  margin-bottom: 2rem;
  padding: 0 1.5rem;
}

.sidebar-user-avatar {
  width: 80px; height: 80px;
  background: linear-gradient(135deg, var(--color-primary), var(--color-secondary));
  border-radius: 50%;
  margin: 0 auto 1rem;
  display: flex; align-items: center; justify-content: center;
  font-size: 2rem; font-weight: 900; color: white;
  box-shadow: 0 8px 16px rgba(0,0,0,0.3);
}

.sidebar-top h3 { font-size: 1.2rem; margin-bottom: 0.2rem; }
.sidebar-top small { text-transform: uppercase; letter-spacing: 1px; color: var(--color-primary); font-weight: 800; font-size: 0.7rem; }

.sidebar-menu { display: flex; flex-direction: column; }
.menu-item {
  display: flex; align-items: center; gap: 1rem;
  padding: 1rem 1.5rem;
  background: none; border: none;
  color: var(--color-text-sub);
  font-family: var(--font-family);
  font-size: 1rem; font-weight: 500;
  cursor: pointer; transition: 0.3s;
  text-align: left;
}
.menu-item:hover { color: white; background: rgba(255,255,255,0.03); }
.menu-item.active {
  color: var(--color-primary);
  background: rgba(232, 136, 42, 0.08);
  border-right: 3px solid var(--color-primary);
  font-weight: 700;
}
.sidebar-divider { height: 1px; background: rgba(255,255,255,0.05); margin: 1rem 0; }
.logout-btn:hover { color: var(--color-error); }

/* ─── Main Content ─── */
.content-section {
  animation: fadeIn 0.5s ease;
}

.section-title {
  font-size: 1.6rem;
  color: white;
  margin-bottom: 1.5rem;
}

/* Forms */
.modern-form { display: flex; flex-direction: column; gap: 1.5rem; }
.form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 1.5rem; }
.form-grid .full-width { grid-column: span 2; }
.form-divider { grid-column: span 2; margin: 1rem 0 0.5rem; font-size: 0.75rem; letter-spacing: 2px; }

.input-wrapper { position: relative; }
.input-icon { position: absolute; left: 1rem; top: 50%; transform: translateY(-50%); color: var(--color-text-muted); pointer-events: none;}
.form-input { padding-left: 3rem; }

.alert {
  padding: 1rem; border-radius: 8px; display: flex; align-items: center; gap: 10px; font-weight: 600;
}
.alert-success { background: rgba(0, 184, 148, 0.1); color: var(--color-success); border: 1px solid rgba(0, 184, 148, 0.2); }
.alert-error { background: rgba(214, 48, 49, 0.1); color: var(--color-error); border: 1px solid rgba(214, 48, 49, 0.2); }

/* History Tickets */
.history-grid { display: flex; flex-direction: column; gap: 1.5rem; }

.ticket-card {
  display: flex;
  background: var(--color-bg-card);
  border-radius: 12px; border: 1px solid rgba(255,255,255,0.05);
  overflow: hidden;
  transition: 0.3s;
}
.ticket-card:hover { transform: scale(1.01); border-color: var(--color-primary); }

.ticket-main { flex: 1; padding: 1.5rem; }
.ticket-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 1.2rem; }
.ticket-header h3 { font-size: 1.3rem; color: white; margin-bottom: 4px; }
.booking-date { font-size: 0.8rem; color: var(--color-text-muted); }
.ticket-id { font-family: monospace; color: var(--color-primary); font-weight: 700; opacity: 0.7; }

.ticket-info-row { display: flex; gap: 2rem; margin-bottom: 1rem; }
.info-item label { display: block; font-size: 0.65rem; color: var(--color-text-sub); text-transform: uppercase; margin-bottom: 4px; letter-spacing: 1px;}
.info-item span { font-weight: 700; color: #fff; font-size: 1.1rem; }
.price-text { color: var(--color-accent) !important; }

.access-code-box {
  background: rgba(0,0,0,0.3); padding: 1.2rem; border-radius: 12px;
  display: flex; justify-content: space-between; align-items: center;
  border: 1px dashed rgba(255,255,255,0.15); margin-top: 10px;
}

.clickable-qr {
  cursor: pointer;
  transition: 0.3s;
}

.clickable-qr:hover {
  background: rgba(232, 136, 42, 0.05);
  border-color: var(--color-primary);
}

.qr-mini {
  padding: 8px;
  background: #fff;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.code-info {
  flex: 1;
  padding-left: 1.5rem;
  display: flex;
  flex-direction: column;
}

.code-info small {
  color: var(--color-text-muted);
  font-size: 0.7rem;
  margin-top: 4px;
}

.expand-icon {
  color: var(--color-text-muted);
  font-size: 1rem;
  opacity: 0.5;
}

.code-text { font-family: 'Courier New', Courier, monospace; font-size: 1.6rem; font-weight: 950; letter-spacing: 4px; color: #fff; }

.concession-text { font-size: 0.95rem; color: #ff9f43; font-weight: 600; display: block; margin-top: 4px; }
.info-item.full { width: 100%; }

/* TICKET MODAL STYLES */
.ticket-modal-overlay {
  position: fixed; inset: 0;
  background: rgba(0,0,0,0.85); backdrop-filter: blur(10px);
  z-index: 9999; display: flex; align-items: center; justify-content: center; padding: 2rem;
}

.ticket-detail-modal {
  width: 100%; max-width: 450px;
  padding: 0; position: relative;
  overflow: hidden; border: 1px solid rgba(255,255,255,0.1);
  box-shadow: 0 40px 100px rgba(0,0,0,0.5);
}

.modal-ticket-content { padding: 3rem 2rem; text-align: center; }

.modal-top { margin-bottom: 2rem; }
.modal-top h2 { font-size: 1.8rem; font-weight: 900; margin-bottom: 0.5rem; }
.modal-top p { color: var(--color-text-sub); font-size: 0.9rem; }

.qr-main-container {
  background: rgba(255,255,255,0.03); border-radius: 24px;
  padding: 2.5rem; margin-bottom: 2.5rem;
  border: 1px solid rgba(255,255,255,0.05);
}

.qr-border {
  display: inline-block; padding: 15px;
  background: rgba(255,255,255,0.05); border-radius: 12px;
}

.qr-code-text {
  margin-top: 1.5rem; font-family: 'Courier New', Courier, monospace;
  font-size: 1.8rem; font-weight: 900; letter-spacing: 5px; color: var(--color-primary);
}

.modal-ticket-info h3 { font-size: 1.5rem; font-weight: 800; margin-bottom: 1rem; }
.info-badges { display: flex; justify-content: center; gap: 1rem; flex-wrap: wrap; }
.info-badges span {
  display: flex; align-items: center; gap: 6px;
  font-size: 0.85rem; color: var(--color-text-sub);
  background: rgba(255,255,255,0.05); padding: 5px 12px; border-radius: 30px;
}

.modal-footer {
  margin-top: 3rem; border-top: 1px solid rgba(255,255,255,0.05);
  padding-top: 1.5rem; display: flex; justify-content: space-between; align-items: center;
}
.footer-left { text-align: left; }
.modal-footer p { font-size: 0.75rem; color: var(--color-text-muted); margin: 0; }
.brand-logo { font-weight: 900; font-size: 0.8rem; letter-spacing: 1px; }

.btn-download-ticket {
  background: var(--color-primary);
  border: none;
  color: white;
  padding: 0.7rem 1.4rem;
  border-radius: 12px;
  font-weight: 800;
  font-size: 0.85rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 10px;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: 0 4px 15px rgba(232, 136, 42, 0.3);
}

.btn-download-ticket:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(232, 136, 42, 0.5);
  filter: brightness(1.1);
}

.btn-download-ticket:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.close-ticket {
  position: absolute; top: 1.5rem; right: 1.5rem;
  width: 36px; height: 36px; border-radius: 50%;
  background: rgba(255,255,255,0.1); border: none; color: white;
  cursor: pointer; transition: 0.3s;
}
.close-ticket:hover { background: var(--color-secondary); transform: rotate(90deg); }

/* Transitions */
.modal-enter-active, .modal-leave-active { transition: opacity 0.4s ease; }
.modal-enter-from, .modal-leave-to { opacity: 0; }
@keyframes scaleUp { from { transform: scale(0.9); opacity: 0; } to { transform: scale(1); opacity: 1; } }
.animate-scale-up { animation: scaleUp 0.4s cubic-bezier(0.19, 1, 0.22, 1); }

.ticket-side {
  width: 40px;
  background: #222; border-left: 2px dashed #000;
  display: flex; align-items: center; justify-content: center;
  position: relative;
}
.ticket-badge {
  writing-mode: vertical-rl; transform: rotate(180deg);
  font-size: 0.7rem; font-weight: 900; text-transform: uppercase; letter-spacing: 2px;
}
.ticket-badge.valid { color: var(--color-success); }
.ticket-badge.used { color: var(--color-text-muted); }

.ticket-cutout-top, .ticket-cutout-bottom {
  position: absolute; left: -10px; width: 20px; height: 20px;
  background: var(--color-bg-base); border-radius: 50%;
}
.ticket-cutout-top { top: -10px; }
.ticket-cutout-bottom { bottom: -10px; }

/* Rewards */
.rewards-intro {
  display: flex; justify-content: space-between; align-items: center;
  gap: 2rem;
}
.user-points-card {
  text-align: center; padding: 1rem; background: rgba(232, 136, 42, 0.1); border-radius: var(--radius-md); border: 1px solid rgba(232, 136, 42, 0.2);
}
.points-label { display: block; font-size: 0.6rem; font-weight: 900; color: var(--color-primary); margin-bottom: 2px; letter-spacing: 1px; }
.points-value { font-size: 2rem; font-weight: 950; color: white; line-height: 1; }

.reward-tabs { display: flex; gap: 0.5rem; flex-wrap: wrap; }
.cat-chip {
  padding: 6px 16px; border-radius: 20px; background: rgba(255,255,255,0.05);
  border: 1px solid rgba(255,255,255,0.1); color: var(--color-text-sub);
  font-size: 0.85rem; font-weight: 600; cursor: pointer; transition: 0.3s;
}
.cat-chip:hover { background: rgba(255,255,255,0.1); color: white; }
.cat-chip.active { background: var(--color-primary); color: white; border-color: var(--color-primary); }

.rewards-container {
  display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 1.5rem;
}
.reward-item-premium {
  background: var(--color-bg-card); border-radius: 16px; overflow: hidden;
  border: 1px solid rgba(255,255,255,0.05); transition: 0.4s;
  display: flex; flex-direction: column;
}
.reward-item-premium:hover { transform: translateY(-5px); border-color: var(--color-primary); }

.reward-image-box { position: relative; height: 180px; }
.reward-image-box img { width: 100%; height: 100%; object-fit: cover; }
.reward-floating-cost {
  position: absolute; bottom: 10px; right: 10px;
  background: var(--color-accent); color: black; font-weight: 900;
  padding: 4px 10px; border-radius: 6px; font-size: 0.9rem;
}
.reward-type-tag {
  position: absolute; top: 10px; left: 10px;
  background: rgba(0,0,0,0.6); backdrop-filter: blur(5px);
  color: white; font-size: 0.6rem; font-weight: 800; padding: 3px 8px; border-radius: 4px;
}

.reward-info { padding: 1.2rem; flex: 1; display: flex; flex-direction: column; }
.reward-info h3 { font-size: 1.1rem; margin-bottom: 0.5rem; }
.reward-info p { font-size: 0.85rem; color: var(--color-text-sub); line-height: 1.5; margin-bottom: 1.5rem; }

.reward-footer { margin-top: auto; }
.progress-track { height: 4px; background: rgba(255,255,255,0.1); border-radius: 2px; margin-bottom: 5px; }
.progress-fill { height: 100%; background: var(--color-accent); border-radius: 2px; }
.progress-note { font-size: 0.7rem; color: var(--color-accent); font-weight: 600; display: block; margin-bottom: 10px; text-align: center;}

.btn-redeem {
  width: 100%; padding: 12px; border-radius: 8px; border: none;
  font-weight: 800; letter-spacing: 1px; transition: 0.3s;
  background: var(--color-primary); color: white; cursor: pointer;
}
.btn-redeem:disabled { background: rgba(255,255,255,0.1); color: #666; cursor: not-allowed; }
.btn-redeem:not(:disabled):hover { filter: brightness(1.1); transform: scale(1.02); }

.guide-steps { display: grid; grid-template-columns: repeat(3, 1fr); gap: 2rem; margin-top: 1.5rem; }
.guide-step { display: flex; gap: 1rem; align-items: flex-start; }
.step-icon { font-size: 2rem; }
.step-content h4 { font-size: 1rem; margin-bottom: 4px; }
.step-content p { font-size: 0.8rem; color: var(--color-text-sub); margin: 0; }

/* Transitions */
.slide-fade-enter-active { transition: all 0.3s ease-out; }
.slide-fade-enter-from { transform: translateX(20px); opacity: 0; }

@keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } }

@media (max-width: 900px) {
  .profile-wrapper { grid-template-columns: 1fr; }
  .profile-sidebar { display: none; }
  .profile-header { display: block; }
  .form-grid { grid-template-columns: 1fr; }
  .form-grid .full-width { grid-column: span 1; }
  .form-divider { grid-column: span 1; }
  .guide-steps { grid-template-columns: 1fr; }
}
/* Rewards Sub Nav */
.rewards-sub-nav {
  display: flex;
  gap: 1rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
  padding-bottom: 0.5rem;
}

.sub-nav-item {
  background: none; border: none;
  padding: 0.8rem 1.5rem;
  color: var(--color-text-sub);
  font-weight: 600;
  cursor: pointer;
  transition: 0.3s;
  border-radius: 8px 8px 0 0;
}

.sub-nav-item:hover { color: white; background: rgba(255, 255, 255, 0.03); }
.sub-nav-item.active {
  color: var(--color-primary);
  background: rgba(232, 136, 42, 0.08);
  border-bottom: 3px solid var(--color-primary);
}

/* History Promo Card */
.reward-history-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 1.5rem;
}

.history-promo-card {
  padding: 1.5rem;
  border-radius: 16px;
  border: 1px solid rgba(255, 255, 255, 0.08);
  transition: 0.3s;
  position: relative;
  overflow: hidden;
}

.history-promo-card:hover { transform: translateY(-5px); border-color: var(--color-primary); }

.promo-header {
  display: flex; justify-content: space-between; align-items: center;
  margin-bottom: 1rem;
}

.promo-type { font-size: 0.7rem; font-weight: 800; color: var(--color-primary); text-transform: uppercase; }
.promo-date { font-size: 0.75rem; color: var(--color-text-muted); }

.promo-title { font-size: 1.1rem; color: white; margin-bottom: 0.5rem; }
.promo-desc { font-size: 0.85rem; color: var(--color-text-sub); margin-bottom: 1.2rem; line-height: 1.5; }

.promo-code-container {
  background: rgba(0, 0, 0, 0.3);
  padding: 1rem;
  border-radius: 12px;
  margin-bottom: 1.2rem;
  border: 1px dashed rgba(255, 255, 255, 0.1);
}

.code-label { display: block; font-size: 0.65rem; color: var(--color-text-muted); margin-bottom: 5px; }

.code-box {
  display: flex; justify-content: space-between; align-items: center;
}

.code-val { font-family: monospace; font-size: 1.2rem; font-weight: 800; color: #fff; letter-spacing: 1px; }

.btn-copy {
  background: none; border: none; color: var(--color-primary);
  cursor: pointer; transition: 0.2s; padding: 5px;
}
.btn-copy:hover { transform: scale(1.2); }

.promo-value {
  display: flex; justify-content: space-between; align-items: center;
  padding-top: 1rem; border-top: 1px solid rgba(255, 255, 255, 0.05);
}

.v-label { font-size: 0.75rem; color: var(--color-text-muted); }
.v-amount { font-size: 1.2rem; font-weight: 900; color: var(--color-accent); }

@media (max-width: 768px) {
  .profile-wrapper { grid-template-columns: 1fr; }
  .profile-sidebar { display: none; }
  .profile-header { display: block; }
  .reward-history-grid { grid-template-columns: 1fr; }
}
</style>
