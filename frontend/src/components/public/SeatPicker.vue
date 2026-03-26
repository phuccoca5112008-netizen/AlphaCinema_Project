<template>
  <div class="seat-picker-container">
    <!-- UI SCREEN -->
    <div class="screen-box">
      <div class="screen-surface"></div>
      <div class="screen-text">MÀN HÌNH</div>
      <div class="screen-light"></div>
    </div>

    <!-- SEAT MAP -->
    <div class="seat-grid" v-if="!loading">
      <div v-for="row in seatRows" :key="row.hang" class="seat-row">
        <span class="row-id left">{{ row.hang }}</span>
        
        <div class="seat-list">
          <div 
            v-for="seat in row.ghes" 
            :key="seat.maGhe"
            class="seat-wrapper"
            :class="{ 
              'selected': isSelected(seat.maGhe),
              'booked': seat.daDat,
              'vip': seat.loaiGhe === 'VIP'
            }"
            @click="onSelect(seat)"
          >
            <div class="seat-item" :title="seat.daDat ? 'Đã bán' : (seat.loaiGhe === 'VIP' ? 'VIP' : 'Thường') + ' ' + seat.hang + seat.soGhe">
              <span class="seat-num">{{ seat.soGhe }}</span>
            </div>
          </div>
        </div>

        <span class="row-id right">{{ row.hang }}</span>
      </div>
    </div>

    <div v-else class="loading-state">
      <div class="spinner"></div>
      <p>Đang tải sơ đồ ghế...</p>
    </div>

    <!-- LEGEND -->
    <div class="legend">
      <div class="legend-item">
        <div class="box regular"></div>
        <span>Ghế thường</span>
      </div>
      <div class="legend-item">
        <div class="box vip"></div>
        <span>Ghế VIP</span>
      </div>
      <div class="legend-item">
        <div class="box gold-glow"></div>
        <span>Đang chọn</span>
      </div>
      <div class="legend-item">
        <div class="box sold"></div>
        <span>Đã bán</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';

const props = defineProps({
  seats: { type: Array, default: () => [] },
  selectedIds: { type: Array, default: () => [] },
  loading: { type: Boolean, default: false }
});

const emit = defineEmits(['toggle']);

const seatRows = computed(() => {
  const map = {};
  props.seats.forEach(s => {
    if (!map[s.hang]) map[s.hang] = { hang: s.hang, ghes: [] };
    map[s.hang].ghes.push(s);
  });
  return Object.values(map)
    .sort((a,b) => a.hang.localeCompare(b.hang))
    .map(r => ({ ...r, ghes: r.ghes.sort((a,b) => a.soGhe - b.soGhe) }));
});

const isSelected = (id) => props.selectedIds.includes(id);

const onSelect = (seat) => {
  if (seat.daDat) return;
  emit('toggle', seat);
};
</script>

<style scoped>
.seat-picker-container {
  padding: 2rem 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  user-select: none;
}

/* SCREEN */
.screen-box {
  position: relative;
  width: 80%;
  max-width: 600px;
  height: 60px;
  margin-bottom: 4rem;
  perspective: 400px;
}
.screen-surface {
  width: 100%;
  height: 10px;
  background: #fff;
  border-radius: 50% / 100% 100% 0 0;
  box-shadow: 0 0 20px 2px rgba(255,255,255,0.7);
  transform: rotateX(-10deg);
}
.screen-text {
  position: absolute;
  top: 15px;
  left: 50%;
  transform: translateX(-50%);
  font-size: 0.75rem;
  letter-spacing: 0.8rem;
  color: rgba(255,255,255,0.4);
  font-weight: 700;
}
.screen-light {
  position: absolute;
  top: 10px;
  left: 0;
  width: 100%;
  height: 50px;
  background: linear-gradient(to bottom, rgba(255,255,255,0.1) 0%, transparent 100%);
  clip-path: polygon(10% 0, 90% 0, 100% 100%, 0 100%);
}

/* GRID */
.seat-grid {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-bottom: 3rem;
}
.seat-row {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}
.row-id {
  width: 20px;
  font-size: 0.85rem;
  font-weight: 800;
  color: #555;
  text-align: center;
}
.seat-list {
  display: flex;
  gap: 8px;
}

/* SEAT WRAPPER */
.seat-wrapper {
  cursor: pointer;
  transition: transform 0.2s cubic-bezier(0.4, 0, 0.2, 1);
}
.seat-wrapper:hover:not(.booked) {
  transform: scale(1.2) translateY(-2px);
}
.seat-wrapper.booked {
  cursor: not-allowed;
  opacity: 0.2;
}

/* SEAT ITEM */
.seat-item {
  width: 32px;
  height: 28px;
  background: #333;
  border: 1.5px solid #444;
  border-radius: 6px 6px 2px 2px;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  transition: all 0.3s ease;
}
.seat-num {
  font-size: 0.65rem;
  font-weight: 700;
  color: #888;
}

/* STATES */
.seat-wrapper.vip .seat-item {
  border-color: #f1c40f;
  background: rgba(241, 196, 15, 0.05);
}
.seat-wrapper.vip .seat-num { color: #f1c40f; }

.seat-wrapper.selected .seat-item {
  background: #e8882a;
  border-color: #e8882a;
  box-shadow: 0 0 15px rgba(232, 136, 42, 0.6);
}
.seat-wrapper.selected .seat-num { color: #fff; }

.seat-wrapper.booked .seat-item {
  background: #111;
  border-color: #222;
}

/* LOADING */
.loading-state {
  text-align: center;
  color: #666;
  padding: 3rem;
}
.spinner {
  width: 40px;
  height: 40px;
  border: 3px solid rgba(255,255,255,0.1);
  border-top-color: #e8882a;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 1rem;
}
@keyframes spin { to { transform: rotate(360deg); } }

/* LEGEND */
.legend {
  display: flex;
  gap: 2rem;
  background: rgba(255,255,255,0.03);
  padding: 1.2rem 2.5rem;
  border-radius: 50px;
  border: 1px solid rgba(255,255,255,0.05);
}
.legend-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 0.85rem;
  color: #999;
}
.box {
  width: 18px;
  height: 16px;
  border-radius: 4px;
}
.box.regular { background: #333; border: 1px solid #444; }
.box.vip     { border: 1px solid #f1c40f; }
.box.gold-glow { background: #e8882a; box-shadow: 0 0 10px rgba(232, 136, 42, 0.5); }
.box.sold    { background: #111; opacity: 0.3; }

@media (max-width: 768px) {
  .seat-list { gap: 4px; }
  .seat-item { width: 24px; height: 22px; }
  .seat-row { gap: 0.75rem; }
  .legend { gap: 1rem; flex-wrap: wrap; justify-content: center; }
}
</style>
