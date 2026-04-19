<template>
  <div class="promotions-page container mt-5">
    <div class="text-center mb-5 animate-slide-down">
      <h1 class="gradient-text section-title">Khuyến Mãi & Ưu Đãi</h1>
      <p class="text-sub">Trải nghiệm điện ảnh đỉnh cao với những ưu đãi đặc quyền chỉ có tại Alpha Cinema.</p>
    </div>

    <!-- Category Tabs -->
    <div class="tabs-container glass-panel mb-5">
      <button 
        v-for="tab in tabs" :key="tab.id"
        class="tab-btn" :class="{ active: activeTab === tab.id }"
        @click="activeTab = tab.id"
      >
        <i :class="tab.icon"></i> {{ tab.name }}
      </button>
    </div>

    <!-- Promotion Grid -->
    <div class="promotion-grid" v-if="filteredPromos.length > 0">
      <div 
        v-for="promo in filteredPromos" :key="promo.id" 
        class="promo-card glass-panel animate-fade-in"
      >
        <div class="promo-image">
          <img 
            :src="promo.image" 
            :alt="promo.title"
            @error="e => e.target.src = 'https://images.unsplash.com/photo-1478720568477-152d9b164e26?auto=format&fit=crop&q=80&w=800'"
          >
          <div class="promo-tag" :class="promo.type">{{ promo.tag }}</div>
          <div class="promo-overlay">
            <span class="view-details" @click="openDetails(promo)">Xem Chi Tiết</span>
          </div>
        </div>
        <div class="promo-content">
          <div class="promo-date">{{ promo.startDate }} - {{ promo.endDate }}</div>
          <h3 class="promo-title">{{ promo.title }}</h3>
          <p class="promo-desc">{{ promo.description }}</p>
          <div class="promo-footer">
            <div class="brand-logo">
              ALPHA<span class="text-primary">CINEMA</span>
            </div>
            <button class="btn btn-primary btn-sm" @click="openDetails(promo)">Xem Thêm</button>
          </div>
        </div>
      </div>
    </div>

    <div v-else class="empty-state glass-panel">
      <i class="fas fa-ticket-alt mb-3" style="font-size: 3rem; opacity: 0.2;"></i>
      <p>Hiện không có chương trình ưu đãi nào trong mục này.</p>
    </div>

    <!-- Detail Modal -->
    <transition name="modal">
      <div v-if="selectedPromo" class="modal-overlay" @click.self="closeDetails">
        <div class="promo-modal glass-panel animate-scale-up">
          <button class="close-modal" @click="closeDetails"><i class="fas fa-times"></i></button>
          
          <div class="modal-body">
            <div class="modal-img">
              <img :src="selectedPromo.image" :alt="selectedPromo.title">
              <div class="modal-tag" :class="selectedPromo.type">{{ selectedPromo.tag }}</div>
            </div>
            
            <div class="modal-info">
              <h2 class="modal-title gradient-text">{{ selectedPromo.title }}</h2>
              <div class="modal-period">
                <i class="far fa-calendar-alt"></i> {{ selectedPromo.startDate }} - {{ selectedPromo.endDate }}
              </div>
              
              <div class="modal-section">
                <h4><i class="fas fa-info-circle"></i> Nội dung ưu đãi</h4>
                <div class="instructions" v-html="formatNewLines(selectedPromo.fullDescription)"></div>
              </div>
              
              <div class="modal-voucher" v-if="selectedPromo.voucher">
                <div class="voucher-label">MÃ ƯU ĐÃI:</div>
                <div class="voucher-code-wrap">
                  <span class="voucher-code">{{ selectedPromo.voucher }}</span>
                  <div class="free-badge">COUPON</div>
                </div>
                <p class="voucher-note">*Sử dụng mã này tại bước thanh toán để nhận ưu đãi.</p>
              </div>
              
              <div class="modal-actions">
                <button class="btn btn-primary w-full" @click="closeDetails">Đã Hiểu, Cảm Ơn!</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </transition>

    <!-- Alpha Membership Banner -->
    <div class="alpha-membership mt-5 glass-panel p-5">
      <div class="banner-grid">
        <div class="banner-text">
          <h2 class="gradient-text mb-3">Đặc Quyền Thành Viên Alpha</h2>
          <p class="text-sub mb-4">Trở thành một phần của cộng đồng Alpha Cinema để nhận những ưu đãi độc quyền, quà tặng sinh nhật và tích điểm không giới hạn.</p>
          <div class="benefits-list">
            <div class="benefit"><i class="fas fa-check-circle text-primary"></i> Tích điểm 5% mỗi giao dịch</div>
            <div class="benefit"><i class="fas fa-check-circle text-primary"></i> Quà tặng sinh nhật đặc biệt</div>
            <div class="benefit"><i class="fas fa-check-circle text-primary"></i> Ưu tiên đặt vé suất chiếu sớm</div>
          </div>
          <router-link to="/register" class="btn btn-primary mt-4 pulsate">Đăng Ký Thành Viên</router-link>
        </div>
        <div class="banner-visual">
          <div class="membership-card-visual">
            <div class="card alpha-card gold">
              <div class="card-logo">ALPHA</div>
              <div class="card-chip"></div>
              <div class="card-rank">GOLD MEMBER</div>
            </div>
            <div class="card alpha-card diamond">
              <div class="card-logo">ALPHA</div>
              <div class="card-chip blue-chip"></div>
              <div class="card-rank">DIAMOND MEMBER</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { promotionApi } from '../../api/promotionApi';

