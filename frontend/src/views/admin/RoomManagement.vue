<template>
  <div class="room-management">
    <div class="header-actions">
      <h2 class="page-title">Quản Lý Phòng Chiếu & Sơ Đồ Ghế</h2>
      <button class="btn btn-primary" @click="createRoom">+ Thêm Phòng Chiếu</button>
    </div>

    <div class="split-layout">
      <!-- Cột Trái: Danh Sách Phòng -->
      <div class="room-list glass-panel">
        <h3 class="panel-title">Danh Sách Phòng</h3>
        <ul class="rooms">
          <li 
            v-for="r in rooms" 
            :key="r.maPhong" 
            :class="{ active: r.maPhong === selectedRoom?.maPhong }"
            @click="selectRoom(r.maPhong)"
          >
            🎬 {{ r.tenPhong }}
          </li>
          <li v-if="rooms.length === 0" class="empty-list">Chưa có phòng chiếu nào.</li>
        </ul>
      </div>

      <!-- Cột Phải: Trình Tảo Sơ Đồ Ghế -->
      <div class="seat-map-builder glass-panel" v-if="selectedRoom">
        <div class="builder-header">
          <h3 class="panel-title">Sơ đồ ghế: {{ selectedRoom.tenPhong }}</h3>
          <button class="btn btn-outline text-error" @click="deleteRoom(selectedRoom.maPhong)">🗑️ Xóa Phòng Này</button>
        </div>

        <div class="builder-controls">
          <div class="form-group flex-1">
            <label>Số Hàng (Ví dụ: 8 hàng A-H)</label>
            <input type="number" v-model="genRows" class="form-input" min="1" max="20" />
          </div>
          <div class="form-group flex-1">
            <label>Ghế / Hàng (Ví dụ: 10 ghế)</label>
            <input type="number" v-model="genCols" class="form-input" min="1" max="30" />
          </div>
          <button class="btn btn-primary mt-4" @click="generateSeats">Tạo Sơ Đồ Mới</button>
        </div>

        <div class="seat-instruction">
          <p><i>Hướng dẫn: Nhấn vào từng ghế bên dưới để thay đổi loại ghế (Thường ↔ VIP).</i></p>
          <div class="legend mt-2">
            <div class="seat-legend thuong"></div> <span>Thường</span>
            <div class="seat-legend vip"></div> <span>VIP</span>
          </div>
        </div>

        <!-- Bản Đồ Ghế Trực Quan -->
        <div class="seat-grid-container" v-if="seatMatrix.length > 0">
          <div class="screen">MÀN HÌNH</div>
          <div v-for="(row, index) in seatMatrix" :key="index" class="seat-row">
            <div class="row-label">{{ row[0]?.hang }}</div>
            <div class="seats">
              <div 
                v-for="seat in row" 
                :key="seat.maGhe" 
                class="seat" 
                :class="{ 'seat-vip': seat.loaiGhe === 'VIP', 'seat-thuong': seat.loaiGhe === 'Thuong' }"
                @click="toggleSeatType(seat)"
                :title="`${seat.hang}${seat.soGhe} - ${seat.loaiGhe === 'VIP' ? 'VIP' : 'Thường'}`"
              >
                {{ seat.soGhe }}
              </div>
            </div>
            <div class="row-label">{{ row[0]?.hang }}</div>
          </div>
        </div>
        <div v-else class="text-center p-4">Phòng chiếu này chưa có thiết lập sơ đồ ghế. Hãy tạo mới bên trên.</div>
      </div>
      <div v-else class="seat-map-builder glass-panel text-center" style="display:flex;align-items:center;justify-content:center;color:#888;">
        Vui lòng chọn hoặc tạo một phòng chiếu ở cột bên trái.
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import api from '../../api/axios';

const rooms = ref([]);
const selectedRoom = ref(null);
const genRows = ref(8);
const genCols = ref(10);

const loadRooms = async () => {
  try {
    const res = await api.get('/phong-chieu');
    if (res.success) {
      rooms.value = res.data;
    }
  } catch(e) { console.error(e); }
};

const selectRoom = async (id) => {
  try {
    const res = await api.get(`/phong-chieu/${id}`);
    if (res.success) {
      selectedRoom.value = res.data;
    }
  } catch(e) { console.error(e); }
};

const createRoom = async () => {
  const name = prompt('Nhập tên phòng chiếu (VD: Phòng 1, IMAX 3D):');
  if (!name) return;
  try {
    const res = await api.post('/phong-chieu', { tenPhong: name });
    if (res.success) {
      await loadRooms();
      selectRoom(res.data.maPhong);
    }
  } catch(e) { alert('Lỗi: ' + e.message); }
};

const deleteRoom = async (id) => {
  if (!confirm('Cảnh báo! Xóa phòng chiếu sẽ xóa toàn bộ ghế và có thể ảnh hưởng đến lịch chiếu. Tiếp tục?')) return;
  try {
    const res = await api.delete(`/phong-chieu/${id}`);
    if (res.success) {
      selectedRoom.value = null;
      loadRooms();
    }
  } catch(e) { alert('Lỗi: ' + e.message); }
};

