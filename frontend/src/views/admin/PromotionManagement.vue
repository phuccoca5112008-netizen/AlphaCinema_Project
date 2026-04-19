<template>
  <div class="promotion-management">
    <div class="header-section mb-5">
      <div class="header-info">
        <h1 class="gradient-text title-large">Quản Lý Bài Đăng & Ưu Đãi</h1>
        <p class="text-muted">Đăng tải và điều chỉnh nội dung hiển thị tại khu vực "Khuyến Mãi & Ưu Đãi"</p>
      </div>
      <button class="btn-create-premium" @click="openForm()">
        <span class="icon-plus">✨</span>
        <span>ĐĂNG BÀI ƯU ĐÃI MỚI</span>
      </button>
    </div>

    <!-- Stats Summary -->
    <div class="promo-stats mb-5">
      <div class="mini-stat glass-panel">
        <span class="label">Tổng số mã</span>
        <span class="value">{{ promotions.length }}</span>
      </div>
      <div class="mini-stat glass-panel">
        <span class="label">Đang chạy</span>
        <span class="value text-success-bright">{{ promotions.filter(p => getStatus(p) === 'active').length }}</span>
      </div>
      <div class="mini-stat glass-panel">
        <span class="label">Chờ công bố</span>
        <span class="value text-warning-bright">{{ promotions.filter(p => getStatus(p) === 'waiting').length }}</span>
      </div>
    </div>

    <div v-if="loading" class="loading-state">
      <div class="loader-circle"></div>
      <p>Đang tải danh sách ưu đãi...</p>
    </div>
    
    <div v-else class="promo-grid">
      <div v-for="km in promotions" :key="km.maKhuyenMai" class="promo-card glass-panel-heavy" :class="getStatus(km)">
        <div class="promo-card-header">
          <div class="promo-category-badge" v-if="km.phanLoai">{{ km.phanLoai }}</div>
          <div class="promo-badge" :class="getStatus(km)">
            {{ getStatusText(km) }}
          </div>
          <div class="promo-actions">
            <button class="action-btn edit" @click="openForm(km)"><i class="fas fa-edit"></i></button>
            <button class="action-btn delete" @click="deletePromotion(km.maKhuyenMai)"><i class="fas fa-trash-alt"></i></button>
          </div>
        </div>

        <div class="promo-poster-mini" v-if="km.hinhAnh">
          <img :src="getImageUrl(km.hinhAnh)" alt="Promo Poster" />
        </div>

        <div class="promo-card-body">
          <div class="promo-main-info">
            <h3 class="promo-title">{{ km.tenKhuyenMai }}</h3>
            <p class="promo-desc">{{ km.moTa || 'Chương trình ưu đãi Alpha Cinema' }}</p>
          </div>

          <div class="promo-coupon-box">
            <div class="coupon-label">MÃ GIẢM GIÁ</div>
            <div class="coupon-code" @click="copyCode(km.maCodeGiamGia)">
              {{ km.maCodeGiamGia }}
              <i class="far fa-copy ms-2"></i>
            </div>
          </div>

          <div class="promo-details">
            <div class="detail-item">
              <span class="detail-label">Mức giảm</span>
              <span class="detail-value highlight">
                {{ km.loaiGiamGia === 'PhanTram' ? km.giaTriGiam + '%' : km.giaTriGiam.toLocaleString() + 'đ' }}
              </span>
            </div>
            <div class="detail-item" v-if="km.giamToiDa">
              <span class="detail-label">Tối đa</span>
              <span class="detail-value">{{ km.giamToiDa.toLocaleString() }}đ</span>
            </div>
          </div>
        </div>

        <div class="promo-card-footer">
          <div class="promo-period">
            <i class="far fa-clock me-2"></i>
            {{ formatDate(km.ngayBatDau) }} — {{ formatDate(km.ngayKetThuc) }}
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-if="promotions.length === 0" class="empty-promo glass-panel">
        <i class="fas fa-ticket-alt large-icon mb-3"></i>
        <h3>Chưa có mã khuyến mãi nào</h3>
        <p>Bắt đầu tạo mã đầu tiên để thu hút khách hàng ngay!</p>
      </div>
    </div>

    <!-- MODAL FORM -->
    <div v-if="showModal" class="modal-overlay">
      <div class="modal-box glass-panel-heavy animate-pop">
        <div class="modal-top">
          <h2>{{ isEdit ? 'Chỉnh Sửa Ưu Đãi' : 'Tạo Ưu Đãi Mới' }}</h2>
          <button @click="showModal = false" class="close-modal">×</button>
        </div>
        
        <form @submit.prevent="savePromotion" class="modern-form">
          <div class="form-grid">
            <div class="field full">
              <label>Tên Chương Trình Ưu Đãi</label>
              <input type="text" v-model="form.tenKhuyenMai" placeholder="VD: Thứ 3 Vui Vẻ - Đồng giá 50k" required />
            </div>

            <div class="field">
              <label>Phân Loại (Badge)</label>
              <input type="text" v-model="form.phanLoai" placeholder="VD: ƯU ĐÃI HOT, STUDENT DEAL..." />
            </div>

            <div class="field">
              <label>Link Hình Ảnh (Poster)</label>
              <input type="text" v-model="form.hinhAnh" placeholder="Hệ thống sẽ lấy ảnh từ link này..." />
            </div>
            
            <div class="field">
              <label>Mã Code (Coupon)</label>
              <input type="text" v-model="form.maCodeGiamGia" placeholder="VD: ALPHA50K" required />
            </div>

            <div class="field">
              <label>Loại Giảm Giá</label>
              <select v-model="form.loaiGiamGia" class="form-select-custom" :disabled="isEdit">
                <option value="PhanTram">Giảm theo %</option>
                <option value="CoDinh">Giảm tiền mặt (đ)</option>
              </select>
            </div>

            <div class="field">
              <label>Giá Trị Giảm</label>
              <input type="number" v-model="form.giaTriGiam" min="1" required :disabled="isEdit" />
            </div>

            <div class="field">
              <label>Giảm Tối Đa (VNĐ)</label>
              <input type="number" v-model="form.giamToiDa" placeholder="Không giới hạn" :disabled="isEdit" />
            </div>

            <div class="field">
              <label>Ngày Bắt Đầu</label>
              <input type="date" v-model="form.ngayBatDau" required />
            </div>

            <div class="field">
              <label>Ngày Kết Thúc</label>
              <input type="date" v-model="form.ngayKetThuc" required />
            </div>

            <div class="field full">
              <label>Nội Dung Chi Tiết Ưu Đãi</label>
              <textarea v-model="form.moTa" rows="4" placeholder="Nhập nội dung hiển thị khi khách hàng nhấn Xem Thêm..."></textarea>
            </div>
          </div>

          <div class="modal-actions">
            <button type="button" class="btn-cancel" @click="showModal = false">HUỶ BỎ</button>
            <button type="submit" class="btn-save-gold">
                {{ isEdit ? 'CẬP NHẬT ƯU ĐÃI' : 'ĐĂNG ƯU ĐÃI NGAY' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { promotionApi } from '../../api/promotionApi';

const promotions = ref([]);
const loading = ref(true);
const showModal = ref(false);
const isEdit = ref(false);
const form = ref({});

const getImageUrl = (name) => {
  if (!name) return 'https://placehold.co/800x400/1a1a1a/E8882A?text=Alpha+Cinema';
  // Nếu là link web thì trả về luôn
  if (name.startsWith('http')) return name;
  // Nếu là file local thì map vào thư mục assets
  return new URL(`../../assets/promotions/${name}`, import.meta.url).href;
};

const loadPromotions = async () => {
  try {
    const res = await promotionApi.getPromotions();
    if (res.success) {
      promotions.value = res.data;
    }
  } catch (error) {
    console.error('Lỗi tải khuyến mãi:', error);
  } finally {
    loading.value = false;
  }
};

const openForm = (km = null) => {
  if (km) {
    isEdit.value = true;
    form.value = {
      ...km,
      ngayBatDau: km.ngayBatDau.split('T')[0],
      ngayKetThuc: km.ngayKetThuc.split('T')[0]
    };
  } else {
    isEdit.value = false;
    form.value = {
      tenKhuyenMai: '',
      maCodeGiamGia: '',
      moTa: '',
      ngayBatDau: new Date().toISOString().split('T')[0],
      ngayKetThuc: '',
      loaiGiamGia: 'PhanTram',
      giaTriGiam: 0,
      giamToiDa: null,
      donHangToiThieu: 0,
      hinhAnh: '',
      phanLoai: ''
    };
  }
  showModal.value = true;
};

const savePromotion = async () => {
  const confirmMsg = isEdit.value 
    ? "Bạn có chắc chắn muốn cập nhật các thông tin cho ưu đãi này?" 
    : "Xác nhận đăng ưu đãi mới? \n\nLưu ý: Sau khi đăng, bạn sẽ KHÔNG thể thay đổi Loại giảm giá, Giá trị giảm và Mức giảm tối đa để đảm bảo tính minh bạch cho khách hàng.";
    
  if (!confirm(confirmMsg)) return;

  try {
    let res;
    if (isEdit.value) {
      res = await promotionApi.updatePromotion(form.value.maKhuyenMai, form.value);
    } else {
      res = await promotionApi.createPromotion(form.value);
    }
    
    if (res.success) {
      showModal.value = false;
      loadPromotions();
    }
  } catch (error) {
    alert(error.message || 'Lỗi lưu thông tin');
  }
};

const deletePromotion = async (id) => {
  if (!confirm('Bạn có chắc chắn muốn xóa mã này?')) return;
  try {
    const res = await promotionApi.deletePromotion(id);
    if (res.success) {
      loadPromotions();
    }
  } catch (error) {
    alert(error.message || 'Lỗi khi xóa');
  }
};

const formatDate = (date) => new Date(date).toLocaleDateString('vi-VN');

const getStatus = (km) => {
  const now = new Date();
  const start = new Date(km.ngayBatDau);
  const end = new Date(km.ngayKetThuc);
  
  if (now < start) return 'waiting';
  if (now > end) return 'expired';
  return 'active';
};

const getStatusText = (km) => {
  const status = getStatus(km);
  if (status === 'waiting') return 'SẮP DIỄN RA';
  if (status === 'expired') return 'HẾT HẠN';
  return 'ĐANG CHẠY';
};

const copyCode = (code) => {
  navigator.clipboard.writeText(code);
  alert('Đã copy mã: ' + code);
};

onMounted(() => {
  loadPromotions();
});
</script>

<style scoped>
.promotion-management { padding: 1rem; }

.header-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.title-large { font-size: 2.2rem; font-weight: 800; margin-bottom: 0.2rem; }
.text-muted { color: #888; font-size: 1rem; }

.btn-create-premium {
  background: linear-gradient(135deg, #e8882a 0%, #ff5722 100%);
  color: white; border: none; padding: 0.8rem 1.8rem;
  border-radius: 16px; font-weight: 800; cursor: pointer;
  display: flex; align-items: center; gap: 0.8rem;
  box-shadow: 0 8px 25px rgba(232, 136, 42, 0.3);
  transition: 0.3s;
}

.btn-create-premium:hover {
  transform: translateY(-3px);
  box-shadow: 0 12px 30px rgba(232, 136, 42, 0.5);
}

.icon-plus { font-size: 1.4rem; }

.promo-stats { display: flex; gap: 1.5rem; }
.mini-stat {
  padding: 1rem 2.5rem; border-radius: 20px;
  display: flex; flex-direction: column; align-items: center;
}
.mini-stat .label { font-size: 0.75rem; color: #888; text-transform: uppercase; letter-spacing: 1px; }
.mini-stat .value { font-size: 1.6rem; font-weight: 900; }
.text-success-bright { color: #00e676; }
.text-warning-bright { color: #e8882a; }

.promo-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(360px, 1fr));
  gap: 2rem;
}

.promo-card {
  border-radius: 28px;
  padding: 2rem;
  transition: 0.4s;
  position: relative;
  overflow: hidden;
  border: 1px solid rgba(255,255,255,0.05);
}

.promo-card:hover {
  transform: translateY(-5px);
  border-color: rgba(232, 136, 42, 0.3);
}

.promo-card.expired { opacity: 0.5; filter: grayscale(0.6); }
.promo-card.waiting { opacity: 0.8; border-color: rgba(232, 136, 42, 0.1); }

.promo-card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.promo-badge {
  font-size: 0.65rem; font-weight: 900; padding: 4px 12px;
  border-radius: 50px; text-transform: uppercase;
}
.promo-badge.active { background: rgba(0,230,118,0.1); color: #00e676; }
.promo-badge.waiting { background: rgba(232, 136, 42, 0.1); color: #e8882a; }
.promo-badge.expired { background: rgba(255,51,102,0.1); color: #ff3366; }

.promo-category-badge {
  position: absolute;
  top: 1.5rem;
  left: 2rem;
  background: var(--color-primary);
  color: white;
  padding: 4px 12px;
  border-radius: 6px;
  font-size: 0.7rem;
  font-weight: 900;
  text-transform: uppercase;
  z-index: 10;
}

.promo-poster-mini {
  width: calc(100% + 4rem);
  margin: -2rem -2rem 1.5rem -2rem;
  height: 180px;
  overflow: hidden;
}

.promo-poster-mini img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: 0.5s;
}

.promo-card:hover .promo-poster-mini img {
  transform: scale(1.1);
}

.promo-actions { display: flex; gap: 0.6rem; position: relative; z-index: 20; }
.action-btn {
  width: 32px; height: 32px; border-radius: 8px;
  background: rgba(255,255,255,0.05); border: none;
  color: #666; cursor: pointer; transition: 0.2s;
}
.action-btn.edit:hover { background: #4fc3f7; color: white; }
.action-btn.delete:hover { background: #ff3366; color: white; }

.promo-main-info { margin-bottom: 1.5rem; }
.promo-title { font-size: 1.3rem; font-weight: 800; color: white; margin-bottom: 0.2rem; }
.promo-desc { font-size: 0.85rem; color: #666; line-height: 1.4; height: 2.4rem; overflow: hidden; }

.promo-coupon-box {
  background: rgba(0,0,0,0.25);
  border: 1.5px dashed rgba(232, 136, 42, 0.3);
  border-radius: 16px;
  padding: 1rem; text-align: center; margin-bottom: 1.5rem;
}
.coupon-label { font-size: 0.6rem; color: #888; font-weight: 700; margin-bottom: 0.2rem; }
.coupon-code { font-family: 'Space Mono', monospace; font-size: 1.4rem; font-weight: 900; color: #e8882a; cursor: pointer; }

.promo-details { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; }
.detail-item { display: flex; flex-direction: column; }
.detail-label { font-size: 0.65rem; color: #555; text-transform: uppercase; }
.detail-value { font-weight: 800; font-size: 0.95rem; color: #ccc; }
.detail-value.highlight { color: #00e676; font-size: 1.2rem; }

.promo-card-footer {
  margin-top: 1.5rem; padding-top: 1.2rem;
  border-top: 1px solid rgba(255,255,255,0.05);
}
.promo-period { font-size: 0.75rem; color: #555; display: flex; align-items: center; }

.modal-overlay {
  position: fixed; inset: 0; background: rgba(0,0,0,0.85);
  backdrop-filter: blur(10px); display: flex; align-items: center;
  justify-content: center; z-index: 3000; padding: 1rem;
}
.modal-box { width: 100%; max-width: 600px; padding: 2.5rem; border-radius: 32px; }
.animate-pop { animation: pop 0.4s cubic-bezier(0.34, 1.56, 0.64, 1); }
@keyframes pop { from { opacity: 0; transform: scale(0.9); } }

.modal-top { display: flex; justify-content: space-between; align-items: center; margin-bottom: 2rem; }
.close-modal { background: none; border: none; font-size: 2.5rem; color: #444; cursor: pointer; }

.form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 1.5rem; }
.field.full { grid-column: span 2; }
.field label { display: block; font-size: 0.8rem; font-weight: 700; color: #555; text-transform: uppercase; margin-bottom: 0.5rem; }
.field input, .field select, .field textarea {
  width: 100%; padding: 0.8rem 1.2rem; background: rgba(255,255,255,0.03);
  border: 1px solid rgba(255,255,255,0.08); border-radius: 12px;
  color: white; outline: none; transition: 0.3s;
}
.field input:focus { border-color: #e8882a; }
.form-select-custom { appearance: none; background: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='24' height='24' viewBox='0 0 24 24' fill='none' stroke='%23444' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Cpolyline points='6 9 12 15 18 9'%3E%3C/polyline%3E%3C/svg%3E") no-repeat right 1rem center; background-size: 1.2rem; }

.modal-actions { margin-top: 2.5rem; display: flex; gap: 1rem; }
.btn-cancel { flex: 1; padding: 1rem; border-radius: 14px; border: 1px solid rgba(255,255,255,0.05); background: none; color: #666; font-weight: 800; cursor: pointer; }
.btn-save-gold { flex: 2; padding: 1rem; border-radius: 14px; border: none; background: #e8882a; color: white; font-weight: 900; cursor: pointer; }

.empty-promo { grid-column: 1 / -1; text-align: center; padding: 5rem; color: #333; }
.loading-state { text-align: center; padding: 10rem 0; color: #444; }
.loader-circle { width: 40px; height: 40px; border: 3px solid rgba(255,255,255,0.05); border-top-color: #e8882a; border-radius: 50%; animation: spin 0.8s linear infinite; margin: 0 auto 1.5rem; }
@keyframes spin { to { transform: rotate(360deg); } }

.ms-2 { margin-left: 0.5rem; }
.me-2 { margin-right: 0.5rem; }
.mb-3 { margin-bottom: 1rem; }
.mb-5 { margin-bottom: 3rem; }
</style>