const activeTab = ref('all');
const promotions = ref([]);
const loading = ref(true);

const tabs = [
  { id: 'all', name: 'Tất cả', icon: 'fas fa-th-large' },
  { id: 'member', name: 'Thành Viên', icon: 'fas fa-user-shield' },
  { id: 'ticket', name: 'Giá Vé', icon: 'fas fa-tags' },
  { id: 'payment', name: 'Thanh Toán', icon: 'fas fa-credit-card' }
];

const getImageUrl = (name) => {
  if (!name) return 'https://images.unsplash.com/photo-1478720568477-152d9b164e26?auto=format&fit=crop&q=80&w=800';
  if (name.startsWith('http')) return name;
  
  // Dùng logic Vite-standard để map dynamic asset
  return new URL(`../../assets/promotions/${name}`, import.meta.url).href;
};

const loadPromotions = async () => {
  try {
    const res = await promotionApi.getPromotions();
    if (res.success) {
      promotions.value = res.data
        .filter(p => p.conHieuLuc) // Chỉ hiện mã đang hiệu lực
        .map(p => ({
          id: p.maKhuyenMai,
          // ... rest of mapping
          title: p.tenKhuyenMai,
          description: p.moTa?.substring(0, 100) + (p.moTa?.length > 100 ? '...' : ''),
          fullDescription: p.moTa,
          image: getImageUrl(p.hinhAnh),
          startDate: formatDate(p.ngayBatDau),
          endDate: formatDate(p.ngayKetThuc),
          tag: p.phanLoai || 'ƯU ĐÃI',
          type: mapType(p.phanLoai),
          voucher: p.maCodeGiamGia
        }));
    }
  } catch (error) {
    console.error('Lỗi tải khuyến mãi:', error);
  } finally {
    loading.value = false;
  }
};

const mapType = (phanLoai) => {
  if (!phanLoai) return 'member';
  const val = phanLoai.toLowerCase();
  if (val.includes('thành viên') || val.includes('member')) return 'member';
  if (val.includes('giá vé') || val.includes('ticket') || val.includes('đồng giá')) return 'ticket';
  if (val.includes('thanh toán') || val.includes('payment') || val.includes('vnpay')) return 'payment';
  return 'member'; // default
};

const selectedPromo = ref(null);

const openDetails = (promo) => {
  selectedPromo.value = promo;
  document.body.style.overflow = 'hidden';
};

const closeDetails = () => {
  selectedPromo.value = null;
  document.body.style.overflow = 'auto';
};

const formatNewLines = (text) => {
  if (!text) return '';
  return text.split('\n').map(line => `<p>${line}</p>`).join('');
};

const formatDate = (date) => new Date(date).toLocaleDateString('vi-VN');

const filteredPromos = computed(() => {
  if (activeTab.value === 'all') return promotions.value;
  return promotions.value.filter(p => p.type === activeTab.value);
});

