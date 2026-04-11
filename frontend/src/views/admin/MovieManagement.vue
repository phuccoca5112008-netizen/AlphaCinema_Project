<template>
  <div class="movie-management">
    <div class="header-row mb-4">
      <h1 class="gradient-text m-0">Quản Lý Phim</h1>
      <div>
        <button class="btn btn-outline text-warning border-warning mr-2" @click="seedData" :disabled="seeding">
          {{ seeding ? 'Đang tải...' : '⬇️ Đồng Bộ Phim Từ TMDb' }}
        </button>
        <button class="btn btn-primary ml-2" @click="openModal()">+ Thêm Phim Mới</button>
      </div>
    </div>

    <!-- Danh sách phim -->
    <div class="glass-panel table-container">
      <div v-if="loading" class="p-4 text-muted">Đang tải danh sách phim...</div>
      <table class="data-table" v-else>
        <thead>
          <tr>
            <th>ID</th>
            <th>Poster</th>
            <th>Tên Phim</th>
            <th>Thời Lượng</th>
            <th>Trạng Thái</th>
            <th class="text-right">Hành Động</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="phim in phims" :key="phim.maPhim">
            <td class="text-muted">#{{ phim.maPhim }}</td>
            <td>
              <img :src="phim.poster || 'https://via.placeholder.com/50x75'" alt="Poster" class="table-img">
            </td>
            <td><strong>{{ phim.tenPhim }}</strong> <br><span class="text-muted text-sm">{{ phim.theLoai }}</span></td>
            <td>{{ phim.thoiLuong }} phút</td>
            <td>
              <span class="badge" :class="getBadgeClass(phim.trangThaiPhim)">{{ phim.trangThaiPhim }}</span>
            </td>
            <td class="text-right">
              <button class="btn btn-outline btn-sm action-btn" @click="openModal(phim)">Sửa</button>
              <button class="btn btn-outline btn-sm action-btn text-error border-error" @click="deletePhim(phim.maPhim)">Xóa</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Modal Form (Basic Implementation) -->
    <div class="modal-backdrop" v-if="showModal">
      <div class="modal-content glass-panel">
        <h3>{{ isEdit ? 'Cập Nhật Phim' : 'Thêm Phim Mới' }}</h3>
        
        <form @submit.prevent="savePhim" class="mt-4">
          <div class="form-group row">
            <div class="col">
              <label class="form-label">Tên Phim</label>
              <input type="text" v-model="formData.tenPhim" class="form-input" required>
            </div>
            <div class="col">
              <label class="form-label">Thể loại</label>
              <input type="text" v-model="formData.theLoai" class="form-input">
            </div>
          </div>
          
          <div class="form-group row">
            <div class="col">
              <label class="form-label">Thời lượng (phút)</label>
              <input type="number" v-model="formData.thoiLuong" class="form-input" required>
            </div>
            <div class="col">
              <label class="form-label">Trạng thái</label>
              <select v-model="formData.trangThaiPhim" class="form-input">
                <option value="Sắp chiếu">Sắp chiếu</option>
                <option value="Đang chiếu">Đang chiếu</option>
                <option value="Ngừng chiếu">Ngừng chiếu</option>
              </select>
            </div>
          </div>

          <div class="form-group">
            <label class="form-label">Link Poster</label>
            <input type="text" v-model="formData.poster" class="form-input">
          </div>
          
          <div class="form-group">
            <label class="form-label">Tóm tắt</label>
            <textarea v-model="formData.tomTat" class="form-input" rows="4"></textarea>
          </div>

          <div class="modal-actions">
            <button type="button" class="btn btn-outline" @click="showModal = false">Hủy</button>
            <button type="submit" class="btn btn-primary" :disabled="saving">{{ saving ? 'Đang lưu...' : 'Lưu Phim' }}</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { movieApi } from '../../api/movieApi';

const phims = ref([]);
const loading = ref(true);

const showModal = ref(false);
const isEdit = ref(false);
const saving = ref(false);
const seeding = ref(false);
const formData = ref({});

const fetchPhims = async () => {
  loading.value = true;
  try {
    const res = await movieApi.getMovies();
    if (res.success) phims.value = res.data;
  } catch (error) {
    console.error(error);
  } finally {
    loading.value = false;
  }
}

