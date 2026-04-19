<template>
  <div class="showtime-management">
    <div class="header-actions">
      <div>
        <h1 class="gradient-text"><i class="fas fa-clock"></i> Quản Lý Suất Chiếu</h1>
        <p class="text-muted">Lập lịch và điều hành hoạt động rạp phim Alpha Cinema.</p>
      </div>
      <button class="btn btn-primary shadow-lg" @click="handleOpenModal()">
        <i class="fas fa-plus"></i> Thêm Suất Chiếu
      </button>
    </div>

    <!-- Filter Bar - Premium Glassmorphism -->
    <div class="glass-panel filter-toolbar mb-4">
      <div class="filter-group date-filters">
        <label class="form-label mb-2"><i class="far fa-calendar-alt"></i> Chọn ngày chiếu:</label>
        <div class="date-chips">
          <button 
            v-for="d in dateShortcuts" :key="d.date"
            class="chip" :class="{ active: selectedDate === d.date }"
            @click="selectDate(d.date)"
          >
            <span class="day-name">{{ d.dayLabel }}</span>
            <span class="day-num">{{ d.numLabel }}</span>
          </button>
          <div class="date-picker-wrap">
            <input type="date" v-model="selectedDate" class="form-input date-input" @change="fetchShowtimes">
          </div>
        </div>
      </div>

      <div class="divider"></div>

      <div class="filter-group select-filters">
        <div class="select-item">
          <label class="form-label mb-2"><i class="fas fa-door-open"></i> Phòng:</label>
          <select v-model="filterRoom" class="form-input" @change="fetchShowtimes">
            <option value="">Tất cả phòng</option>
            <option v-for="r in rooms" :key="r.maPhong" :value="r.maPhong">{{ r.tenPhong }}</option>
          </select>
        </div>
        <div class="select-item">
          <label class="form-label mb-2"><i class="fas fa-film"></i> Phim:</label>
          <select v-model="filterPhim" class="form-input" @change="fetchShowtimes">
            <option value="">Tất cả phim</option>
            <option v-for="p in phims" :key="p.maPhim" :value="p.maPhim">{{ p.tenPhim }}</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Board Content -->
    <div class="content-body" v-if="!loading">
      <div v-if="filteredGroups.length > 0" class="room-grid">
        <div v-for="room in filteredGroups" :key="room.maPhong" class="room-column animate-in">
          <div class="room-header glass-panel">
            <div class="room-title">
              <span class="indicator"></span>
              <h3>{{ room.tenPhong }}</h3>
            </div>
            <span class="count-badge">{{ room.items.length }} suất</span>
          </div>

          <div class="showtime-cards">
            <div v-for="s in room.items" :key="s.maSuatChieu" class="st-card glass-panel" :class="getStatusClass(s)">
              <div class="st-status-bar"></div>
              
              <div class="st-main">
                <div class="st-time">
                  <div class="start-time">{{ formatTime(s.thoiGianBatDau) }}</div>
                  <div class="duration text-muted">{{ s.dinhDang }}</div>
                </div>
                
                <div class="st-info">
                  <h4 class="movie-title" :title="s.tenPhim">{{ s.tenPhim }}</h4>
                  <div class="price-info">{{ (s.giaVeGoc || 0).toLocaleString() }}đ</div>
                </div>

                <div class="st-occupancy">
                  <div class="occ-text">
                    <span><i class="fas fa-users"></i> {{ calculateOccupancy(s) }}%</span>
                    <span class="empty-seats">{{ s.soGheTrong }} trống</span>
                  </div>
                  <div class="occ-progress">
                    <div class="occ-fill" :style="{ width: calculateOccupancy(s) + '%' }"></div>
                  </div>
                </div>
              </div>

              <div class="st-actions">
                <button class="action-btn edit-btn" @click.stop="handleOpenModal(s)" title="Chỉnh sửa">
                  <i class="fas fa-pen"></i>
                </button>
                <button class="action-btn delete-btn" @click.stop="deleteShowtime(s.maSuatChieu)" title="Xóa">
                  <i class="fas fa-times"></i>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <div v-else class="empty-state glass-panel">
        <div class="empty-icon"><i class="fas fa-calendar-times"></i></div>
        <h3>Không tìm thấy lịch chiếu</h3>
        <p>Thử thay đổi ngày xem hoặc chỉnh lại bộ lọc phòng/phim.</p>
        <button class="btn btn-outline mt-3" @click="clearFilters">Cài lại bộ lọc</button>
      </div>
    </div>

    <!-- Loading screen -->
    <div v-else class="loading-overlay">
      <div class="spinner"></div>
      <p>Đang chuẩn bị lịch chiếu...</p>
    </div>

    <!-- Modal Form (Create/Edit) -->
    <teleport to="body">
      <transition name="modal-fade">
        <div class="modal-root" v-if="showModal" @click.self="showModal = false">
          <div class="modal-dialog glass-panel animate-modal">
            <div class="modal-header">
                <h2 class="modal-title">
                  <i class="fas" :class="isEdit ? 'fa-edit' : 'fa-plus-circle'"></i>
                  {{ isEdit ? 'Cập Nhật Suất Chiếu' : 'Tạo Suất Chiếu Mới' }}
                </h2>
              <button class="close-btn" @click="showModal = false">&times;</button>
            </div>

            <!-- Notification Row (Full Width) -->
            <div class="modal-notifications" v-if="modalError || modalSuccess">
                <div v-if="modalError" class="modal-alert modal-alert-error animate-shake">
                  <i class="fas fa-exclamation-triangle"></i>
                  <span>{{ modalError }}</span>
                  <button @click="modalError = ''" class="close-alert">&times;</button>
                </div>
                <div v-if="modalSuccess" class="modal-alert modal-alert-success animate-in">
                  <i class="fas fa-check-circle"></i>
                  <span>{{ modalSuccess }}</span>
                  <button @click="modalSuccess = ''" class="close-alert">&times;</button>
                </div>
            </div>
            
            <form @submit.prevent="saveShowtime" class="modal-body">
              <div class="form-row">
                <div class="form-group flex-1">
                  <label class="form-label">Phim</label>
                  <select v-model="formData.maPhim" class="form-input" required>
                    <option value="" disabled>Chọn phim...</option>
                    <option v-for="phim in dangChieuPhims" :key="phim.maPhim" :value="phim.maPhim">
                      {{ phim.tenPhim }} {{ phim.trangThaiPhim === 'Sắp chiếu' ? '(Sắp chiếu)' : '' }}
                    </option>
                  </select>
                </div>
                <div class="form-group flex-1">
                  <label class="form-label">Phòng</label>
                  <select v-model="formData.maPhong" class="form-input" required>
                    <option value="" disabled>Chọn phòng...</option>
                    <option v-for="r in rooms" :key="r.maPhong" :value="r.maPhong">{{ r.tenPhong }}</option>
                  </select>
                </div>
              </div>
              
              <div class="form-row mt-4">
                <div class="form-group flex-1">
                  <label class="form-label">Ngày Chiếu</label>
                  <input type="date" v-model="formData.datePart" class="form-input" required>
                </div>
                <div class="form-group" style="width: 140px;">
                  <label class="form-label">Giở Chiếu (24h)</label>
                  <select v-model="formData.timePart" class="form-input" required>
                    <option v-for="h in 24" :key="'h'+(h-1)" :value="padStr(h-1) + ':00'">{{ padStr(h-1) }}:00</option>
                    <option v-for="h in 24" :key="'m'+(h-1)" :value="padStr(h-1) + ':30'">{{ padStr(h-1) }}:30</option>
                  </select>
                </div>
                <div class="form-group" style="width: 140px;">
                  <label class="form-label">Định Dạng</label>
                  <select v-model="formData.dinhDang" class="form-input">
                    <option value="2D">2D</option>
                    <option value="3D">3D</option>
                    <option value="IMAX">IMAX</option>
                  </select>
                </div>
              </div>
              
              <!-- NEW: Dynamic End Time Preview -->
              <div v-if="formData.maPhim && formData.timePart" class="end-time-preview animate-in">
                <div class="preview-item">
                  <i class="fas fa-clock"></i>
                  <span>Dự kiến kết thúc: <strong>{{ calculatedEndTime }}</strong></span>
                </div>
                <div class="preview-note">
                  (Đã bao gồm thời lượng phim + 15 phút dọn phòng)
                </div>
              </div>
              
              <div class="form-row mt-4">
                <div class="form-group flex-1">
                  <label class="form-label">Giá Vé (VNĐ)</label>
                  <div class="price-input-group">
                    <input type="number" v-model="formData.giaVeGoc" class="form-input" required min="10000" step="1000">
                    <span class="unit-text">đ</span>
                  </div>
                </div>
              </div>

              <div class="alert-box mt-4" v-if="isEdit">
                <i class="fas fa-info-circle"></i>
                Lưu ý: Thay đổi thông tin sẽ áp dụng ngay cho các giao dịch mới.
              </div>

              <div class="modal-footer">
                <button type="button" class="btn btn-outline" @click="showModal = false">Bỏ qua</button>
                <button type="submit" class="btn btn-primary" :disabled="saving">
                  <i class="fas fa-check-circle" v-if="!saving"></i>
                  {{ saving ? 'Đang thực hiện...' : (isEdit ? 'Lưu Kết Quả' : 'Tạo Suất Chiếu') }}
                </button>
              </div>
            </form>
          </div>
        </div>
      </transition>
    </teleport>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { movieApi } from '../../api/movieApi';
