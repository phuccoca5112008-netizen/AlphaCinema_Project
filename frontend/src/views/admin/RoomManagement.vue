<template>
  <div class="room-management">
    <div class="header-actions">
      <div>
        <h1 class="gradient-text">Quản Lý Phòng Chiếu & Sơ Đồ Ghế</h1>
        <p class="text-muted">Phân loại phòng theo định dạng và thiết lập không gian rạp chiếu chuyên nghiệp.</p>
      </div>
      <button class="btn btn-primary" @click="isAddingRoom = true">
        <i class="fas fa-plus-circle"></i> Thêm Phòng Mới
      </button>
    </div>

    <!-- Format Navigation Tabs -->
    <div class="format-tabs mb-4">
      <div 
        v-for="fmt in formats" 
        :key="fmt.id" 
        class="format-tab-card glass-panel"
        :class="['format-' + fmt.id.toLowerCase(), { active: selectedFormat === fmt.id }]"
        @click="selectedFormat = fmt.id"
      >
        <div class="fmt-icon">
          <i :class="fmt.icon"></i>
        </div>
        <div class="fmt-info">
          <h3>{{ fmt.name }}</h3>
          <span class="price-tag">{{ fmt.priceDesc }}</span>
        </div>
        <div class="room-count">
          {{ getRoomCount(fmt.id) }} Phòng
        </div>
      </div>
    </div>

    <!-- Inline Add Room Form -->
    <div v-if="isAddingRoom" class="add-room-inline glass-panel mb-4">
      <div class="d-flex align-center gap-4 wrap">
        <div class="form-group m-0 flex-1 min-w-150">
          <label class="form-label mini">Định dạng mặc định</label>
          <select v-model="newRoomType" class="form-input">
            <option value="IMAX">IMAX Theater (Cao nhất)</option>
            <option value="3D">3D Hybrid (Trung bình)</option>
            <option value="2D">2D Standard (Cơ bản)</option>
          </select>
        </div>
        <div class="form-group m-0 flex-2 min-w-200">
          <label class="form-label mini">Tên Phòng (Tự động gợi ý)</label>
          <div class="d-flex gap-2">
            <input 
              type="text" 
              v-model="newRoomName" 
              class="form-input" 
              :placeholder="'VD: ' + getSuggestedName(newRoomType)"
              @keyup.enter="handleCreateRoom"
            />
          </div>
        </div>
        <div class="d-flex gap-2">
           <button class="btn btn-primary" @click="handleCreateRoom" :disabled="!newRoomName">Tạo phòng</button>
           <button class="btn btn-outline" @click="isAddingRoom = false">Hủy</button>
        </div>
      </div>
    </div>

    <div class="split-layout">
      <!-- Cột Trái: Danh Sách Phòng Theo Định Dạng -->
      <aside class="room-sidebar">
        <div class="sidebar-header">
          <i class="fas fa-list-ul"></i> Danh Sách {{ selectedFormat }}
        </div>
        
        <div class="room-list-scroll">
          <div 
            v-for="r in filteredRooms" 
            :key="r.maPhong" 
            class="room-card glass-panel"
            :class="{ active: r.maPhong === selectedRoom?.maPhong }"
            @click="selectRoom(r.maPhong)"
          >
            <div class="room-mini-icon">
              <i class="fas fa-door-open"></i>
            </div>
            <div class="room-info">
              <h4>{{ r.tenPhong }}</h4>
              <span class="room-id">Sức chứa: {{ r.ghes?.length || 0 }} ghế</span>
            </div>
          </div>
          
          <div v-if="filteredRooms.length === 0" class="empty-rooms">
            <i class="fas fa-ghost"></i>
            <p>Trống mục này</p>
          </div>
        </div>
      </aside>

      <!-- Cột Phải: Trình Thiết Kế Sơ Đồ -->
      <main class="builder-area">
        <div v-if="selectedRoom" class="builder-container glass-panel" :class="'theme-' + selectedFormat.toLowerCase()">
          <div class="builder-header">
            <div class="room-identity">
              <span class="format-label">{{ selectedFormat }}</span>
              <div v-if="!isEditingName" class="d-flex align-center gap-3">
                <h2>{{ selectedRoom.tenPhong }}</h2>
                <button class="btn-icon" @click="startEditName"><i class="fas fa-edit"></i></button>
              </div>
              <div v-else class="d-flex align-center gap-2">
                <input v-model="editingName" class="form-input h2-style" @keyup.enter="saveRoomName" />
                <button class="btn btn-primary btn-sm" @click="saveRoomName">Lưu</button>
                <button class="btn btn-outline btn-sm" @click="isEditingName = false">Hủy</button>
              </div>
            </div>
            
            <div class="builder-actions">
              <button class="btn btn-outline-error" @click="deleteRoom(selectedRoom.maPhong)">
                <i class="fas fa-trash-alt"></i> Xóa phòng
              </button>
            </div>
          </div>

          <!-- Configuration & Stats Section -->
          <div class="config-stats-grid mb-5">
            <div class="config-panel glass-panel">
               <h4><i class="fas fa-tools"></i> Cấu hình hàng & ghế</h4>
               <div class="d-flex gap-3 mt-3">
                  <div class="stepper-box">
                    <span>Hàng</span>
                    <div class="stepper">
                      <button @click="genRows > 1 && genRows--">-</button>
                      <input type="number" v-model="genRows" />
                      <button @click="genRows < 25 && genRows++">+</button>
                    </div>
                  </div>
                  <div class="stepper-box">
                    <span>Cột</span>
                    <div class="stepper">
                      <button @click="genCols > 1 && genCols--">-</button>
                      <input type="number" v-model="genCols" />
                      <button @click="genCols < 30 && genCols++">+</button>
                    </div>
                  </div>
                  <button class="btn btn-primary flex-1" @click="generateSeats">
                    Tạo mới
                  </button>
               </div>
            </div>

            <div class="stats-panel glass-panel">
              <div class="stat-bubble">
                <span class="num">{{ selectedRoom.ghes?.length || 0 }}</span>
                <span class="lbl">Tổng ghế</span>
              </div>
              <div class="stat-bubble vip">
                <span class="num">{{ vipCount }}</span>
                <span class="lbl">Ghế VIP</span>
              </div>
              <div class="stat-bubble std">
                <span class="num">{{ standardCount }}</span>
                <span class="lbl">Thường</span>
              </div>
            </div>
          </div>

          <!-- Visualization Section -->
          <div class="cinema-view">
            <div class="screen-container">
              <div class="screen-glow"></div>
              <div class="screen-label">MÀN HÌNH {{ selectedFormat }}</div>
            </div>

            <div class="seat-grid-scroller" v-if="seatMatrix.length > 0">
              <div class="seat-grid">
                <div v-for="(row, index) in seatMatrix" :key="index" class="seat-row">
                  <div class="row-label">{{ row[0]?.hang }}</div>
                  <div class="seats-row-list">
                    <div 
                      v-for="seat in row" 
                      :key="seat.maGhe" 
                      class="seat-item" 
                      :class="{ 
                        'is-vip': seat.loaiGhe === 'VIP', 
                        'is-normal': seat.loaiGhe === 'Thuong' 
                      }"
                      @click="toggleSeatType(seat)"
                    >
                      <span class="seat-num">{{ seat.soGhe }}</span>
                    </div>
                  </div>
                  <div class="row-label">{{ row[0]?.hang }}</div>
                </div>
              </div>
            </div>
            
            <div v-else class="empty-layout">
              <div class="empty-icon"><i class="fas fa-th"></i></div>
              <p>Chưa có sơ đồ ghế. Hãy tạo mới ở phía trên.</p>
            </div>

            <div class="legend-area mt-4">
              <div class="legend-item"><span class="box std"></span> Ghế Thường</div>
              <div class="legend-item"><span class="box vip"></span> Ghế VIP</div>
              <div class="legend-hint"><i class="fas fa-mouse-pointer"></i> Nhấn vào ghế để thay đổi loại</div>
            </div>
          </div>
        </div>
        
        <div v-else class="no-selection glass-panel">
          <div class="no-selection-content">
            <div class="hero-icon">
              <i v-if="selectedFormat === 'IMAX'" class="fas fa-layer-group"></i>
              <i v-else-if="selectedFormat === '3D'" class="fas fa-cube"></i>
              <i v-else class="fas fa-film"></i>
            </div>
            <h2>{{ selectedFormat }} Cinema Room</h2>
            <p>Chọn một phòng {{ selectedFormat }} từ danh sách hoặc tạo mới để thiết lập màn hình và ghế ngồi.</p>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue';
