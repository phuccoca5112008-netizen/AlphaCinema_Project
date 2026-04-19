<template>
  <div class="home-page">

    <!-- ═══ HERO SLIDER ═══ -->
    <section class="hero">
      <div class="hero-slides">
        <transition name="slide-fade" mode="out-in">
          <div class="hero-slide" :key="currentSlide" :style="{ backgroundImage: `url('${slides[currentSlide].backdrop}')` }">
            <div class="hero-gradient"></div>
            <div class="container hero-content">
              <div class="hero-meta">
                <span class="badge badge-orange uppercase">{{ slides[currentSlide].type }}</span>
                <span class="badge badge-red uppercase ms-1">T{{ slides[currentSlide].age }}</span>
              </div>
              <h1 class="hero-title">{{ slides[currentSlide].title }}</h1>
              <p class="hero-desc">{{ slides[currentSlide].desc }}</p>
              <div class="hero-actions">
                <router-link v-if="allPhims.find(p => p.maPhim === slides[currentSlide].id)?.trangThaiPhim !== 'Sắp chiếu'" :to="'/booking?phim=' + slides[currentSlide].id" class="btn btn-primary">
                  🎟 Mua Vé Ngay
                </router-link>
                <button v-else class="btn btn-outline" style="opacity: 0.6; cursor: not-allowed; color: #888; border-color: #888;">
                  🔔 Sắp Chiếu
                </button>
                <router-link :to="'/movies/' + slides[currentSlide].id" class="btn btn-outline border-white">
                  Xem Chi Tiết
                </router-link>
              </div>
            </div>
          </div>
        </transition>
      </div>

      <!-- Dots -->
      <div class="hero-dots">
        <button v-for="(s, i) in slides" :key="i"
          class="dot" :class="{ active: i === currentSlide }"
          @click="currentSlide = i; restartTimer()">
        </button>
      </div>

      <!-- Prev/Next -->
      <button class="hero-arrow left" @click="prev">&#8249;</button>
      <button class="hero-arrow right" @click="next">&#8250;</button>
    </section>



    <!-- ═══ TOP 4 PHIM DOANH THU ═══ -->
    <section class="movies-section container text-center">
      <div class="centered-header">
        <h2 class="section-title">⭐ Top các phim thịnh hành</h2>
        <p class="text-sub mb-4">Khám phá những bộ phim được khán giả yêu thích và quan tâm nhất tại Alpha Cinema.</p>
      </div>

      <div v-if="loading" class="empty-state">Đang tải phim...</div>
      <div v-else class="movie-grid">
        <router-link v-for="(phim, index) in topMovies" :key="phim.maPhim"
          :to="'/movies/' + phim.maPhim" class="mcard" :class="'rank-' + (index + 1)">
          
          <!-- Medal Image Badge (Top 3 only) -->
          <div class="medal-image-wrap" v-if="index < 3">
            <img v-if="index === 0" src="/img/medals/gold.png" alt="Gold Medal" class="medal-image">
            <img v-else-if="index === 1" src="/img/medals/silver.png" alt="Silver Medal" class="medal-image">
            <img v-else-if="index === 2" src="/img/medals/bronze.png" alt="Bronze Medal" class="medal-image">
          </div>
          <div class="rank-badge-basic" v-else>
            #{{ index + 1 }}
          </div>

          <div class="mcard-poster">
            <img :src="phim.poster" :alt="phim.tenPhim" class="mcard-img" @error="fallbackImg">
            <div class="mcard-overlay">
              <router-link v-if="phim.trangThaiPhim === 'Đang chiếu'" :to="'/booking?phim=' + phim.maPhim" class="btn btn-primary" @click.stop>🎟 Mua Vé</router-link>
              <button v-else class="btn btn-outline" style="border-color: white; color: white; opacity: 0.8; cursor: default;" @click.stop>Sắp Chiếu</button>
            </div>
            <div class="mcard-badge">{{ phim.thoiLuong }}p</div>
          </div>
          <div class="mcard-info">
            <p class="mcard-genre">{{ phim.theLoai }}</p>
            <h3 class="mcard-title">{{ phim.tenPhim }}</h3>
            <!-- Doanh thu ẩn theo yêu cầu -->
            <div class="mcard-rating" v-if="phim.diemTrungBinh > 0">
              ⭐ {{ phim.diemTrungBinh.toFixed(1) }} <span class="review-count">({{ phim.soLuongDanhGia }} đánh giá)</span>
            </div>
            <!-- Ẩn nếu chưa có đánh giá -->
          </div>
        </router-link>
      </div>
    </section>

    <!-- Sections removed as per user request (moved to Phim menu) -->

    <!-- ═══ PROMO STRIP ═══ -->
    <section class="promo-section container mt-5 mb-5">
      <div class="promo-grid">
        <router-link to="/promotions" v-for="promo in homePromos" :key="promo.id" 
          class="promo-card" :style="{ backgroundImage: `url(${promo.image})` }">
          <div class="promo-overlay">
            <span class="badge mb-2" 
              :class="{ 
                'badge-gold': promo.type === 'member', 
                'badge-orange': promo.type === 'ticket',
                'badge-red': promo.type === 'payment'
              }">
              {{ promo.tag.toUpperCase() }}
            </span>
            <h3>{{ promo.title }}</h3>
            <p>{{ promo.description }}</p>
          </div>
        </router-link>
      </div>
    </section>

  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import { movieApi } from '../../api/movieApi';