import { adminApi } from '../../api/adminApi';
import { bookingApi } from '../../api/bookingApi';
import { useToastStore } from '../../stores/toast';

// --- STORES ---
const toast = useToastStore();

// --- UTILS ---
const padStr = (n) => (n < 10 ? '0' + n : n);
const toISOStringForInput = (dateStr) => {
  if (!dateStr) return '';
  const d = new Date(dateStr);
  if (isNaN(d.getTime())) return '';
  const YYYY = d.getFullYear();
  const MM = padStr(d.getMonth() + 1);
  const DD = padStr(d.getDate());
  const hh = padStr(d.getHours());
  const mm = padStr(d.getMinutes());
  return `${YYYY}-${MM}-${DD}T${hh}:${mm}`;
};

// --- STATE ---
const showtimes = ref([]);
const phims = ref([]);
const rooms = ref([]);
const loading = ref(true);
const saving = ref(false);
const modalError = ref('');
const modalSuccess = ref('');

// Computed list of "Now Showing" movies for the dropdown
const dangChieuPhims = computed(() => {
  return phims.value.filter(p => {
    // Luôn hiện phim đang chọn nếu đang ở chế độ Edit (Trường hợp đặc biệt)
    if (isEdit.value && p.maPhim === formData.value.maPhim) return true;
    return p.trangThaiPhim === 'Đang chiếu';
  });
});