import { adminApi } from '../../api/adminApi';

const rooms = ref([]);
const selectedRoom = ref(null);
const selectedFormat = ref('IMAX');
const genRows = ref(8);
const genCols = ref(12);
const isAddingRoom = ref(false);
const newRoomName = ref('');
const newRoomType = ref('IMAX');
const isEditingName = ref(false);
const editingName = ref('');

const formats = [
  { id: 'IMAX', name: 'Premium IMAX', icon: 'fas fa-clone', priceDesc: 'Giá vé cao nhất' },
  { id: '3D', name: 'Digital 3D', icon: 'fas fa-cube', priceDesc: 'Giá vé trung bình' },
  { id: '2D', name: 'Cinema 2D', icon: 'fas fa-film', priceDesc: 'Giá vé cơ bản' }
];

const filteredRooms = computed(() => {
  return rooms.value.filter(r => (r.loaiPhong || '2D') === selectedFormat.value);
});

const getRoomCount = (format) => {
  return rooms.value.filter(r => (r.loaiPhong || '2D') === format).length;
};

const getSuggestedName = (format) => {
  const count = rooms.value.filter(r => (r.loaiPhong || '2D') === format).length + 1;
  if (format === 'IMAX') return `IMAX Premium ${count.toString().padStart(2, '0')}`;
  if (format === '3D') return `Digital 3D - Room ${count}`;
  return `Cinema 2D - Room ${count}`;
};