onMounted(() => {
  loadPromotions();
});
</script>

<style scoped>
.promotions-page { padding-bottom: 5rem; }

.tabs-container {
  display: flex;
  justify-content: center;
  gap: 1rem;
  padding: 0.75rem;
  margin-bottom: 3.5rem;
}

.tab-btn {
  background: transparent;
  border: none;
  color: var(--color-text-sub);
  padding: 0.8rem 2rem;
  border-radius: var(--radius-md);
  font-weight: 700;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  display: flex;
  align-items: center;
  gap: 0.75rem;
  text-transform: uppercase;
  font-size: 0.85rem;
  letter-spacing: 0.5px;
}

.tab-btn:hover { color: #fff; background: rgba(255,255,255,0.05); }
.tab-btn.active {
  background: linear-gradient(135deg, var(--color-primary), var(--color-secondary));
  color: #fff;
  box-shadow: 0 8px 20px rgba(232, 136, 42, 0.3);
}

.promotion-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(360px, 1fr));
  gap: 2.5rem;
}

.promo-card {
  overflow: hidden;
  transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  display: flex;
  flex-direction: column;
  border: 1px solid rgba(255, 255, 255, 0.05);
}

.promo-card:hover {
  transform: translateY(-12px);
  border-color: var(--color-primary);
  box-shadow: 0 15px 40px rgba(0,0,0,0.6);
}

.promo-image {
  position: relative;
  height: 220px;
  overflow: hidden;
}

.promo-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.6s ease;
}

.promo-card:hover .promo-image img { transform: scale(1.08); }

.promo-overlay {
  position: absolute;
  top: 0; left: 0; width: 100%; height: 100%;
  background: rgba(0,0,0,0.4);
  display: flex;
  align-items: center; justify-content: center;
  opacity: 0;
  transition: 0.3s;
}

.promo-card:hover .promo-overlay { opacity: 1; }

.view-details {
  border: 1px solid #fff;
  padding: 8px 20px;
  border-radius: 40px;
  font-weight: 700;
  font-size: 0.8rem;
  text-transform: uppercase;
}

.promo-tag {
  position: absolute;
  top: 20px;
  right: 20px;
  padding: 6px 16px;
  border-radius: 6px;
  font-size: 0.7rem;
  font-weight: 900;
  text-transform: uppercase;
  color: #fff;
  z-index: 1;
  box-shadow: 0 4px 10px rgba(0,0,0,0.3);
}