import { promotions as promoData } from '../../data/promotions';

const router = useRouter();

const getImageUrl = (name) => {
  return new URL(`../../assets/promotions/${name}`, import.meta.url).href;
};

const homePromos = computed(() => {
  return promoData.slice(0, 3).map(p => ({
    ...p,
    image: getImageUrl(p.imageName)
  }));
});

// ─── Slider data ───────────────────────────────────────────
const slides = ref([
  { id: 1, title: 'DUNE: HÀNH TINH CÁT 2', type: 'VIỄN TƯỞNG', age: 13,
    desc: 'Paul Atreides hợp lực với Chani và người Fremen để trả thù những kẻ đã hủy diệt gia đình mình.',
    backdrop: 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/original/xOMo8BRK7PfcJv9JCnx7s5hj0PX.jpg' },
  { id: 2, title: 'DEADPOOL & WOLVERINE', type: 'HÀNH ĐỘNG', age: 18,
    desc: 'Cặp đôi hoàn cảnh nhất vũ trụ Marvel kết hợp trong một nhiệm vụ giải cứu đa vũ trụ đầy tiếng cười và bạo lực.',
    backdrop: 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/original/yDHYTfA3R0jFYba16jBB1ef8oIt.jpg' },
  { id: 5, title: 'INTERSTELLAR', type: 'VIỄN TƯỞNG', age: 13,
    desc: 'Một nhóm các nhà thám hiểm du hành xuyên qua một lỗ đen ngoài không gian để tìm ngôi nhà mới cho nhân loại.',
    backdrop: 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/original/2ssWTSVklAEc98frZUQhgtGHx7s.jpg' },
  { id: 6, title: 'MOANA 2', type: 'PHIÊU LƯU', age: 0,
    desc: 'Hành trình mới của Moana và Maui đến những vùng biển xa xôi để kết nối lại các hòn đảo bị chia cắt.',
    backdrop: 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/original/vYqt6kb4lcF8wwqsMMaULkP9OEn.jpg' },
  { id: 4, title: 'OPPENHEIMER', type: 'LỊCH SỬ', age: 16,
    desc: 'Câu chuyện về J. Robert Oppenheimer và quá trình tạo ra bom nguyên tử, thay đổi dòng lịch sử nhân loại.',
    backdrop: 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/original/neeNHeXjMF5fXoCJRsOmkNGC7q.jpg' },
]);
const currentSlide = ref(0);
let timer = null;

const next = () => { currentSlide.value = (currentSlide.value + 1) % slides.value.length; restartTimer(); };
const prev = () => { currentSlide.value = (currentSlide.value - 1 + slides.value.length) % slides.value.length; restartTimer(); };
const restartTimer = () => { clearInterval(timer); timer = setInterval(next, 6000); };

onMounted(() => { timer = setInterval(next, 6000); loadPhims(); });
onUnmounted(() => clearInterval(timer));

// ─── Movies ────────────────────────────────────────────────
const allPhims = ref([]);
const loading = ref(true);

const dangChieu = computed(() => allPhims.value.filter(p => p.trangThaiPhim === 'Đang chiếu'));
const sapChieu  = computed(() => allPhims.value.filter(p => p.trangThaiPhim === 'Sắp chiếu'));

const topMovies = computed(() => {
  return [...allPhims.value]
    .sort((a, b) => b.doanhThu - a.doanhThu)
    .slice(0, 4);
});

const otherPhims = computed(() => {
  const top4Ids = topMovies.value.map(m => m.maPhim);
  return dangChieu.value.filter(p => !top4Ids.includes(p.maPhim));
});

const formatVND = (val) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val);
};

const loadPhims = async () => {
  try {
    const res = await movieApi.getMovies();
    if (res.success) allPhims.value = res.data;
  } catch (e) { console.error(e); }
  finally { loading.value = false; }
};

const fallbackImg = (e) => {
  e.target.src = 'https://placehold.co/400x600/1a1a1a/E8882A?text=Alpha+Cinema';
};


</script>

<style scoped>
/* ─── HERO ─── */
.hero {
  position: relative;
  height: 88vh;
  min-height: 550px;
  overflow: hidden;
}
.hero-slide {
  position: absolute;
  inset: 0;
  background-size: cover;
  background-position: center top;
}
.hero-gradient {
  position: absolute;
  inset: 0;
  background: linear-gradient(
    to right,
    rgba(0,0,0,0.85) 0%,
    rgba(0,0,0,0.5) 50%,
    transparent 100%
  ),
  linear-gradient(to top, rgba(13,13,13,1) 0%, transparent 50%);
}
.hero-content {
  position: relative;
  z-index: 2;
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: center;
  padding-top: 4rem;
  max-width: 650px;
}
.hero-meta { margin-bottom: 1rem; }
.ms-1 { margin-left: 0.4rem; }
.hero-title {
  font-size: clamp(2.2rem, 5vw, 4rem);
  font-weight: 900;
  text-transform: uppercase;
  line-height: 1.1;
  margin-bottom: 1rem;
  text-shadow: 0 4px 20px rgba(0,0,0,0.8);
}
.hero-desc {
  font-size: 1.05rem;
  color: rgba(255,255,255,0.8);
  margin-bottom: 2rem;
  line-height: 1.7;
  max-width: 520px;
}
.hero-actions { display: flex; gap: 1rem; flex-wrap: wrap; }
.border-white { border-color: rgba(255,255,255,0.4) !important; color: white !important; }
.border-white:hover { border-color: var(--color-primary) !important; color: var(--color-primary) !important; }

/* Dots */
.hero-dots {
  position: absolute; bottom: 2rem; left: 50%;
  transform: translateX(-50%);
  display: flex; gap: 8px; z-index: 5;
}
.dot {
  width: 8px; height: 8px;
  border-radius: 50%;
  background: rgba(255,255,255,0.3);
  border: none; cursor: pointer;
  transition: 0.3s; padding: 0;
}
.dot.active { background: var(--color-primary); width: 24px; border-radius: 4px; }

/* Arrows */
.hero-arrow {
  position: absolute; top: 50%; transform: translateY(-50%);
  z-index: 5;
  background: rgba(0,0,0,0.4);
  border: 1px solid rgba(255,255,255,0.15);
  color: white; font-size: 2rem;
  width: 48px; height: 48px;
  border-radius: 50%;
  cursor: pointer;
  display: flex; align-items: center; justify-content: center;
  transition: 0.2s;
  line-height: 1;
}
.hero-arrow:hover { background: var(--color-primary); border-color: var(--color-primary); }
.hero-arrow.left { left: 2rem; }
.hero-arrow.right { right: 2rem; }

/* Vue transition for hero */
.slide-fade-enter-active, .slide-fade-leave-active { transition: opacity 0.6s ease; }
.slide-fade-enter-from, .slide-fade-leave-to { opacity: 0; }



/* ─── MOVIES SECTION ─── */
.movies-section { padding: 4rem 0 2rem; }
.section-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  margin-bottom: 2.5rem;
}

/* GRID CARDS */
.movie-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 2.5rem;
  margin-top: 1rem;
}
@media (max-width: 1200px) {
  .movie-grid { grid-template-columns: repeat(3, 1fr); gap: 2rem; }
}
@media (max-width: 992px) {
  .movie-grid { grid-template-columns: repeat(2, 1fr); gap: 1.5rem; }
}
@media (max-width: 576px) {
  .movie-grid { grid-template-columns: 1fr; gap: 1.5rem; }
}
.mcard {
  display: block;
  text-decoration: none;
  color: inherit;
  background: var(--color-bg-card);
  border-radius: var(--radius-md);
  overflow: hidden;
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}
.mcard:hover { transform: translateY(-6px); box-shadow: 0 20px 40px rgba(0,0,0,0.5); }
.mcard-poster { position: relative; aspect-ratio: 2/3; overflow: hidden; }
.mcard-img {
  width: 100%; height: 100%;
  object-fit: cover;
  transition: transform 0.5s ease;
}
.mcard:hover .mcard-img { transform: scale(1.06); }
.mcard-overlay {
  position: absolute; inset: 0;
  background: rgba(0,0,0,0.75);
  display: flex; align-items: center; justify-content: center;
  opacity: 0;
  transition: opacity 0.3s;
}
.mcard:hover .mcard-overlay { opacity: 1; }
.mcard-badge {
  position: absolute; top: 10px; right: 10px;
  background: rgba(0,0,0,0.7);
  color: var(--color-accent);
  padding: 3px 8px;
  border-radius: 4px;
  font-size: 0.78rem;
  font-weight: 700;
}
.mcard-info { padding: 1rem; }
.mcard-genre { font-size: 0.78rem; color: var(--color-primary); font-weight: 600; text-transform: uppercase; margin-bottom: 0.3rem; }
.mcard-title { font-size: 1rem; font-weight: 700; line-height: 1.3; margin-bottom: 0.5rem; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; }
.mcard-desc { font-size: 0.85rem; color: #aaa; margin-bottom: 0.8rem; display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden; line-height: 1.4; }
.mcard-rating { font-size: 0.85rem; color: var(--color-accent); font-weight: bold; }
.review-count { font-size: 0.75rem; color: #888; font-weight: normal; margin-left: 4px; }

/* SAP CHIEU – horizontal list */
.movie-grid-small {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1rem;
}
.mcard-small {
  display: flex;
  gap: 1rem;
  background: var(--color-bg-card);
  border-radius: var(--radius-sm);
  padding: 0.75rem;
  text-decoration: none;
  color: inherit;
  transition: background 0.2s;
  align-items: center;
}
.mcard-small:hover { background: #222; }
.mcard-small img { width: 80px; height: 120px; object-fit: cover; border-radius: 6px; flex-shrink: 0; }
.mcard-small-info h4 { font-size: 1rem; font-weight: 700; margin-bottom: 0.3rem; }
.small-genre { font-size: 0.8rem; color: var(--color-primary); margin-bottom: 0.4rem; font-weight: 600; text-transform: uppercase; }
.small-desc { font-size: 0.8rem; color: #aaa; display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden; line-height: 1.4; }

/* ─── PROMO SECTION ─── */
.promo-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1.5rem;
}
.promo-card {
  height: 240px;
  border-radius: var(--radius-lg);
  background-size: cover;
  background-position: center;
  position: relative;
  overflow: hidden;
  cursor: pointer;
  transition: transform 0.3s;
}
.promo-card:hover { transform: translateY(-4px); box-shadow: 0 16px 35px rgba(0,0,0,0.5); }
.promo-overlay {
  position: absolute; inset: 0;
  background: linear-gradient(to top, rgba(0,0,0,0.85) 0%, rgba(0,0,0,0.2) 60%);
  display: flex; flex-direction: column; justify-content: flex-end;
  padding: 1.5rem;
}
.promo-overlay h3 { 
  font-size: 1.2rem; 
  font-weight: 800; 
  color: #fff; 
  margin: 0.3rem 0;
  display: -webkit-box;
  -webkit-line-clamp: 1;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
.promo-overlay p  { 
  font-size: 0.85rem; 
  color: rgba(255,255,255,0.75);
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  line-height: 1.4;
}
.mb-2 { margin-bottom: 0.5rem; }
.mt-5 { margin-top: 3rem; }
.mb-5 { margin-bottom: 3rem; }



.centered-header { margin-bottom: 3rem; }

/* ─── MEDALS ─── */
.mcard { position: relative; }

.medal-image-wrap {
  position: absolute;
  top: -15px;
  right: 15px;
  z-index: 10;
  width: 90px;
  height: 110px;
  display: flex;
  align-items: center;
  justify-content: center;
  /* Blend mode to remove black background from medals */
  mix-blend-mode: screen;
  transition: transform 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}
.medal-image {
  width: 100%;
  height: 100%;
  object-fit: contain;
  /* Ensure the gold/silver colors pop */
  filter: brightness(1.2) contrast(1.1);
}
.mcard:hover .medal-image-wrap { transform: scale(1.1) translateY(-5px); }

.rank-badge-basic {
  position: absolute;
  top: 10px;
  left: 10px;
  z-index: 10;
  color: #fff;
  padding: 4px 10px;
  font-weight: 800;
  font-size: 1rem;
  opacity: 0.6;
}

.mcard-revenue {
  font-size: 0.85rem;
  color: var(--color-accent);
  font-weight: 700;
  margin-bottom: 0.4rem;
}

.mcard-title-small { font-size: 0.95rem; font-weight: 700; margin-bottom: 0.3rem; }
.section-subtitle { font-size: 1.4rem; font-weight: 800; color: rgba(255,255,255,0.6); }
.pt-0 { padding-top: 0 !important; }

/* Effects for ranks */
.rank-1 { box-shadow: 0 0 20px rgba(241, 196, 15, 0.3); }
.rank-1:hover { box-shadow: 0 0 35px rgba(241, 196, 15, 0.5); }

/* Responsive */
@media (max-width: 768px) {
  .medal-badge { width: 40px; height: 40px; top: -5px; left: -5px; }
  .rank-num { font-size: 0.7rem; }
  .medal-icon { font-size: 1rem; }
}
</style>
