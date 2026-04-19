<template>
  <div class="movies-page container">
    <div class="header-section">
      <h1 class="gradient-text">Danh Sách Phim</h1>
      
      <div class="filters glass-panel">
        <input type="text" v-model="filters.tuKhoa" class="form-input" placeholder="Tìm kiếm phim..." @input="fetchMovies">
        
        <select v-model="filters.trangThai" class="form-input" @change="fetchMovies">
          <option value="">Tất cả trạng thái</option>
          <option value="Đang chiếu">Đang chiếu</option>
          <option value="Sắp chiếu">Sắp chiếu</option>
        </select>
      </div>
    </div>

    <div v-if="loading" class="loading">Đang tải danh sách phim...</div>
    <div v-else-if="phims.length === 0" class="empty-state">Không tìm thấy phim nào.</div>
    
    <div v-else class="movie-grid">
      <div v-for="phim in phims" :key="phim.maPhim" class="movie-card glass-panel hover-scale">
        <div class="poster-container">
          <img :src="phim.poster || 'https://via.placeholder.com/400x600/1e1e2d/9aa0a6?text=No+Poster'" alt="Poster" class="poster">
          <div class="overlay">
            <router-link :to="'/movies/' + phim.maPhim" class="btn btn-outline" style="border-color: white; color: white;">Chi Tiết</router-link>
            <router-link v-if="phim.trangThaiPhim === 'Đang chiếu'" :to="`/booking?phim=${phim.maPhim}`" class="btn btn-primary" style="margin-top: 1rem;">Đặt Vé</router-link>
            <button v-else class="btn btn-outline" style="margin-top: 1rem; opacity: 0.5; cursor: not-allowed; border-color: #888; color: #888;">Sắp Chiếu</button>
          </div>
        </div>
        <div class="info">
          <div class="meta">
            <span class="genre">{{ phim.theLoai }}</span>
            <span class="duration">{{ phim.thoiLuong }} phút</span>
          </div>
          <h3 class="title">{{ phim.tenPhim }}</h3>
          <div class="status-badge" :class="phim.trangThaiPhim === 'Đang chiếu' ? 'active' : ''">
            {{ phim.trangThaiPhim }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue';
import { useRoute } from 'vue-router';
import { movieApi } from '../../api/movieApi';

const route = useRoute();
const phims = ref([]);
const loading = ref(true);

const filters = ref({
  tuKhoa: '',
  trangThai: route.query.trangThai || '',
  theLoai: ''
});

const fetchMovies = async () => {
  loading.value = true;
  try {
    const res = await movieApi.getMovies(filters.value);
    if (res.success) {
      phims.value = res.data;
    }
  } catch (error) {
    console.error("Lỗi tải phim", error);
  } finally {
    loading.value = false;
  }
};

watch(() => route.query.trangThai, (newStatus) => {
  filters.value.trangThai = newStatus || '';
  fetchMovies();
});

onMounted(() => {
  fetchMovies();
});
</script>

<style scoped>
.movies-page {
  padding: 3rem 1.5rem;
}

.header-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 3rem;
  flex-wrap: wrap;
  gap: 1rem;
}

.filters {
  display: flex;
  gap: 1rem;
  padding: 1rem;
}

.filters .form-input {
  width: auto;
  min-width: 200px;
}

.movie-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 2.5rem;
}

/* Reusing cards from Home */
.movie-card {
  border-radius: var(--radius-md);
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.poster-container {
  position: relative;
  width: 100%;
  aspect-ratio: 2/3;
  overflow: hidden;
}

.poster {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.5s ease;
}

.overlay {
  position: absolute;
  top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(0,0,0,0.8);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.3s ease;
  backdrop-filter: blur(5px);
}

.movie-card:hover .poster {
  transform: scale(1.1);
}

.movie-card:hover .overlay {
  opacity: 1;
}

.info {
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  flex-grow: 1;
}

.meta {
  display: flex;
  justify-content: space-between;
  font-size: 0.85rem;
  color: var(--color-text-muted);
  margin-bottom: 0.8rem;
}

.genre {
  color: var(--color-accent);
  background: rgba(0, 229, 255, 0.1);
  padding: 0.2rem 0.6rem;
  border-radius: 4px;
}

.title {
  font-size: 1.3rem;
  font-weight: 700;
  margin-bottom: 1rem;
}

.status-badge {
  display: inline-block;
  padding: 0.3rem 0.8rem;
  border-radius: 4px;
  font-size: 0.85rem;
  font-weight: 600;
  background: rgba(255,255,255,0.1);
  color: var(--color-text-muted);
  width: fit-content;
}

.status-badge.active {
  background: rgba(0, 230, 118, 0.1);
  color: var(--color-success);
}
</style>