const filterPhim = ref('');
const filterRoom = ref('');
const selectedDate = ref(new Date().toISOString().split('T')[0]);

const showModal = ref(false);
const isEdit = ref(false);
const formData = ref({});

// --- COMPUTED: Calculate End Time Preview ---
const calculatedEndTime = computed(() => {
  if (!formData.value.maPhim || !formData.value.timePart) return '';
  
  const movie = phims.value.find(p => p.maPhim === formData.value.maPhim);
  if (!movie) return '';

  const [hours, minutes] = formData.value.timePart.split(':').map(Number);
  const totalMinutes = hours * 60 + minutes + movie.thoiLuong + 15; // Phim + 15p dọn dẹp
  
  const endHours = Math.floor(totalMinutes / 60) % 24;
  const endMins = totalMinutes % 60;
  
  return `${endHours.toString().padStart(2, '0')}:${endMins.toString().padStart(2, '0')}`;
});

// --- LIFE CYCLE ---
onMounted(async () => {
  try {
    await Promise.all([fetchPhims(), fetchRooms(), fetchShowtimes()]);
  } catch (e) {
    console.error('App init failed:', e);
  } finally {
    loading.value = false;
  }
});

// --- API ACTIONS ---
async function fetchPhims() {
  const res = await movieApi.getMovies();
  if (res.success) phims.value = res.data;
}

