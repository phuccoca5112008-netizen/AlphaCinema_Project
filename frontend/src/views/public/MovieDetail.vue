<template>
  <div class="movie-detail container" v-if="phim">
    <div class="backdrop" :style="{ backgroundImage: `url(${phim.poster})` }">
      <div class="backdrop-overlay"></div>
    </div>

    <div class="content glass-panel">
      <div class="poster-col">
        <img :src="phim.poster || 'https://via.placeholder.com/400x600/1e1e2d/9aa0a6?text=No+Poster'" alt="Poster" class="detail-poster">
        <router-link :to="`/booking?phim=${phim.maPhim}`" class="btn btn-primary btn-block">Mua Vé Ngay</router-link>
      </div>
      
      <div class="info-col">
        <h1 class="title">{{ phim.tenPhim }}</h1>
        <div class="badges">
          <span class="badge genre">{{ phim.theLoai }}</span>
          <span class="badge duration">{{ phim.thoiLuong }} phút</span>
          <span class="badge status">{{ phim.trangThaiPhim }}</span>
        </div>
        
        <div class="rating-box">
          <div class="score">⭐ {{ phim.diemTrungBinh.toFixed(1) }}</div>
          <div class="votes">Dựa trên {{ phim.soLuongDanhGia }} đánh giá</div>
        </div>

        <div class="synopsis">
          <h3>Nội dung phim</h3>
          <p>{{ phim.tomTat || 'Chưa có thông tin tóm tắt.' }}</p>
        </div>

        <!-- REVIEWS SECTION -->
        <div class="reviews-section mt-5 pt-4">
          <div class="section-header">
            <h3>Bình luận & Đánh giá ({{ reviews.length }})</h3>
          </div>

          <!-- Review Form (Only for eligible users) -->
          <div v-if="isEligible" class="review-form glass-panel-inner mt-4 animate-slide-up">
            <h4>Chia sẻ cảm nhận của bạn</h4>
            <div class="rating-input mt-3">
              <span v-for="i in 5" :key="i" class="star-btn" 
                :class="{ active: newReview.diemSo >= i }" 
                @click="newReview.diemSo = i">⭐</span>
            </div>
            <textarea 
              v-model="newReview.noiDung" 
              placeholder="Viết bình luận của bạn tại đây..." 
              class="form-control mt-3"
              rows="3"></textarea>
            <button @click="submitReview" class="btn btn-primary mt-3" :disabled="submitting">
              {{ submitting ? 'Đang gửi...' : 'Gửi đánh giá' }}
            </button>
          </div>
          
          <div v-else-if="user" class="review-info-box mt-4">
            <p v-if="hasAlreadyReviewed">Bạn đã đánh giá phim này rồi. Cảm ơn bạn!</p>
            <p v-else>Bạn phải mua vé xem phim này mới được tham gia đánh giá nội dung.</p>
          </div>

          <!-- Reviews List -->
          <div class="reviews-list mt-4">
            <div v-if="reviews.length === 0" class="no-reviews">Chưa có bình luận nào cho phim này.</div>
            <div v-for="r in reviews" :key="r.maDanhGia" class="review-card animate-fade-in">
              <div class="rc-header">
                <div class="rc-user">
                  <div class="avatar-sm">{{ r.hoTenNguoiDung.charAt(0) }}</div>
                  <span class="name">{{ r.hoTenNguoiDung }}</span>
                </div>
                <div class="rc-meta">
                  <span class="rc-rating">⭐ {{ r.diemSo }}</span>
                  <span class="rc-date">{{ formatDate(r.ngayTao) }}</span>
                </div>
              </div>
              <p class="rc-content">{{ r.noiDung }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
  <div v-else-if="loading" class="container" style="padding: 5rem 0; text-align: center;">
    Đang tải thông tin phim...
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { useAuthStore } from '../../stores/auth';
import api from '../../api/axios';

const route = useRoute();
const authStore = useAuthStore();
const user = authStore.user;

const phim = ref(null);
const loading = ref(true);
const reviews = ref([]);
const isEligible = ref(false);
const hasAlreadyReviewed = ref(false);
const submitting = ref(false);

const newReview = ref({
  diemSo: 5,
  noiDung: ''
});

const formatDate = (dateStr) => {
  return new Date(dateStr).toLocaleDateString('vi-VN', { 
    day: '2-digit', 
    month: '2-digit', 
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  });
};

const fetchReviews = async () => {
  try {
    const res = await api.get(`/danh-gia/phim/${route.params.id}`);
    if (res.success) reviews.value = res.data;
  } catch (err) { console.error(err); }
};

const checkEligibility = async () => {
  if (!user) return;
  try {
    const res = await api.get(`/danh-gia/check-eligibility/${route.params.id}`);
    isEligible.value = res.data;
    
    // Nếu không đủ điều kiện, kiểm tra xem có phải vì đã đánh giá rồi không
    if (!res.data) {
      const existing = reviews.value.find(r => r.hoTenNguoiDung === user.hoTen);
      if (existing) hasAlreadyReviewed.value = true;
    }
  } catch (err) { console.error(err); }
};

const submitReview = async () => {
  if (!newReview.value.noiDung.trim()) return alert("Vui lòng nhập nội dung bình luận!");
  submitting.value = true;
  try {
    const res = await api.post('/danh-gia', {
      maPhim: parseInt(route.params.id),
      diemSo: newReview.value.diemSo,
      noiDung: newReview.value.noiDung
    });
    if (res.success) {
      alert("Cảm ơn bạn đã đánh giá!");
      newReview.value.noiDung = '';
      isEligible.value = false;
      hasAlreadyReviewed.value = true;
      await fetchReviews();
    }
  } catch (err) {
    alert(err.response?.data?.message || "Lỗi khi gửi đánh giá.");
  } finally {
    submitting.value = false;
  }
};

onMounted(async () => {
  const id = route.params.id;
  try {
    const res = await api.get(`/phim/${id}`);
    if (res.success) phim.value = res.data;
    await fetchReviews();
    await checkEligibility();
  } catch (error) {
    console.error("Lỗi:", error);
  } finally {
    loading.value = false;
  }
});
</script>

<style scoped>
.movie-detail {
  padding: 3rem 1.5rem;
  position: relative;
}

.backdrop {
  position: absolute;
  top: 0; left: 0; width: 100%; height: 60vh;
  background-size: cover;
  background-position: center;
  z-index: -1;
  opacity: 0.3;
  mask-image: linear-gradient(to bottom, black 0%, transparent 100%);
  -webkit-mask-image: linear-gradient(to bottom, black 0%, transparent 100%);
}

.backdrop-overlay {
  position: absolute;
  top: 0; left: 0; width: 100%; height: 100%;
  background: radial-gradient(circle at center, transparent 0%, var(--color-bg-base) 100%);
}

.content {
  display: flex;
  gap: 3rem;
  padding: 3rem;
  margin-top: 5rem;
  background: rgba(20,20,30,0.8);
}

.poster-col {
  flex: 0 0 300px;
}

.detail-poster {
  width: 100%;
  border-radius: var(--radius-md);
  box-shadow: 0 15px 35px rgba(0,0,0,0.5);
  margin-bottom: 1.5rem;
}

.btn-block {
  width: 100%;
  padding: 1rem;
  font-size: 1.1rem;
}

.info-col {
  flex: 1;
}

.title {
  font-size: 3rem;
  margin-bottom: 1rem;
  text-shadow: 0 4px 10px rgba(0,0,0,0.5);
}

.badges {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
}

.badge {
  padding: 0.4rem 1rem;
  border-radius: 20px;
  font-size: 0.9rem;
  font-weight: 600;
  border: 1px solid rgba(255,255,255,0.1);
}

.badge.genre { color: var(--color-accent); border-color: rgba(0,229,255,0.3); background: rgba(0,229,255,0.05); }
.badge.duration { color: var(--color-text-main); }
.badge.status { color: var(--color-primary); border-color: rgba(255,51,102,0.3); background: rgba(255,51,102,0.05); }

.rating-box {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 2.5rem;
  background: rgba(0,0,0,0.3);
  padding: 1rem 1.5rem;
  border-radius: var(--radius-sm);
  width: fit-content;
  border-left: 4px solid var(--color-warning);
}

.score {
  font-size: 1.8rem;
  font-weight: 800;
  color: var(--color-warning);
}

.votes {
  color: var(--color-text-muted);
}

.synopsis h3 {
  font-size: 1.4rem;
  margin-bottom: 1rem;
  color: var(--color-text-main);
}

.synopsis p {
  color: var(--color-text-muted);
  line-height: 1.8;
  font-size: 1.05rem;
}

@media (max-width: 768px) {
  .content {
    flex-direction: column;
    padding: 1.5rem;
  }
  .poster-col {
    flex: auto;
    max-width: 300px;
    margin: 0 auto;
  }
  .title { font-size: 2rem; }
}

/* REVIEWS STYLES */
.reviews-section {
  border-top: 1px solid rgba(255,255,255,0.05);
}
.section-header h3 { font-size: 1.5rem; color: #fff; }

.glass-panel-inner {
  background: rgba(255,255,255,0.03);
  border: 1px solid rgba(255,255,255,0.05);
  padding: 1.5rem;
  border-radius: 16px;
}

.review-info-box {
  background: rgba(255, 193, 7, 0.05);
  border: 1px solid rgba(255, 193, 7, 0.2);
  padding: 1rem;
  border-radius: 12px;
  color: #ffc107;
  font-size: 0.9rem;
  text-align: center;
}

.star-btn {
  font-size: 1.5rem;
  cursor: pointer;
  filter: grayscale(1);
  transition: 0.2s;
}
.star-btn.active { filter: grayscale(0); transform: scale(1.1); }

.review-card {
  background: rgba(0,0,0,0.2);
  padding: 1.25rem;
  border-radius: 12px;
  margin-bottom: 1rem;
  border-left: 2px solid transparent;
  transition: 0.3s;
}
.review-card:hover { border-left-color: var(--color-primary); background: rgba(0,0,0,0.3); }

.rc-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.75rem;
}

.rc-user {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.avatar-sm {
  width: 32px; height: 32px;
  background: var(--color-primary);
  border-radius: 50%;
  display: flex; align-items: center; justify-content: center;
  font-weight: 800; font-size: 0.8rem;
}

.rc-meta {
  display: flex;
  gap: 1rem;
  font-size: 0.8rem;
}
.rc-rating { color: var(--color-warning); font-weight: 700; }
.rc-date { color: var(--color-text-muted); }

.rc-content {
  color: #ddd;
  line-height: 1.6;
}

.animate-slide-up { animation: slideUp 0.5s ease-out; }
@keyframes slideUp { from { opacity: 0; transform: translateY(20px); } to { opacity: 1; transform: translateY(0); } }
.animate-fade-in { animation: fadeIn 0.5s ease-out; }
@keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
</style>