onMounted(() => { fetchPhims(); });

const getBadgeClass = (status) => {
  if (status === 'Đang chiếu') return 'badge-success';
  if (status === 'Ngừng chiếu') return 'badge-error';
  return 'badge-warning';
}

const openModal = (phim = null) => {
  if (phim) {
    isEdit.value = true;
    formData.value = { ...phim };
  } else {
    isEdit.value = false;
    formData.value = { tenPhim: '', theLoai: '', thoiLuong: 120, trangThaiPhim: 'Sắp chiếu', poster: '', tomTat: '' };
  }
  showModal.value = true;
}

const savePhim = async () => {
  saving.value = true;
  try {
    if (isEdit.value) {
      await movieApi.updateMovie(formData.value.maPhim, formData.value);
    } else {
      await movieApi.createMovie(formData.value);
    }
    showModal.value = false;
    await fetchPhims(); // Refresh list
  } catch(e) {
    alert(e.message || "Lỗi khi lưu phim");
  } finally {
    saving.value = false;
  }
}

const deletePhim = async (id) => {
  if (confirm("Bạn có chắc chắn muốn xóa phim này? Mọi suất chiếu liên quan cũng sẽ bị ảnh hưởng.")) {
    try {
      await movieApi.deleteMovie(id);
      await fetchPhims();
    } catch(e) {
      alert("Lỗi khi xóa phim.");
    }
  }
}

const seedData = async () => {
  if (confirm("Bạn có muốn tự động lấy dữ liệu từ hệ thống API TMDb và tạo sẵn phòng chiếu, suất chiếu mẫu không?")) {
    seeding.value = true;
    try {
      const res = await movieApi.seedMovies();
      if (res.success) {
        alert("Đồng bộ hoàn tất! Dữ liệu đã được nạp vào hệ thống.");
        await fetchPhims();
      }
    } catch (e) {
      alert("Có lỗi lúc đồng bộ.");
    } finally {
      seeding.value = false;
    }
  }
}
</script>

<style scoped>
.m-0 { margin: 0; }
.mb-4 { margin-bottom: 2rem; }
.mt-4 { margin-top: 1.5rem; }
.p-4 { padding: 2rem; }
.text-right { text-align: right; }
.text-muted { color: var(--color-text-muted); }
.text-sm { font-size: 0.85rem; }

.header-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.table-img {
  width: 50px;
  height: 75px;
  object-fit: cover;
  border-radius: 4px;
}

.action-btn {
  padding: 0.3rem 0.6rem;
  font-size: 0.85rem;
  margin-left: 0.5rem;
}

.text-error { color: var(--color-error) !important; }
.border-error { border-color: rgba(255, 23, 68, 0.5) !important; }

.badge {
  padding: 0.2rem 0.6rem;
  border-radius: 12px;
  font-size: 0.8rem;
  font-weight: 600;
}
.badge-success { background: rgba(0, 230, 118, 0.1); color: var(--color-success); }
.badge-warning { background: rgba(255, 234, 0, 0.1); color: var(--color-warning); }
.badge-error { background: rgba(255, 23, 68, 0.1); color: var(--color-error); }

/* Table Styles - reused logic */
.table-container { overflow-x: auto; }
.data-table { width: 100%; border-collapse: collapse; }
.data-table th, .data-table td { padding: 1rem; border-bottom: 1px solid rgba(255,255,255,0.05); text-align: left; vertical-align: middle;}
.data-table th { color: var(--color-text-muted); font-weight: 600; text-transform: uppercase; font-size: 0.85rem; }
.data-table tbody tr:hover { background: rgba(255,255,255,0.02); }

/* Modal Styles */
.modal-backdrop {
  position: fixed;
  top: 0; left: 0; width: 100%; height: 100%;
  background: rgba(0,0,0,0.7);
  backdrop-filter: blur(5px);
  z-index: 1000;
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal-content {
  width: 100%;
  max-width: 600px;
  padding: 2.5rem;
  background: var(--color-bg-base); /* solid override */
}

.modal-content h3 { font-size: 1.5rem; color: var(--color-primary); }

.row { display: flex; gap: 1rem; }
.col { flex: 1; }

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 2rem;
}
</style>