async function fetchRooms() {
  const res = await adminApi.getRooms();
  if (res.success) rooms.value = res.data;
}

async function fetchShowtimes() {
  loading.value = true;
  try {
    const res = await bookingApi.getShowtimes({
      maPhim: filterPhim.value,
      ngay: selectedDate.value
    });
    if (res.success) showtimes.value = res.data;
  } finally {
    loading.value = false;
  }
}

// --- LOGIC ---
const dateShortcuts = computed(() => {
  const out = [];
  const start = new Date();
  for (let i = 0; i < 7; i++) {
    const d = new Date(start);
    d.setDate(start.getDate() + i);
    const iso = d.toISOString().split('T')[0];
    out.push({
      date: iso,
      dayLabel: i === 0 ? 'Nay' : d.toLocaleDateString('vi-VN', { weekday: 'short' }),
      numLabel: `${d.getDate()}/${d.getMonth() + 1}`
    });
  }
  return out;
});

const selectDate = (date) => {
  selectedDate.value = date;
  fetchShowtimes();
};

const clearFilters = () => {
  filterPhim.value = '';
  filterRoom.value = '';
  selectedDate.value = new Date().toISOString().split('T')[0];
  fetchShowtimes();
};

const filteredGroups = computed(() => {
  const list = [...showtimes.value];
  if (filterRoom.value) {
    // Single room filter
    const matched = list.filter(s => s.maPhong == filterRoom.value);
    const r = rooms.value.find(rm => rm.maPhong == filterRoom.value);
    return [{
      maPhong: filterRoom.value,
      tenPhong: r ? r.tenPhong : 'Phòng Đã Chọn',
      items: matched.sort((a,b) => new Date(a.thoiGianBatDau) - new Date(b.thoiGianBatDau))
    }];
  }

  // All rooms with content
  const groups = {};
  rooms.value.forEach(r => {
    const matched = list.filter(s => s.maPhong === r.maPhong);
    if (matched.length > 0) {
      groups[r.maPhong] = {
        maPhong: r.maPhong,
        tenPhong: r.tenPhong,
        items: matched.sort((a,b) => new Date(a.thoiGianBatDau) - new Date(b.thoiGianBatDau))
      };
    }
  });
  return Object.values(groups).sort((a,b) => (a.tenPhong || '').localeCompare(b.tenPhong || ''));
});

// --- MODAL & ACTIONS ---
const handleOpenModal = (item = null) => {
  modalError.value = ''; // Reset errors
  modalSuccess.value = '';
  if (item) {
    const d = new Date(item.thoiGianBatDau);
    isEdit.value = true;
    formData.value = {
      maSuatChieu: item.maSuatChieu,
      maPhim: item.maPhim,
      maPhong: item.maPhong,
      datePart: item.thoiGianBatDau.split('T')[0],
      timePart: padStr(d.getHours()) + ':' + padStr(d.getMinutes()),
      giaVeGoc: item.giaVeGoc,
      dinhDang: item.dinhDang
    };
  } else {
    isEdit.value = false;
    const nextDay = new Date();
    nextDay.setDate(nextDay.getDate() + 1);
    
    formData.value = { 
      maPhim: phims.value.length > 0 ? phims.value[0].maPhim : '', 
      maPhong: rooms.value.length > 0 ? rooms.value[0].maPhong : '', 
      datePart: nextDay.toISOString().split('T')[0],
      timePart: '19:00',
      giaVeGoc: 80000, 
      dinhDang: '2D' 
    };
  }
  showModal.value = true;
};