const vipCount = computed(() => selectedRoom.value?.ghes?.filter(g => g.loaiGhe === 'VIP').length || 0);
const standardCount = computed(() => selectedRoom.value?.ghes?.filter(g => g.loaiGhe === 'Thuong').length || 0);

const loadRooms = async () => {
  try {
    const res = await adminApi.getRooms();
    if (res.success) {
      rooms.value = res.data;
    }
  } catch(e) { console.error(e); }
};

const selectRoom = async (id) => {
  try {
    const res = await adminApi.getRoomDetail(id);
    if (res.success) {
      selectedRoom.value = res.data;
      isEditingName.value = false;
    }
  } catch(e) { console.error(e); }
};

const startEditName = () => {
  editingName.value = selectedRoom.value.tenPhong;
  isEditingName.value = true;
};

const saveRoomName = async () => {
  if (!editingName.value) return;
  try {
    const res = await adminApi.updateRoom(selectedRoom.value.maPhong, { 
      tenPhong: editingName.value,
      loaiPhong: selectedRoom.value.loaiPhong 
    });
    if (res.success) {
      selectedRoom.value.tenPhong = editingName.value;
      isEditingName.value = false;
      loadRooms();
    }
  } catch(e) { alert('Lỗi: ' + e.message); }
};

const handleCreateRoom = async () => {
  if (!newRoomName.value) return;
  try {
    const res = await adminApi.createRoom({ 
      tenPhong: newRoomName.value,
      loaiPhong: newRoomType.value 
    });
    if (res.success) {
      newRoomName.value = '';
      isAddingRoom.value = false;
      selectedFormat.value = newRoomType.value;
      await loadRooms();
      selectRoom(res.data.maPhong);
    }
  } catch(e) { alert('Lỗi: ' + e.message); }
};

const deleteRoom = async (id) => {
  if (!confirm('Bạn có chắc muốn xóa phòng chiếu này?')) return;
  try {
    const res = await adminApi.deleteRoom(id);
    if (res.success) {
      selectedRoom.value = null;
      loadRooms();
    }
  } catch(e) { alert('Lỗi: ' + e.message); }
};

const generateSeats = async () => {
  try {
    const payload = { soHang: genRows.value, soGheMotHang: genCols.value };
    const res = await adminApi.generateSeats(selectedRoom.value.maPhong, payload);
    if (res.success) {
      await selectRoom(selectedRoom.value.maPhong);
    }
  } catch(e) { 
    console.error('Lỗi tạo ghế:', e);
    alert('Lỗi: ' + (e.response?.data?.message || e.message)); 
  }
};

const toggleSeatType = async (seat) => {
  const newType = seat.loaiGhe === 'VIP' ? 'Thuong' : 'VIP';
  try {
    const res = await adminApi.updateSeat(seat.maGhe, { loaiGhe: newType });
    if (res.success) {
      seat.loaiGhe = newType;
    }
  } catch(e) { console.error('Lỗi đổi loại ghế:', e); }
};

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
    rowMap[hang].sort((a,b) => a.soGhe - b.soGhe);
    matrix.push(rowMap[hang]);
  });
  
  return matrix;
});

// Auto-reset selection when format changes
watch(selectedFormat, () => {
  selectedRoom.value = null;
  newRoomType.value = selectedFormat.value;
  newRoomName.value = getSuggestedName(selectedFormat.value);
});

watch(newRoomType, (newVal) => {
  newRoomName.value = getSuggestedName(newVal);
});

watch(isAddingRoom, (newVal) => {
  if (newVal) {
    newRoomName.value = getSuggestedName(newRoomType.value);
  }
});

onMounted(() => {
  loadRooms();
});
</script>

<style scoped>
.room-management { padding: 1.5rem; max-width: 1600px; margin: 0 auto; }
.header-actions { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 2.5rem; }