const generateSeats = async () => {
  if (!confirm(`Hành động này sẽ XÓA SẠCH sơ đồ ghế cũ và tạo lại ${genRows.value} hàng x ${genCols.value} cột. Bạn chắc chứ?`)) return;
  try {
    const payload = { soHang: genRows.value, soGheMotHang: genCols.value };
    const res = await api.post(`/phong-chieu/${selectedRoom.value.maPhong}/ghe/generate`, payload);
    if (res.success) {
      selectRoom(selectedRoom.value.maPhong); // Reload
    }
  } catch(e) { alert('Lỗi: ' + e.message); }
};

const toggleSeatType = async (seat) => {
  const newType = seat.loaiGhe === 'VIP' ? 'Thuong' : 'VIP';
  try {
    const res = await api.put(`/phong-chieu/ghe/${seat.maGhe}`, { loaiGhe: newType });
    if (res.success) {
      seat.loaiGhe = newType; // Update local immediately
    }
  } catch(e) { alert('Không thể cập nhật loại ghế'); }
};

// Format flat array of ghes into a matrix (rows)
const seatMatrix = computed(() => {
  if (!selectedRoom.value || !selectedRoom.value.ghes) return [];
  const ghes = selectedRoom.value.ghes;
  const matrix = [];
  const rowMap = {};
  
  ghes.forEach(g => {
    if (!rowMap[g.hang]) rowMap[g.hang] = [];
    rowMap[g.hang].push(g);
  });
  
  Object.keys(rowMap).sort().forEach(hang => {
    // sort seats inside row by number
    rowMap[hang].sort((a,b) => a.soGhe - b.soGhe);
    matrix.push(rowMap[hang]);
  });
  
  return matrix;
});

onMounted(() => {
  loadRooms();
});
</script>

<style scoped>
.page-title { font-size: 1.8rem; font-weight: 700; margin-bottom: 0; }
.header-actions { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; }

.split-layout {
  display: flex; gap: 1.5rem;
  align-items: flex-start;
}
.room-list {
  width: 280px; padding: 1.5rem;
  border-radius: var(--radius-lg);
  background: var(--color-bg-card);
  flex-shrink: 0;
}
.panel-title { font-size: 1.2rem; font-weight: 700; margin-bottom: 1rem; color: white; border-bottom: 1px solid rgba(255,255,255,0.1); padding-bottom: 0.5rem;}
.rooms { list-style: none; padding: 0; margin: 0; }
.rooms li {
  padding: 0.8rem 1rem;
  margin-bottom: 0.5rem;
  border-radius: 6px;
  background: rgba(255,255,255,0.02);
  cursor: pointer;
  transition: 0.2s;
  font-weight: 500;
}
.rooms li:hover { background: rgba(255,255,255,0.08); }
.rooms li.active { background: var(--color-primary); color: white; }
.empty-list { color: #888; text-align: center; padding: 1rem 0; font-size: 0.9rem;}

.seat-map-builder {
  flex: 1; padding: 2rem;
  border-radius: var(--radius-lg);
  background: var(--color-bg-card);
  min-height: 500px;
}
.builder-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem;}
.builder-controls { display: flex; gap: 1rem; background: rgba(0,0,0,0.3); padding: 1.5rem; border-radius: 8px; margin-bottom: 1.5rem;}
.flex-1 { flex: 1; }
.mt-4 { margin-top: 1.5rem; }

/* SEAT MAP UI */
.seat-instruction { background: rgba(255, 255, 255, 0.03); padding: 1rem; border-radius: 6px; margin-bottom: 2rem; }
.legend { display: flex; gap: 1rem; align-items: center; justify-content: center;}
.seat-legend { width: 24px; height: 24px; border-radius: 4px; }
.seat-legend.thuong { background: #34495e; }
.seat-legend.vip { background: #e84393; }
.mt-2 { margin-top: 0.5rem; }

.seat-grid-container {
  overflow-x: auto;
  text-align: center;
}
.screen {
  background: linear-gradient(to bottom, #ddd, transparent);
  height: 40px; margin: 0 auto 3rem; max-width: 500px;
  border-radius: 5px; color: #111; font-weight: bold; line-height: 40px; letter-spacing: 2px;
}
.seat-row {
  display: flex; align-items: center; justify-content: center; margin-bottom: 0.6rem; gap: 1rem;
}
.row-label { font-weight: bold; color: #888; width: 30px; }
.seats { display: flex; gap: 0.6rem; }
.seat {
  width: 32px; height: 32px;
  border-radius: 6px 6px 4px 4px;
  position: relative;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.75rem; font-weight: bold; color: white;
  cursor: pointer;
  transition: transform 0.2s, background 0.2s;
  box-shadow: 0 4px 0 rgba(0,0,0,0.3);
}
.seat:active { transform: translateY(4px); box-shadow: none; }
.seat-thuong { background: #34495e; border: 1px solid #2c3e50; }
.seat-vip { background: #e84393; border: 1px solid #d63031; }
.seat:hover { filter: brightness(1.2); }

.p-4 { padding: 1.5rem; }
.text-center { text-align: center; }
</style>