const saveShowtime = async () => {
  if (saving.value) return;
  saving.value = true;
  modalError.value = '';
  modalSuccess.value = '';
  
  try {
    const fullIso = `${formData.value.datePart}T${formData.value.timePart}:00`;
    const payload = { 
      ...formData.value, 
      thoiGianBatDau: fullIso
    };

    let res;
    if (isEdit.value) {
      res = await bookingApi.updateShowtime(formData.value.maSuatChieu, payload);
    } else {
      res = await bookingApi.createShowtime(payload);
    }

    if (res.success) {
      modalSuccess.value = isEdit.value ? 'Cập nhật suất chiếu thành công!' : 'Tạo suất chiếu mới thành công!';
      await fetchShowtimes();
      setTimeout(() => {
        showModal.value = false;
        modalSuccess.value = '';
      }, 1500);
    } else {
      modalError.value = res.message || "Có lỗi xảy ra khi lưu dữ liệu.";
    }
  } catch (e) {
    console.error("Save error:", e);
    modalError.value = e.response?.data?.message || e.message || "Lỗi cập nhật. Hãy kiểm tra lại tính hợp lệ của thời gian.";
  } finally {
    saving.value = false;
  }
};

const deleteShowtime = async (id) => {
  if (!confirm("Hành động này sẽ hủy suất chiếu và toàn bộ vé. Bạn chắc chắn?")) return;
  try {
    await bookingApi.deleteShowtime(id);
    toast.add('Đã xóa suất chiếu thành công!', 'success');
    await fetchShowtimes();
  } catch (e) {
    const errorMsg = e.response?.data?.message || "Không thể xóa suất chiếu này (đã có vé được đặt thành công).";
    toast.add(errorMsg, 'error');
  }
};

// --- DISPLAY ---
const formatTime = (iso) => {
  if (!iso) return '--:--';
  const d = new Date(iso);
  // Đảm bảo hiển thị chuẩn 24h (HH:mm)
  return d.getHours().toString().padStart(2, '0') + ':' + d.getMinutes().toString().padStart(2, '0');
};

const calculateOccupancy = (s) => {
  const total = (s.soGheTrong || 0) + (s.ves?.length || 0);
  if (total <= 0) return 0;
  return Math.round(((s.ves?.length || 0) / total) * 100);
};

const getStatusClass = (s) => {
  const now = new Date();
  const start = new Date(s.thoiGianBatDau);
  if (start > now) return 'st-upcoming';
  const end = new Date(start.getTime() + 120 * 60000);
  if (now < end) return 'st-running';
  return 'st-finished';
};

const getStatusLabel = (s) => {
  const st = getStatusClass(s);
  return st === 'st-upcoming' ? 'Sắp chiếu' : (st === 'st-running' ? 'Đang chiếu' : 'Đã xong');
};
</script>