.promo-tag.member { background: linear-gradient(90deg, #f1c40f, #d35400); }
.promo-tag.ticket { background: linear-gradient(90deg, #e67e22, #c0392b); }
.promo-tag.payment { background: linear-gradient(90deg, #3498db, #2980b9); }

.promo-content {
  padding: 2rem;
  flex: 1;
  display: flex;
  flex-direction: column;
}

.promo-date {
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 1px;
  color: var(--color-primary);
  margin-bottom: 0.75rem;
  font-weight: 700;
}

.promo-title {
  font-size: 1.25rem;
  margin-bottom: 1rem;
  color: #fff;
  line-height: 1.4;
  font-weight: 900;
}

.promo-desc {
  font-size: 0.95rem;
  color: var(--color-text-sub);
  margin-bottom: 2rem;
  flex: 1;
  line-height: 1.6;
}

.promo-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.brand-logo {
  font-size: 0.8rem;
  font-weight: 900;
  letter-spacing: 1px;
  opacity: 0.6;
}

/* Membership Banner */
.alpha-membership {
  background: linear-gradient(145deg, rgba(30,30,30,0.9), rgba(15,15,15,0.95));
  overflow: hidden;
  position: relative;
  border: 1px solid rgba(255,255,255,0.05);
  box-shadow: 0 10px 40px rgba(0,0,0,0.5);
}

.alpha-membership::before {
  content: '';
  position: absolute;
  top: -100px; right: -100px;
  width: 300px; height: 300px;
  background: radial-gradient(circle, var(--color-primary-glow), transparent 70%);
  z-index: 0;
  pointer-events: none;
}

.banner-grid {
  display: grid;
  grid-template-columns: 1.1fr 0.9fr;
  align-items: center;
  gap: 4rem;
  position: relative;
  z-index: 1;
}

.banner-text { padding: 1rem 0; padding-left: 5rem; }

.benefits-list {
  display: grid;
  grid-template-columns: 1fr;
  gap: 1.25rem;
  margin-top: 2rem;
}

.benefit {
  display: flex;
  align-items: center;
  gap: 14px;
  font-weight: 500;
  font-size: 1.05rem;
  color: var(--color-text-main);
}

.benefit i {
  font-size: 1.2rem;
  filter: drop-shadow(0 0 8px var(--color-primary-glow));
}

.banner-visual { 
  position: relative; 
  height: 350px; 
  display: flex; 
  align-items: center; 
  justify-content: center; 
}

.membership-card-visual { 
  position: relative; 
  width: 100%; 
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.alpha-card {
  position: absolute;
  width: 280px;
  height: 175px;
  border-radius: 18px;
  padding: 1.8rem;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  box-shadow: 0 20px 50px rgba(0,0,0,0.8);
  transition: all 0.6s cubic-bezier(0.19, 1, 0.22, 1);
  backdrop-filter: blur(5px);
  border: 1px solid rgba(255,255,255,0.1);
  overflow: hidden;
}

/* Common Card Elements */
.alpha-card::after {
  content: '';
  position: absolute;
  top: -50%; left: -50%;
  width: 200%; height: 200%;
  background: linear-gradient(45deg, transparent, rgba(255,255,255,0.1), transparent);
  transform: rotate(25deg);
  transition: 0.8s;
}

.alpha-card:hover::after { left: 100%; }

.card-logo { 
  font-weight: 900; 
  letter-spacing: 3px; 
  font-size: 1rem; 
  opacity: 0.9;
}

.card-chip {
  width: 40px;
  height: 30px;
  background: linear-gradient(135deg, #e1b12c, #fbc531);
  border-radius: 6px;
  position: relative;
  opacity: 0.8;
}

.card-rank { 
  font-weight: 800; 
  font-size: 0.8rem; 
  letter-spacing: 4px; 
  text-transform: uppercase;
  border-top: 1px solid rgba(255,255,255,0.15); 
  padding-top: 12px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

/* Specific Card Styles */
.gold {
  background: linear-gradient(135deg, #bf953f, #fcf6ba, #b38728, #fbf5b7, #aa771c);
  color: #5c4100;
  top: 15%; left: 0%;
  transform: rotate(-8deg) translateX(20px);
  z-index: 1;
  animation: floatGold 6s ease-in-out infinite;
}

.diamond {
  background: linear-gradient(135deg, #130f40 0%, #30336b 50%, #130f40 100%);
  color: #fff;
  bottom: 15%; right: 0%;
  transform: rotate(5deg) translateX(-20px);
  z-index: 2;
  border-color: rgba(255,255,255,0.2);
  animation: floatDiamond 6s ease-in-out infinite;
}

.blue-chip { background: linear-gradient(135deg, #74b9ff, #0984e3); }

/* Animations */
@keyframes floatGold {
  0%, 100% { transform: rotate(-8deg) translate(20px, 0px); }
  50% { transform: rotate(-6deg) translate(25px, -15px); }
}

@keyframes floatDiamond {
  0%, 100% { transform: rotate(5deg) translate(-20px, 0px); }
  50% { transform: rotate(7deg) translate(-25px, 15px); }
}

.alpha-card:hover { 
  transform: scale(1.1) rotate(0) translate(0, -10px) !important; 
  z-index: 10; 
  box-shadow: 0 30px 60px rgba(0,0,0,0.9);
  animation-play-state: paused;
}

.pulsate { 
  animation: pulse 2s infinite; 
  padding: 1rem 2.5rem;
  border-radius: 50px;
  font-size: 1.1rem;
}

@keyframes pulse {
  0% { box-shadow: 0 0 0 0 rgba(232, 136, 42, 0.7); }
  70% { box-shadow: 0 0 0 20px rgba(232, 136, 42, 0); }
  100% { box-shadow: 0 0 0 0 rgba(232, 136, 42, 0); }
}

/* ═══ DETAIL MODAL ═══ */
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.9);
  backdrop-filter: blur(10px);
  z-index: 2000;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem;
}

.promo-modal {
  width: 100%;
  max-width: 900px;
  max-height: 90vh;
  position: relative;
  overflow-y: auto;
  border: 1px solid rgba(255, 255, 255, 0.1);
  box-shadow: 0 40px 100px rgba(0, 0, 0, 0.7);
  padding: 0;
}

.close-modal {
  position: absolute;
  top: 1.5rem; right: 1.5rem;
  width: 40px; height: 40px;
  border-radius: 50%;
  background: rgba(0,0,0,0.5);
  border: 1px solid rgba(255,255,255,0.1);
  color: #fff;
  cursor: pointer;
  z-index: 10;
  transition: 0.3s;
}
.close-modal:hover { background: var(--color-primary); transform: rotate(90deg); }

.modal-body {
  display: grid;
  grid-template-columns: 380px 1fr;
  gap: 0;
}

.modal-img {
  position: relative;
  height: 100%;
  min-height: 400px;
}
.modal-img img { width: 100%; height: 100%; object-fit: cover; }
.modal-tag {
  position: absolute;
  top: 1.5rem; left: 1.5rem;
  padding: 6px 15px;
  border-radius: 6px;
  font-weight: 800;
  font-size: 0.8rem;
  text-transform: uppercase;
  /* Class-based colors already defined or generic */
  background: var(--color-primary);
  color: #fff;
}

.modal-info { padding: 3rem; }
.modal-title { font-size: 1.8rem; font-weight: 900; margin-bottom: 0.5rem; line-height: 1.3; }
.modal-period { font-size: 0.9rem; color: #888; margin-bottom: 2.5rem; display: flex; align-items: center; gap: 8px; }

.modal-section { margin-bottom: 2rem; }
.modal-section h4 { 
  font-size: 1rem; 
  text-transform: uppercase; 
  color: var(--color-primary); 
  margin-bottom: 1rem; 
  display: flex; 
  align-items: center; 
  gap: 10px;
  letter-spacing: 1px;
}
.modal-section p { color: var(--color-text-sub); line-height: 1.7; font-size: 1rem; }
.instructions :deep(p) { margin-bottom: 0.75rem; color: var(--color-text-sub); }

.modal-voucher {
  background: rgba(232, 136, 42, 0.1);
  border: 1px dashed var(--color-primary);
  border-radius: 12px;
  padding: 1.5rem;
  margin-bottom: 2rem;
  position: relative;
}
.voucher-label { font-size: 0.75rem; font-weight: 800; color: #aaa; margin-bottom: 0.5rem; letter-spacing: 1.5px; }
.voucher-code-wrap { display: flex; align-items: center; gap: 12px; }
.voucher-code { font-size: 1.6rem; font-family: 'Courier New', Courier, monospace; font-weight: 900; color: #fff; letter-spacing: 2px; }
.free-badge {
  background: var(--color-red);
  color: #fff;
  padding: 2px 10px;
  border-radius: 4px;
  font-size: 0.7rem;
  font-weight: 900;
}
.voucher-note { font-size: 0.8rem; color: #666; margin-top: 0.75rem; margin-bottom: 0; }

.w-full { width: 100%; }

/* Modal Transitions */
.modal-enter-active, .modal-leave-active { transition: opacity 0.4s ease; }
.modal-enter-from, .modal-leave-to { opacity: 0; }

@keyframes scaleUp {
  from { transform: scale(0.9); opacity: 0; }
  to { transform: scale(1); opacity: 1; }
}
.animate-scale-up { animation: scaleUp 0.4s cubic-bezier(0.19, 1, 0.22, 1); }

@media (max-width: 900px) {
  .modal-body { grid-template-columns: 1fr; }
  .modal-img { min-height: 250px; max-height: 300px; }
  .modal-info { padding: 2rem; }
  .modal-title { font-size: 1.5rem; }
}

@media (max-width: 992px) {
  .banner-grid { grid-template-columns: 1fr; text-align: center; gap: 2rem; }
  .benefit { justify-content: center; }
  .banner-visual { height: 300px; margin-top: 1rem; }
  .alpha-card { width: 240px; height: 150px; }
  .gold { left: 10%; }
  .diamond { right: 10%; }
}
</style>