/* Format Tabs */
.format-tabs { display: grid; grid-template-columns: repeat(3, 1fr); gap: 1.5rem; }
.format-tab-card {
  padding: 1.5rem; display: flex; align-items: center; gap: 1.25rem;
  cursor: pointer; transition: 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative; border: 1px solid rgba(255,255,255,0.05);
}
.format-tab-card:hover { transform: translateY(-5px); border-color: rgba(255,255,255,0.15); }
.format-tab-card.active { background: rgba(255,255,255,0.08); border-color: var(--color-primary); }

.fmt-icon {
  width: 50px; height: 50px; border-radius: 14px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.5rem; background: rgba(0,0,0,0.3); transition: 0.3s;
}
.format-tab-card.active .fmt-icon { background: var(--color-primary); color: white; transform: rotate(10deg); }

.format-tab-card.format-imax.active { border-color: #00cec9; box-shadow: 0 10px 30px rgba(0, 206, 201, 0.15); }
.format-tab-card.format-imax.active .fmt-icon { background: #00cec9; }

.format-tab-card.format-3d.active { border-color: #a29bfe; box-shadow: 0 10px 30px rgba(162, 155, 254, 0.15); }
.format-tab-card.format-3d.active .fmt-icon { background: #a29bfe; }

.fmt-info h3 { font-size: 1.1rem; margin: 0; font-weight: 800; }
.price-tag { font-size: 0.75rem; color: var(--color-text-sub); opacity: 0.7; }
.room-count { margin-left: auto; font-size: 0.85rem; font-weight: 700; color: var(--color-primary); }

/* Sidebar */
.split-layout { display: flex; gap: 2rem; }
.room-sidebar { width: 280px; flex-shrink: 0; }
.sidebar-header { 
  display: flex; align-items: center; gap: 0.8rem;
  font-size: 0.9rem; font-weight: 800; color: var(--color-text-sub);
  margin-bottom: 1.25rem; padding-left: 0.5rem;
  text-transform: uppercase; letter-spacing: 1px;
}
.room-list-scroll { display: flex; flex-direction: column; gap: 0.8rem; max-height: 800px; overflow-y: auto; }

.room-card {
  padding: 1rem 1.25rem; display: flex; align-items: center; gap: 1rem;
  cursor: pointer; transition: 0.3s;
}
.room-card:hover { transform: scale(1.02); background: rgba(255,255,255,0.05); }
.room-card.active { border-left: 4px solid var(--color-primary); background: rgba(255,255,255,0.08); }

.room-mini-icon { color: var(--color-text-muted); font-size: 1.2rem; }
.room-card.active .room-mini-icon { color: var(--color-primary); }
.room-info h4 { margin: 0; font-size: 0.95rem; }
.room-id { font-size: 0.75rem; color: var(--color-text-muted); }

.empty-rooms { text-align: center; padding: 3rem 1rem; color: var(--color-text-muted); border: 2px dashed rgba(255,255,255,0.05); border-radius: 12px; }
.empty-rooms i { font-size: 2rem; margin-bottom: 1rem; opacity: 0.2; }

/* Builder Area */
.builder-area { flex: 1; min-height: 800px; }
.builder-container { padding: 3rem; border-radius: 24px; position: relative; overflow: hidden; }
.builder-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 3rem; }

.room-identity .format-label {
  display: inline-block; padding: 4px 12px; border-radius: 20px;
  background: var(--color-primary); color: white; font-size: 0.7rem;
  font-weight: 900; letter-spacing: 1px; margin-bottom: 0.8rem;
}
.room-identity h2 { font-size: 2.2rem; margin: 0; font-weight: 900; }
.h2-style { font-size: 1.8rem; font-weight: 900; background: rgba(255,255,255,0.05); border: 1px solid rgba(255,255,255,0.1); padding: 5px 15px; border-radius: 12px; height: auto; }
.btn-icon { background: none; border: none; color: var(--color-primary); cursor: pointer; font-size: 1.2rem; opacity: 0.6; transition: 0.3s; }
.btn-icon:hover { opacity: 1; transform: scale(1.1); }
.btn-sm { padding: 4px 12px; font-size: 0.8rem; }

/* Themes */
.theme-imax .format-label { background: #00cec9; }
.theme-imax h2 { filter: drop-shadow(0 0 10px rgba(0, 206, 201, 0.3)); }
.theme-3d .format-label { background: #a29bfe; }
.theme-3d h2 { filter: drop-shadow(0 0 10px rgba(162, 155, 254, 0.3)); }

.config-stats-grid { display: grid; grid-template-columns: 1.5fr 1fr; gap: 2rem; }
.config-panel h4 { margin: 0; font-size: 1rem; font-weight: 800; color: var(--color-text-sub); }
.stepper-box { display: flex; flex-direction: column; gap: 5px; }
.stepper-box span { font-size: 0.7rem; font-weight: 700; color: var(--color-text-muted); }

.stepper { display: flex; align-items: center; background: rgba(0,0,0,0.3); border-radius: 10px; border: 1px solid rgba(255,255,255,0.1); }
.stepper button { 
  background: none; border: none; color: white; width: 34px; height: 34px; 
  cursor: pointer; transition: 0.2s; 
}
.stepper input { 
  background: none; border: none; width: 40px; text-align: center; color: white; 
  font-weight: 700; -moz-appearance: textfield; appearance: none;
}

.stats-panel { display: flex; justify-content: space-around; align-items: center; padding: 1rem; }
.stat-bubble { text-align: center; }
.stat-bubble .num { display: block; font-size: 1.5rem; font-weight: 900; }
.stat-bubble .lbl { font-size: 0.7rem; font-weight: 800; color: var(--color-text-muted); text-transform: uppercase; }
.stat-bubble.vip .num { color: #e84393; }

/* Cinema Layout */
.screen-container { margin: 4rem auto; max-width: 800px; position: relative; }
.screen-glow {
  height: 20px; width: 100%; border-radius: 50% / 100% 100% 0 0;
  background: linear-gradient(to bottom, rgba(255,255,255,0.8), transparent);
  box-shadow: 0 -15px 40px rgba(255,255,255,0.3);
}
.theme-imax .screen-glow { background: linear-gradient(to bottom, #00cec9, transparent); box-shadow: 0 -15px 40px rgba(0, 206, 201, 0.4); }
.theme-3d .screen-glow { background: linear-gradient(to bottom, #a29bfe, transparent); box-shadow: 0 -15px 40px rgba(162, 155, 254, 0.4); }

.screen-label { text-align: center; margin-top: 10px; font-size: 0.75rem; letter-spacing: 6px; font-weight: 800; color: var(--color-text-muted); }

.seat-grid-scroller { overflow-x: auto; padding: 4rem 0; background: radial-gradient(circle at center, rgba(255,255,255,0.03), transparent); }
.seat-grid { display: flex; flex-direction: column; gap: 1rem; align-items: center; min-width: max-content; }
.seat-row { display: flex; align-items: center; gap: 2.5rem; }
.seats-row-list { display: flex; gap: 0.8rem; }

.seat-item {
  width: 34px; height: 34px; border-radius: 50%;
  display: flex; align-items: center; justify-content: center;
  cursor: pointer; transition: 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  font-size: 0.65rem; font-weight: 900;
}
.is-normal { background: rgba(255,255,255,0.1); border: 2px solid rgba(255,255,255,0.2); }
.is-vip { background: #e84393; border: 2px solid #fd79a8; box-shadow: 0 0 15px rgba(232, 67, 147, 0.4); }
.seat-item:hover { transform: scale(1.2) rotate(5deg); z-index: 10; }

.legend-area { display: flex; align-items: center; gap: 2rem; padding: 1.5rem; background: rgba(0,0,0,0.2); border-radius: 12px; }
.legend-item { display: flex; align-items: center; gap: 8px; font-size: 0.85rem; font-weight: 700; }
.box { width: 14px; height: 14px; border-radius: 4px; }
.box.std { background: rgba(255,255,255,0.2); }
.box.vip { background: #e84393; }
.legend-hint { margin-left: auto; font-size: 0.8rem; color: var(--color-primary); font-style: italic; }

.no-selection {
  height: 600px; display: flex; align-items: center; justify-content: center; text-align: center;
}
.hero-icon { font-size: 5rem; margin-bottom: 2rem; color: var(--color-primary); opacity: 0.5; }
.no-selection p { color: var(--color-text-muted); max-width: 400px; line-height: 1.6; }

.btn-outline-error { border: 2px solid var(--color-error); color: var(--color-error); background: transparent; padding: 8px 16px; border-radius: 8px; cursor: pointer; transition: 0.3s; }
.btn-outline-error:hover { background: var(--color-error); color: white; box-shadow: 0 0 20px rgba(255, 71, 87, 0.3); }

/* Responsive Wrap */
.wrap { flex-wrap: wrap; }
.min-w-200 { min-width: 200px; }
.min-w-150 { min-width: 150px; }

input::-webkit-outer-spin-button,
input::-webkit-inner-spin-button { -webkit-appearance: none; margin: 0; }
</style>
