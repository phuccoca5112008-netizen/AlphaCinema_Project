<template>
  <div class="user-management">
    <div class="header-actions">
      <h2 class="page-title">Quản Lý Người Dùng</h2>
    </div>

    <div v-if="loading" class="loading-state">Đang tải dữ liệu...</div>
    
    <div v-else class="table-container glass-panel">
      <table class="admin-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Họ Tên</th>
            <th>Email</th>
            <th>Vai Trò</th>
            <th>Điểm</th>
            <th>Thao Tác</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in users" :key="user.maNguoiDung">
            <td>{{ user.maNguoiDung }}</td>
            <td class="font-bold">{{ user.hoTen }}</td>
            <td>{{ user.email }}</td>
            <td>
              <span class="badge" :class="getRoleBadgeClass(user.vaiTro)">
                {{ user.vaiTro }}
              </span>
            </td>
            <td>{{ user.diemTichLuy }}</td>
            <td>
              <button class="btn-icon edit" title="Sửa" @click="openEdit(user)">✏️</button>
              <button 
                class="btn-icon delete" 
                title="Xóa" 
                @click="deleteUser(user.maNguoiDung)"
                v-if="user.vaiTro !== 'Admin' || user.maNguoiDung !== currentUserMa"
              >
                🗑️
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Edit Modal -->
    <div v-if="showModal" class="modal-backdrop">
      <div class="modal-content glass-panel">
        <h3 class="modal-title">Sửa Thông Tin Người Dùng</h3>
        
        <form @submit.prevent="updateUser" class="admin-form">
          <div class="form-group">
            <label>Họ Tên</label>
            <input type="text" v-model="editForm.hoTen" class="form-input" required />
          </div>
          <div class="form-group">
            <label>Vai Trò</label>
            <select v-model="editForm.vaiTro" class="form-input" required>
              <option value="Admin">Admin</option>
              <option value="Staff">Staff</option>
              <option value="Customer">Customer</option>
            </select>
          </div>
          <!-- Password field removed for security/privacy -->

          <div class="modal-actions">
            <button type="button" class="btn btn-outline" @click="showModal = false">Hủy</button>
            <button type="submit" class="btn btn-primary">Lưu Thay Đổi</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { adminApi } from '../../api/adminApi';
import { useAuthStore } from '../../stores/auth';

const authStore = useAuthStore();
const currentUserMa = authStore.user?.maNguoiDung;

const users = ref([]);
const loading = ref(true);
const showModal = ref(false);
const editForm = ref({});

const loadUsers = async () => {
  try {
    const res = await adminApi.getUsers();
    if (res.success) {
      users.value = res.data;
    }
  } catch (error) {
    console.error('Lỗi tải người dùng:', error);
  } finally {
    loading.value = false;
  }
};

const getRoleBadgeClass = (role) => {
  if (role === 'Admin') return 'badge-admin';
  if (role === 'Staff') return 'badge-staff';
  return 'badge-customer';
};

const openEdit = (user) => {
  editForm.value = { ...user };
  showModal.value = true;
};

const updateUser = async () => {
  try {
    const res = await adminApi.updateUser(editForm.value.maNguoiDung, {
      hoTen: editForm.value.hoTen,
      vaiTro: editForm.value.vaiTro
    });
    if (res.success) {
      alert('Cập nhật thành công!');
      showModal.value = false;
      loadUsers();
    }
  } catch (error) {
    alert(error.message || 'Lỗi cập nhật');
  }
};

const deleteUser = async (id) => {
  if (!confirm('Bạn có chắc chắn muốn xóa người dùng này?')) return;
  try {
    const res = await adminApi.deleteUser(id);
    if (res.success) {
      alert('Xóa thành công');
      loadUsers();
    }
  } catch (error) {
    alert(error.message || 'Lỗi xóa');
  }
};

onMounted(() => {
  loadUsers();
});
</script>

<style scoped>
.page-title { font-size: 1.8rem; font-weight: 700; }
.header-actions { display: flex; justify-content: space-between; align-items: center; margin-bottom: 2rem; }

.table-container { border-radius: var(--radius-lg); overflow: hidden; }
.admin-table { width: 100%; border-collapse: collapse; }
.admin-table th, .admin-table td { padding: 1.2rem 1.5rem; text-align: left; border-bottom: 1px solid rgba(255,255,255,0.05); }

.badge { padding: 4px 10px; border-radius: 20px; font-size: 0.75rem; font-weight: 700; text-transform: uppercase; }
.badge-admin { background: rgba(255, 51, 102, 0.1); color: #ff3366; border: 1px solid rgba(255, 51, 102, 0.2); }
.badge-staff { background: rgba(232, 136, 42, 0.1); color: #e8882a; border: 1px solid rgba(232, 136, 42, 0.2); }
.badge-customer { background: rgba(79, 195, 247, 0.1); color: #4fc3f7; border: 1px solid rgba(79, 195, 247, 0.2); }

.btn-icon { background: none; border: none; font-size: 1.1rem; cursor: pointer; transition: 0.2s; margin-right: 0.5rem; }
.btn-icon:hover { transform: scale(1.2); }
.btn-icon.edit:hover { color: #4fc3f7; }
.btn-icon.delete:hover { color: #ff3366; }

/* Modal Styles */
.modal-backdrop {
  position: fixed; inset: 0; background: rgba(0,0,0,0.8);
  display: flex; align-items: center; justify-content: center; z-index: 1000;
}
.modal-content {
  width: 100%; max-width: 450px; padding: 2.5rem; border-radius: 24px;
}
.modal-title { margin-bottom: 2rem; }
.form-group { margin-bottom: 1.5rem; }
.form-group label { display: block; margin-bottom: 0.5rem; font-size: 0.85rem; color: #888; }
.form-input { 
  width: 100%; padding: 0.8rem 1.2rem; background: rgba(0,0,0,0.2); 
  border: 1px solid rgba(255,255,255,0.1); border-radius: 12px; color: white;
}
.modal-actions { display: flex; justify-content: flex-end; gap: 1rem; margin-top: 2rem; }
</style>
