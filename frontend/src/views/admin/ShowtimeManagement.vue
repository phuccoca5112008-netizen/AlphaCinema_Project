<template>
  <div class="showtime-management">
    <div class="header-row mb-4">
      <h1 class="gradient-text m-0">Quản Lý Suất Chiếu</h1>
      <button class="btn btn-primary" @click="openModal()">+ Thêm Suất Chiếu</button>
    </div>

    <!-- Filter phim -->
    <div class="glass-panel filter-box mb-4">
      <label class="form-label" style="display:inline-block; margin-right: 1rem;">Lọc theo phim:</label>
      <select v-model="filterPhim" class="form-input" style="width: 300px; display:inline-block;" @change="fetchShowtimes">
        <option value="">-- Tất cả phim --</option>
        <option v-for="p in phims" :key="p.maPhim" :value="p.maPhim">{{ p.tenPhim }}</option>
      </select>
    </div>

    <!-- Danh sách suất chiếu -->
    <div class="glass-panel table-container">
      <div v-if="loading" class="p-4 text-muted">Đang tải danh sách...</div>
      <table class="data-table" v-else>
        <thead>
          <tr>
            <th>Phim</th>
            <th>Phòng / Định Dạng</th>
            <th>Thời Gian Bắt Đầu</th>
            <th>Giá Gốc (VNĐ)</th>
            <th class="text-right">Hành Động</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="s in showtimes" :key="s.maSuatChieu">
            <td><strong>{{ s.tenPhim }}</strong></td>
            <td>Phòng {{ s.tenPhong }} - {{ s.dinhDang }}</td>
            <td class="text-accent">{{ new Date(s.thoiGianBatDau).toLocaleString() }}</td>
            <td class="text-success">{{ s.giaVeGoc.toLocaleString() }}đ</td>
            <td class="text-right">
              <button class="btn btn-outline btn-sm action-btn text-error border-error" @click="deleteShowtime(s.maSuatChieu)">Xóa</button>
            </td>
          </tr>
          <tr v-if="showtimes.length === 0">
            <td colspan="5" class="text-center text-muted p-4">Không có suất chiếu nào.</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Modal Form (Create Only for simplicity here, updating showtime usually involves more rules) -->
    <div class="modal-backdrop" v-if="showModal">
      <div class="modal-content glass-panel">
        <h3>Thêm Suất Chiếu Mới</h3>
        
        <form @submit.prevent="saveShowtime" class="mt-4">
          <div class="form-group row">
            <div class="col">
              <label class="form-label">Chọn phim</label>
              <select v-model="formData.maPhim" class="form-input" required>
                <option v-for="p in phims" :key="p.maPhim" :value="p.maPhim">{{ p.tenPhim }}</option>
              </select>
            </div>
            <div class="col">
              <label class="form-label">Phòng chiếu (1-5)</label>
              <input type="number" v-model="formData.maPhong" class="form-input" required min="1" max="5">
              <small class="text-muted">Database seed sẵn Phòng 1 đến 5.</small>
            </div>
          </div>
          
          <div class="form-group row">
            <div class="col">
              <label class="form-label">Thời gian bắt đầu</label>
              <input type="datetime-local" v-model="formData.thoiGianBatDau" class="form-input" required>
            </div>
            <div class="col">
              <label class="form-label">Giá vé gốc</label>
              <input type="number" v-model="formData.giaVeGoc" class="form-input" required min="0" step="5000">
            </div>
          </div>

          <div class="form-group">
            <label class="form-label">Định dạng</label>
            <select v-model="formData.dinhDang" class="form-input">
              <option value="2D">2D Tiêu Chuẩn</option>
              <option value="3D">3D Cao Cấp</option>
              <option value="IMAX">IMAX Hiện Đại</option>
            </select>
          </div>

          <div class="modal-actions">
            <button type="button" class="btn btn-outline" @click="showModal = false">Hủy</button>
            <button type="submit" class="btn btn-primary" :disabled="saving">{{ saving ? 'Đang lưu...' : 'Lưu' }}</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import api from '../../api/axios';

const showtimes = ref([]);
const phims = ref([]);
const filterPhim = ref('');
const loading = ref(true);

const showModal = ref(false);
const saving = ref(false);
const formData = ref({});

onMounted(async () => {
  await fetchPhims();
  await fetchShowtimes();
});