<style scoped>
.showtime-management { padding: 1.5rem; max-width: 1700px; margin: 0 auto; color: #fff; }
.header-actions { display: flex; justify-content: space-between; align-items: center; margin-bottom: 2.5rem; }

/* Dashboard Toolbar */
.filter-toolbar { 
  display: flex; gap: 2rem; padding: 1.5rem 2rem; align-items: flex-end;
  border: 1px solid rgba(255,255,255,0.1); border-radius: 20px;
}
.divider { width: 1px; height: 50px; background: rgba(255,255,255,0.1); margin: 0 0.5rem; }
.select-filters { display: flex; gap: 1.5rem; flex: 1; }
.select-item { flex: 1; min-width: 150px; }

.date-chips { display: flex; gap: 0.75rem; align-items: center; }
.chip {
  background: rgba(255,255,255,0.05); border: 1px solid rgba(255,255,255,0.05); padding: 0.6rem 1rem;
  border-radius: 14px; cursor: pointer; transition: 0.3s;
  display: flex; flex-direction: column; align-items: center; min-width: 75px;
}
.chip:hover { border-color: var(--color-primary); background: rgba(255,255,255,0.1); }
.chip.active { background: var(--color-primary); color: #000; box-shadow: 0 4px 20px rgba(255,165,0,0.4); border: none; }
.day-name { font-size: 0.65rem; font-weight: 700; opacity: 0.8; }
.day-num { font-size: 1.1rem; font-weight: 900; }

.date-input { width: 160px; background: rgba(0,0,0,0.4); }

/* Room Boards */
.room-grid { display: flex; gap: 1.5rem; overflow-x: auto; padding-bottom: 2rem; min-height: 60vh; }
.room-column { min-width: 380px; flex: 1; display: flex; flex-direction: column; gap: 1.5rem; }

.room-header { 
  display: flex; justify-content: space-between; align-items: center; 
  padding: 1rem 1.25rem; border-radius: 15px; border: 1px solid rgba(255,255,255,0.05);
}
.room-title { display: flex; align-items: center; gap: 12px; }
.indicator { width: 6px; height: 6px; background: var(--color-primary); border-radius: 50%; box-shadow: 0 0 10px var(--color-primary); }
.room-header h3 { margin: 0; font-size: 1.2rem; font-weight: 800; }
.count-badge { font-size: 0.7rem; font-weight: 900; background: rgba(255,255,255,0.1); padding: 4px 10px; border-radius: 20px; }

/* Showtime Cards */
.st-card { 
  position: relative; display: flex; padding: 1.5rem; transition: 0.35s;
  border: 1px solid rgba(255,255,255,0.08); overflow: hidden;
}
.st-card:hover { transform: translateY(-5px); border-color: var(--color-primary); background: rgba(255,255,255,0.05); }

.st-status-bar { position: absolute; left: 0; top: 0; bottom: 0; width: 4px; }
.st-upcoming .st-status-bar { background: #3498db; }
.st-running .st-status-bar { background: #2ecc71; box-shadow: 2px 0 12px #2ecc71; }
.st-finished .st-status-bar { background: #555; }

.st-main { flex: 1; display: flex; flex-direction: column; gap: 0.8rem; }
.st-time { display: flex; align-items: baseline; gap: 0.8rem; }
.start-time { font-size: 1.6rem; font-weight: 900; color: #fff; line-height: 1; }
.duration { font-size: 0.75rem; letter-spacing: 1px; font-weight: 700; opacity: 0.6; }

.movie-title { font-size: 1.1rem; margin: 0; color: #fff; font-weight: 800; max-width: 250px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.price-info { color: var(--color-primary); font-weight: 900; font-size: 0.95rem; }

.st-occupancy { margin-top: 0.5rem; }
.occ-text { display: flex; justify-content: space-between; font-size: 0.7rem; font-weight: 700; margin-bottom: 0.5rem; opacity: 0.7; }
.occ-progress { height: 5px; background: rgba(255,255,255,0.05); border-radius: 10px; overflow: hidden; }
.occ-fill { height: 100%; background: linear-gradient(90deg, var(--color-primary), #ffd43b); transition: 1s ease-out; }

.st-actions { display: flex; flex-direction: column; gap: 15px; margin-left: 1rem; border-left: 1px solid rgba(255,255,255,0.08); padding-left: 1.2rem; }
.action-btn { background: rgba(255,255,255,0.03); border: none; width: 32px; height: 32px; border-radius: 8px; cursor: pointer; color: #aaa; transition: 0.2s; display: grid; place-items: center; }
.edit-btn:hover { background: rgba(52,152,219,0.15); color: #3498db; transform: scale(1.1); }
.delete-btn:hover { background: rgba(231,76,60,0.15); color: #e74c3c; transform: scale(1.1); }

/* Empty state */
.empty-state { padding: 5rem; text-align: center; border-radius: 20px; border: 1px dashed rgba(255,255,255,0.1); }
.empty-icon { font-size: 4rem; opacity: 0.2; margin-bottom: 1.5rem; }

/* Modal Overlay & Card */
.modal-root {
  position: fixed; top: 0; left: 0; width: 100%; height: 100%;
  background: rgba(0,0,0,0.85); backdrop-filter: blur(15px); z-index: 10000;
  display: flex; align-items: center; justify-content: center; padding: 2rem;
}
.modal-dialog { width: 100%; max-width: 680px; padding: 3rem; background: #0c0c0c; border: 1px solid rgba(255,255,255,0.1); border-radius: 30px; box-shadow: 0 30px 60px rgba(0,0,0,0.6); }

.modal-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 2.5rem; }
.modal-header h2 { font-size: 1.8rem; margin: 0; display: flex; align-items: center; gap: 15px; }

.close-btn { background: none; border: none; color: #555; font-size: 2.5rem; cursor: pointer; transition: 0.3s; }
.close-btn:hover { color: #fff; transform: rotate(90deg); }

.price-input-group { position: relative; }
.unit-text { position: absolute; right: 20px; top: 50%; transform: translateY(-50%); opacity: 0.4; font-weight: 700; }
.alert-box { background: rgba(52,152,219,0.05); border: 1px solid rgba(52,152,219,0.2); padding: 1.25rem; border-radius: 12px; color: #3498db; font-size: 0.85rem; display: flex; gap: 12px; align-items: center; }

.modal-notifications { padding: 0 30px; }

.modal-alert {
  padding: 12px 16px;
  border-radius: 8px;
  margin-top: 15px;
  display: flex;
  align-items: center;
  gap: 12px;
  font-size: 0.9rem;
  font-weight: 500;
  position: relative;
  width: 100%;
}

.modal-alert-error {
  background: rgba(231, 76, 60, 0.1);
  border-left: 4px solid #e74c3c;
  color: #e74c3c;
}

.modal-alert-success {
  background: rgba(46, 204, 113, 0.1);
  border-left: 4px solid #2ecc71;
  color: #2ecc71;
}

.modal-alert .close-alert {
  background: none;
  border: none;
  color: inherit;
  font-size: 1.2rem;
  cursor: pointer;
  margin-left: auto;
  opacity: 0.6;
}
.modal-alert .close-alert:hover { opacity: 1; }

/* End Time Preview */
.end-time-preview {
  margin-top: 1rem;
  padding: 0.8rem 1rem;
  background: rgba(232, 136, 42, 0.05);
  border: 1px dashed rgba(232, 136, 42, 0.2);
  border-radius: 12px;
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.preview-item {
  display: flex;
  align-items: center;
  gap: 10px;
  color: var(--color-primary);
  font-size: 0.95rem;
}
.preview-item strong { font-size: 1.1rem; }
.preview-note {
  font-size: 0.75rem;
  color: #888;
  font-style: italic;
  padding-left: 26px;
}

/* Animations */
@keyframes shake {
  0%, 100% { transform: translateX(0); }
  25% { transform: translateX(-5px); }
  75% { transform: translateX(5px); }
}
.animate-shake { animation: shake 0.3s ease-in-out 2; }
.animate-in { animation: fadeInScale 0.6s ease-out forwards; }
@keyframes fadeInScale { from { opacity: 0; transform: scale(0.98) translateY(10px); } to { opacity: 1; transform: scale(1) translateY(0); } }

.animate-modal { animation: modalIn 0.4s cubic-bezier(0.19, 1, 0.22, 1) forwards; }
@keyframes modalIn { from { opacity: 0; transform: scale(0.9) translateY(40px); } to { opacity: 1; transform: scale(1) translateY(0); } }

.modal-fade-enter-active, .modal-fade-leave-active { transition: opacity 0.35s ease; }
.modal-fade-enter-from, .modal-fade-leave-to { opacity: 0; }
</style>
