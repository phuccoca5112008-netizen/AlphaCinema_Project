<template>
  <div class="ticket-check-page">
    <div class="page-header">
      <div class="header-icon">🎟️</div>
      <h2 class="page-title">Hệ Thống Kiểm Soát Vé</h2>
      <p class="page-subtitle">Nhân viên vui lòng nhập mã định danh để xác thực lượt vào cổng</p>
    </div>

    <div class="check-main-layout">
      <!-- Left: Check Input & Result -->
      <div class="check-left-side">
        <!-- Input Section -->
        <div class="input-card glass-panel-heavy">
          <div class="input-wrapper">
            <i class="fas fa-search search-icon"></i>
            <input 
              type="text" 
              v-model="ticketCode" 
              placeholder="Nhập mã vé hoặc mã vào cổng..." 
              class="ticket-input"
              @keyup.enter="handleCheck"
              ref="codeInput"
            />
            <button @click="handleCheck" class="btn btn-primary check-btn" :disabled="loading">
              <span v-if="loading" class="loader"></span>
              <span v-else>XÁC THỰC</span>
            </button>
          </div>
        </div>

        <!-- Result Section (Success/Details) -->
        <transition name="slide-up">
          <div v-if="result" class="result-ticket-wrapper">
            <div class="ticket-glass" :class="{ 'is-used': result.trangThai === 'Đã sử dụng' }">
              <div class="ticket-top">
                <div class="cinema-brand">ALPHA CINEMA</div>
                <div class="ticket-type">{{ result.loaiGhe === 'HÓA ĐƠN' ? 'HÓA ĐƠN' : (result.loaiGhe + ' TICKET') }}</div>
              </div>
              
              <div class="ticket-body">
                <div class="movie-section">
                  <h3 class="movie-title">{{ result.tenPhim }}</h3>
                  <div class="status-indicator" :class="statusClass">
                    <span class="dot"></span> {{ result.trangThai }}
                  </div>
                </div>

                <div class="details-grid">
                  <div class="detail-box">
                    <span class="label">NGÀY CHIẾU</span>
                    <span class="value">{{ formatDate(result.thoiGianChieu).split(' ')[1] }}</span>
                  </div>
                  <div class="detail-box">
                    <span class="label">GIỜ CHIẾU</span>
                    <span class="valueHighlight">{{ formatDate(result.thoiGianChieu).split(' ')[0] }}</span>
                  </div>
                  <div class="detail-box">
                    <span class="label">PHÒNG</span>
                    <span class="value">{{ result.tenPhong }}</span>
                  </div>
                  <div class="detail-box">
                    <span class="label">GHẾ</span>
                    <span class="valueHighlight seat">{{ result.viTriGhe }}</span>
                  </div>
                </div>

                <div class="ticket-footer-info">
                  <div class="access-info">
                    <span class="label">MÃ VÀO CỔNG</span>
                    <span class="code">{{ result.maVaoCong }}</span>
                  </div>
                  <div class="ticket-id-info">
                    <span class="label">ID VÉ</span>
                    <span class="id">#{{ result.maVe }}</span>
                  </div>
                </div>
              </div>

              <!-- Message Overlay for usage -->
              <div v-if="message" class="ticket-message" :class="{ 'is-success': success }">
                <i class="fas" :class="success ? 'fa-check-circle' : 'fa-exclamation-triangle'"></i>
                {{ message }}
              </div>

              <div class="ticket-stub">
                <div class="barcode-visual">
                  <div v-for="n in 30" :key="n" class="bar" :style="{ width: Math.random() * 4 + 1 + 'px', margin: '0 ' + Math.random() * 2 + 'px' }"></div>
                </div>
              </div>
            </div>

            <div class="ticket-actions">
              <button @click="printTicket" class="btn btn-glow">
                <i class="fas fa-print"></i> IN PHIẾU VÀO CỔNG
              </button>
              <button @click="clearSearch" class="btn btn-outline" style="border-color: rgba(255,255,255,0.2)">
                KIỂM TRA TIẾP
              </button>
            </div>
          </div>
        </transition>
        
        <!-- Error State -->
        <transition name="fade">
          <div v-if="errorMsg" class="error-container glass-panel">
            <div class="error-glow"></div>
            <div class="error-icon">⚠️</div>
            <h4>XÁC THỰC THẤT BẠI</h4>
            <p>{{ errorMsg }}</p>
            <button @click="clearSearch" class="btn btn-primary">THỬ LẠI</button>
          </div>
        </transition>
      </div>

      <!-- Right: Recent History -->
      <div class="history-sidebar glass-panel-heavy">
        <div class="sidebar-header">
          <i class="fas fa-history"></i> LỊCH SỬ KIỂM VÉ
        </div>
        <div class="history-list">
          <div v-if="checkHistory.length === 0" class="empty-history">
            Chưa có lượt kiểm vé nào
          </div>
          <div v-for="(h, i) in checkHistory" :key="i" class="history-item animate-slide-in">
            <div class="hi-time">{{ formatTime(h.checkedAt) }}</div>
            <div class="hi-info">
              <div class="hi-movie">{{ h.tenPhim }}</div>
              <div class="hi-seats">Ghế: {{ h.viTriGhe }}</div>
            </div>
            <div class="hi-status">
              <span class="badge" :class="h.type === 'HÓA ĐƠN' ? 'bg-bill' : 'bg-ticket'">
                {{ h.type }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import api from '../../api/axios';

const ticketCode = ref('');
const loading = ref(false);
const result = ref(null);
const message = ref('');
const success = ref(false);
const errorMsg = ref('');
const codeInput = ref(null);

const statusClass = computed(() => {
  if (!result.value) return '';
  switch (result.value.trangThai) {
    case 'Đã thanh toán': return 'status-valid';
    case 'Đã sử dụng': return 'status-invalid';
    case 'Đã hủy': return 'status-cancelled';
    default: return '';
  }
});

const formatDate = (dateStr) => {
  const date = new Date(dateStr);
  const time = date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit', hour12: false });
  const day = date.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' });
  return `${time} ${day}`;
};

const formatTime = (date) => {
  return date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
};

const checkHistory = ref([]);
const addToHistory = (ticket) => {
  checkHistory.value.unshift({
    tenPhim: ticket.tenPhim,
    viTriGhe: ticket.viTriGhe,
    type: ticket.loaiGhe === 'HÓA ĐƠN' ? 'HÓA ĐƠN' : 'VÉ LẺ',
    checkedAt: new Date()
  });
  if (checkHistory.value.length > 10) checkHistory.value.pop();
};

const handleCheck = async () => {
  if (!ticketCode.value) return;
  
  loading.value = true;
  result.value = null;
  message.value = '';
  errorMsg.value = '';
  success.value = false;

  try {
    const res = await api.post('/dat-ve/check-in', { code: ticketCode.value });
    if (res.success) {
      result.value = res.data;
      message.value = res.message;
      success.value = true;
      addToHistory(res.data);
    }
  } catch (err) {
    errorMsg.value = err.message || 'Mã vé không hợp lệ hoặc lỗi kết nối.';
  } finally {
    loading.value = false;
  }
};

const clearSearch = () => {
  errorMsg.value = '';
  result.value = null;
  message.value = '';
  ticketCode.value = '';
  setTimeout(() => codeInput.value?.focus(), 100);
};

const printTicket = () => {
  window.print();
};

onMounted(() => {
  codeInput.value?.focus();
});
</script>

<style scoped>
.ticket-check-page {
  padding: 2rem;
  min-height: 100%;
  background: radial-gradient(circle at top right, rgba(232, 136, 42, 0.05), transparent 400px),
              radial-gradient(circle at bottom left, rgba(255, 51, 102, 0.05), transparent 400px);
}

.page-header {
  text-align: center;
  margin-bottom: 3rem;
}

.header-icon {
  font-size: 3rem;
  margin-bottom: 1rem;
  filter: drop-shadow(0 0 10px var(--color-primary));
}

.page-title {
  font-size: 2.2rem;
  font-weight: 800;
  margin-bottom: 0.5rem;
  letter-spacing: -1px;
}

.page-subtitle {
  color: var(--color-text-muted);
  font-size: 1.1rem;
}

.check-main-layout {
  display: grid;
  grid-template-columns: 1fr 320px;
  gap: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.check-left-side {
  max-width: 650px;
  justify-self: end;
  width: 100%;
}

/* Glass Panels */
.glass-panel-heavy {
  background: rgba(255, 255, 255, 0.03);
  backdrop-filter: blur(20px);
  -webkit-backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  box-shadow: 0 15px 35px rgba(0, 0, 0, 0.2);
}

/* Input Styling */
.input-card {
  padding: 1.25rem;
  border-radius: 24px;
  margin-bottom: 3rem;
  transition: transform 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}

.input-card:focus-within {
  transform: translateY(-5px);
  border-color: var(--color-primary);
}

.input-wrapper {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.search-icon {
  color: var(--color-text-muted);
  font-size: 1.2rem;
  margin-left: 0.5rem;
}

.ticket-input {
  flex: 1;
  background: transparent;
  border: none;
  font-size: 1.3rem;
  color: white;
  padding: 0.5rem;
  outline: none;
  font-family: 'Space Mono', monospace;
  letter-spacing: 2px;
}

.check-btn {
  padding: 1rem 2.5rem;
  border-radius: 16px;
  font-weight: 800;
  letter-spacing: 1px;
  box-shadow: 0 8px 20px rgba(232, 136, 42, 0.3);
}

/* Ticket Design */
.ticket-glass {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(30px);
  -webkit-backdrop-filter: blur(30px);
  border: 1px solid rgba(255, 255, 255, 0.15);
  border-radius: 30px;
  position: relative;
  overflow: hidden;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5);
}

.ticket-glass.is-used {
  filter: grayscale(0.5);
  opacity: 0.9;
}

.ticket-glass::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0; height: 6px;
  background: linear-gradient(90deg, var(--color-primary), var(--color-secondary));
}

.ticket-top {
  padding: 1.5rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px dashed rgba(255, 255, 255, 0.2);
}

.cinema-brand {
  font-weight: 900;
  letter-spacing: 3px;
  font-size: 0.9rem;
  opacity: 0.7;
}

.ticket-type {
  font-size: 0.75rem;
  background: rgba(255,255,255,0.1);
  padding: 0.3rem 0.8rem;
  border-radius: 10px;
  font-weight: 700;
}

.ticket-body {
  padding: 2.5rem;
}

.movie-section {
  text-align: center;
  margin-bottom: 2.5rem;
}

.movie-title {
  font-size: 2.2rem;
  font-weight: 900;
  line-height: 1.2;
  margin-bottom: 1rem;
  background: linear-gradient(to bottom, #fff, #bbb);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.status-indicator {
  display: inline-flex;
  align-items: center;
  gap: 0.6rem;
  padding: 0.5rem 1.2rem;
  border-radius: 50px;
  font-weight: 700;
  font-size: 0.85rem;
  text-transform: uppercase;
}

.dot {
  width: 8px; height: 8px;
  border-radius: 50%;
  display: block;
}

.status-valid { background: rgba(0, 230, 118, 0.1); color: #00e676; }
.status-valid .dot { background: #00e676; box-shadow: 0 0 10px #00e676; }

.status-invalid { background: rgba(255, 152, 0, 0.1); color: #ff9800; }
.status-invalid .dot { background: #ff9800; box-shadow: 0 0 10px #ff9800; }

.status-cancelled { background: rgba(244, 67, 54, 0.1); color: #f44336; }
.status-cancelled .dot { background: #f44336; }

.details-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 2rem;
  margin-bottom: 3rem;
}

.detail-box {
  display: flex;
  flex-direction: column;
}

.detail-box .label {
  font-size: 0.7rem;
  color: var(--color-text-muted);
  letter-spacing: 2px;
  margin-bottom: 0.4rem;
}

.detail-box .value {
  font-size: 1.1rem;
  font-weight: 600;
}

.valueHighlight {
  font-size: 1.5rem;
  font-weight: 900;
  color: var(--color-primary);
}

.seat {
  color: #4fc3f7;
  text-shadow: 0 0 10px rgba(79, 195, 247, 0.3);
}

.ticket-footer-info {
  display: flex;
  justify-content: space-between;
  border-top: 1px solid rgba(255,255,255,0.1);
  padding-top: 1.5rem;
}

.access-info, .ticket-id-info {
  display: flex;
  flex-direction: column;
}

.access-info .code {
  font-family: 'Space Mono', monospace;
  font-weight: 700;
  color: var(--color-primary);
}

.ticket-id {
  font-size: 0.9rem;
  opacity: 0.5;
}

/* Ticket Message Overlay */
.ticket-message {
  padding: 1.2rem;
  text-align: center;
  background: rgba(255, 152, 0, 0.2);
  color: #ffb74d;
  font-weight: 700;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.8rem;
  animation: pulse 2s infinite;
}

.ticket-message.is-success {
  background: rgba(0, 230, 118, 0.2);
  color: #69f0ae;
}

@keyframes pulse {
  0% { backdrop-filter: blur(30px); }
  50% { backdrop-filter: blur(40px); background: rgba(0, 230, 118, 0.25); }
  100% { backdrop-filter: blur(30px); }
}

/* Stub / Barcode */
.ticket-stub {
  background: rgba(0, 0, 0, 0.15);
  padding: 1.5rem;
  display: flex;
  justify-content: center;
}

.barcode-visual {
  display: flex;
  height: 40px;
  align-items: center;
  opacity: 0.4;
}

.bar {
  background: white;
  height: 100%;
}

/* Actions */
.ticket-actions {
  margin-top: 2rem;
  display: flex;
  gap: 1rem;
}

.ticket-actions button {
  flex: 1;
  padding: 1.2rem;
  border-radius: 18px;
  font-weight: 700;
}

.btn-glow {
  background: var(--color-primary);
  color: white;
  border: none;
  box-shadow: 0 10px 30px rgba(232, 136, 42, 0.4);
}

/* Error Container */
.error-container {
  padding: 4rem 2rem;
  text-align: center;
  border-radius: 30px;
  position: relative;
  overflow: hidden;
  border: 1px solid rgba(255, 51, 102, 0.3);
}

.error-glow {
  position: absolute;
  top: 50%; left: 50%;
  transform: translate(-50%, -50%);
  width: 200px; height: 200px;
  background: var(--color-secondary);
  filter: blur(100px);
  opacity: 0.2;
  z-index: -1;
}

.error-icon {
  font-size: 4rem;
  margin-bottom: 1.5rem;
  animation: shake 0.5s infinite;
}

.error-container h4 {
  font-size: 1.5rem;
  font-weight: 900;
  margin-bottom: 1rem;
  color: var(--color-secondary);
}

.error-container p {
  color: var(--color-text-muted);
  margin-bottom: 2.5rem;
}

/* Loader */
.loader {
  width: 20px; height: 20px;
  border: 3px solid rgba(255,255,255,0.3);
  border-top-color: white;
  border-radius: 50%;
  display: inline-block;
  animation: spin 0.8s linear infinite;
}

@keyframes spin { to { transform: rotate(360deg); } }

@keyframes shake {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.1); }
}

/* History Sidebar */
.history-sidebar {
  padding: 1.5rem;
  border-radius: 20px;
  height: fit-content;
  max-height: 80vh;
  display: flex;
  flex-direction: column;
}

.sidebar-header {
  font-weight: 800;
  font-size: 0.9rem;
  color: var(--color-primary);
  margin-bottom: 1.5rem;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid rgba(255,255,255,0.05);
}

.history-list {
  overflow-y: auto;
  flex: 1;
}

.empty-history {
  text-align: center;
  color: var(--color-text-muted);
  padding: 2rem 0;
  font-size: 0.85rem;
  font-style: italic;
}

.history-item {
  background: rgba(255,255,255,0.02);
  border: 1px solid rgba(255,255,255,0.05);
  border-radius: 12px;
  padding: 1rem;
  margin-bottom: 0.75rem;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.hi-time {
  font-size: 0.7rem;
  color: var(--color-text-muted);
  font-weight: 700;
}

.hi-movie {
  font-weight: 700;
  font-size: 0.9rem;
  color: white;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.hi-seats {
  font-size: 0.75rem;
  color: #4fc3f7;
}

.hi-status {
  margin-top: 0.2rem;
}

.badge {
  font-size: 0.65rem;
  padding: 0.2rem 0.5rem;
  border-radius: 4px;
  font-weight: 900;
}

.bg-bill { background: #e8882a; color: white; }
.bg-ticket { background: rgba(255,255,255,0.1); color: #ccc; }

.animate-slide-in {
  animation: slideInRight 0.4s ease-out;
}

@keyframes slideInRight {
  from { opacity: 0; transform: translateX(20px); }
  to { opacity: 1; transform: translateX(0); }
}

@media (max-width: 992px) {
  .check-main-layout {
    grid-template-columns: 1fr;
  }
  .check-left-side {
    max-width: 100%;
    justify-self: center;
  }
}
</style>
