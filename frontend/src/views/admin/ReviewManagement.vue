<template>
  <div class="review-management">
    <div class="header-actions">
      <h2 class="page-title">Quản Lý Đánh Giá & Bình Luận</h2>
    </div>

    <div v-if="loading" class="loading-state">Đang tải dữ liệu...</div>
    
    <div v-else class="table-container glass-panel">
      <table class="admin-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Người Dùng</th>
            <th>Phim</th>
            <th>Điểm</th>
            <th>Nội Dung Bình Luận</th>
            <th>Ngày Đăng</th>
            <th>Thao Tác</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="review in reviews" :key="review.maDanhGia">
            <td>{{ review.maDanhGia }}</td>
            <td class="font-bold">{{ review.tenNguoiDung }}</td>
            <td class="text-primary">{{ review.tenPhim }}</td>
            <td>
              <span class="star-rating">⭐ {{ review.diemSo }}</span>
            </td>
            <td class="comment-cell">
              <div class="comment-text" :title="review.binhLuan">{{ review.binhLuan || '(Không có bình luận)' }}</div>
            </td>
            <td>{{ new Date(review.ngayDanhGia).toLocaleString('vi-VN') }}</td>
            <td>
              <button 
                class="btn-icon delete" 
                title="Xóa bình luận vi phạm" 
                @click="deleteReview(review.maDanhGia)"
              >
                🗑️
              </button>
            </td>
          </tr>
          <tr v-if="reviews.length === 0">
            <td colspan="7" class="text-center p-4">Chưa có đánh giá nào.</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { movieApi } from '../../api/movieApi';

const reviews = ref([]);
const loading = ref(true);

const loadReviews = async () => {
  try {
    const res = await movieApi.getReviews();
    if (res.success) {
      reviews.value = res.data;
    }
  } catch (error) {
    console.error('Lỗi tải đánh giá:', error);
  } finally {
    loading.value = false;
  }
};

const deleteReview = async (id) => {
  if (!confirm('Bạn có chắc chắn muốn xóa bình luận này?')) return;
  try {
    const res = await movieApi.deleteReview(id);
    if (res.success) {
      alert('Đã xóa đánh giá thành công');
      loadReviews();
    }
  } catch (error) {
    alert(error.message || 'Lỗi khi xóa đánh giá');
  }
};

onMounted(() => {
  loadReviews();
});
</script>

<style scoped>
.page-title { font-size: 1.8rem; font-weight: 700; margin-bottom: 1.5rem; }
.header-actions { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; }
.table-container { border-radius: var(--radius-lg); overflow: hidden; background: var(--color-bg-card); }
.admin-table { width: 100%; border-collapse: collapse; }
.admin-table th, .admin-table td { padding: 1rem; text-align: left; border-bottom: 1px solid rgba(255,255,255,0.05); }
.admin-table th { background: rgba(0,0,0,0.3); font-weight: 600; color: var(--color-text-muted); text-transform: uppercase; font-size: 0.85rem; }
.admin-table tr:hover { background: rgba(255,255,255,0.02); }
.font-bold { font-weight: bold; color: white; }
.text-primary { color: var(--color-primary); font-weight: 500;}
.star-rating { font-weight: bold; color: var(--color-accent); }
.comment-cell { max-width: 300px; }
.comment-text {
  display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical;
  overflow: hidden; text-overflow: ellipsis; font-size: 0.9rem; color: #ccc;
}
.btn-icon { background: none; border: none; font-size: 1.2rem; cursor: pointer; opacity: 0.7; transition: 0.2s; }
.btn-icon:hover { opacity: 1; transform: scale(1.1); }
.btn-icon.delete:hover { color: var(--color-error); }
.loading-state { text-align: center; padding: 3rem; color: #888; }
.text-center { text-align: center; }
</style>