const fetchPhims = async () => {
  try {
    const res = await api.get('/phim');
    if (res.success) phims.value = res.data;
  } catch (error) { console.error(error); }
}

const fetchShowtimes = async () => {
  loading.value = true;
  try {
    const url = filterPhim.value ? `/suat-chieu?maPhim=${filterPhim.value}` : '/suat-chieu';
    const res = await api.get(url);
    if (res.success) showtimes.value = res.data;
  } catch (error) {
    console.error(error);
  } finally {
    loading.value = false;
  }
}

const openModal = () => {
  // ISO string yyyy-MM-ddThh:mm for datetime-local
  const defaultDate = new Date();
  defaultDate.setDate(defaultDate.getDate() + 1); // Tomorrow
  defaultDate.setHours(19, 0, 0, 0); // 19:00
  // Pad function for datetime string
  const pad = (n) => (n < 10 ? '0' + n : n);
  const dateString = `${defaultDate.getFullYear()}-${pad(defaultDate.getMonth()+1)}-${pad(defaultDate.getDate())}T${pad(defaultDate.getHours())}:${pad(defaultDate.getMinutes())}`;

  formData.value = { 
    maPhim: phims.value.length > 0 ? phims.value[0].maPhim : 1, 
    maPhong: 1, 
    thoiGianBatDau: dateString, 
    giaVeGoc: 80000, 
    dinhDang: '2D' 
  };
  showModal.value = true;
}

const saveShowtime = async () => {
  saving.value = true;
  try {
    // Send standard ISO 
    const payload = { ...formData.value, thoiGianBatDau: new Date(formData.value.thoiGianBatDau).toISOString() };
    await api.post('/suat-chieu', payload);
    
    showModal.value = false;
    await fetchShowtimes(); 
  } catch(e) {
    alert(e.message || "Lỗi khi lưu suất chiếu. Kiểm tra lại phòng/phi giờ.");
  } finally {
    saving.value = false;
  }
}

const deleteShowtime = async (id) => {
  if (confirm("Chắc chắn xóa suất chiếu này? (Sẽ lỗi nếu đã có khách đặt vé)")) {
    try {
      await api.delete(`/suat-chieu/${id}`);
      await fetchShowtimes();
    } catch(e) {
      alert("Không thể xóa - đã có vé đặt cho suất này.");
    }
  }
}
</script>

<style scoped>
/* Reusing admin list styles */
.m-0 { margin: 0; }
.mb-4 { margin-bottom: 2rem; }
.mt-4 { margin-top: 1.5rem; }
.p-4 { padding: 2rem; }
.text-right { text-align: right; }
.text-center { text-align: center; }
.text-muted { color: var(--color-text-muted); }
.text-success { color: var(--color-success); font-weight: 600; }
.text-accent { color: var(--color-accent); font-weight: 600; }

.header-row { display: flex; justify-content: space-between; align-items: center; }
.filter-box { padding: 1rem 1.5rem; }

.action-btn { padding: 0.3rem 0.6rem; font-size: 0.85rem; margin-left: 0.5rem; }
.text-error { color: var(--color-error) !important; }
.border-error { border-color: rgba(255, 23, 68, 0.5) !important; }

/* Table Styles - reused logic */
.table-container { overflow-x: auto; }
.data-table { width: 100%; border-collapse: collapse; }
.data-table th, .data-table td { padding: 1rem; border-bottom: 1px solid rgba(255,255,255,0.05); text-align: left; vertical-align: middle;}
.data-table th { color: var(--color-text-muted); font-weight: 600; text-transform: uppercase; font-size: 0.85rem; }
.data-table tbody tr:hover { background: rgba(255,255,255,0.02); }

/* Modal Styles */
.modal-backdrop {
  position: fixed; top: 0; left: 0; width: 100%; height: 100%;
  background: rgba(0,0,0,0.7); backdrop-filter: blur(5px); z-index: 1000;
  display: flex; align-items: center; justify-content: center;
}
.modal-content { width: 100%; max-width: 600px; padding: 2.5rem; background: var(--color-bg-base); }
.modal-content h3 { font-size: 1.5rem; color: var(--color-primary); }
.row { display: flex; gap: 1rem; }
.col { flex: 1; }
.modal-actions { display: flex; justify-content: flex-end; gap: 1rem; margin-top: 2rem; }
</style>
