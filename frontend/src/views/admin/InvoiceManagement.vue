<template>
  <div class="invoice-management">
    <div class="header-actions">
      <h2 class="page-title">Quản Lý Giao Dịch & Hóa Đơn</h2>
    </div>

    <div v-if="loading" class="loading-state">Đang tải biểu đồ và dữ liệu...</div>
    
    <div v-else class="content-wrapper">
      <!-- Thống kê Card -->
      <div class="stats-grid mb-4">
        <div class="stat-card glass-panel">
          <div class="stat-title">Tổng Doanh Thu</div>
          <div class="stat-value text-accent">{{ totalRevenue.toLocaleString() }}đ</div>
        </div>
        <div class="stat-card glass-panel">
          <div class="stat-title">Tổng Xuất Vé</div>
          <div class="stat-value text-primary">{{ invoices.length }} Giao dịch</div>
        </div>
      </div>

      <!-- Danh sách hóa đơn -->
      <div class="table-container glass-panel">
        <table class="admin-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Khách Hàng</th>
              <th>Ngày Đặt</th>
              <th>Tổng Tiền</th>
              <th>Mã Khuyến Mãi</th>
              <th>Trạng Thái</th>
              <th>Hình Thức</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="inv in invoices" :key="inv.maHoaDon">
              <td class="font-bold">#{{ inv.maHoaDon }}</td>
              <td>
                <div class="font-bold text-white">{{ inv.tenNguoiDung || inv.hoTenKhachHang || 'Khách vãng lai' }}</div>
                <div class="small-text">{{ inv.email || 'N/A' }}</div>
              </td>
              <td>{{ inv.ngayLap ? new Date(inv.ngayLap).toLocaleString('vi-VN') : 'N/A' }}</td>
              <td class="text-accent font-bold">{{ (inv.tongTien || 0).toLocaleString() }}đ</td>
              <td>
                <span v-if="inv.maCodeGiamGia" class="vc-badge">{{ inv.maCodeGiamGia }}</span>
                <span v-else class="small-text">-</span>
              </td>
              <td>
                <span class="badge badge-success">{{ inv.trangThai || 'Đã Thanh Toán' }}</span>
              </td>
              <td>{{ inv.phuongThucThanhToan }}</td>
            </tr>
            <tr v-if="invoices.length === 0">
              <td colspan="7" class="text-center p-4">Chưa có giao dịch nào.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { bookingApi } from '../../api/bookingApi';

const invoices = ref([]);
const loading = ref(true);

const totalRevenue = computed(() => {
  return invoices.value.reduce((sum, inv) => sum + inv.tongTien, 0);
});

const loadInvoices = async () => {
  try {
    const res = await bookingApi.getInvoices();
    if (res.success) {
      // Sort newest first
      invoices.value = res.data.sort((a, b) => {
        const dateA = new Date(a.ngayLap || a.ngayGiaoDich || 0);
        const dateB = new Date(b.ngayLap || b.ngayGiaoDich || 0);
        return dateB - dateA;
      });
    }
  } catch (error) {
    console.error('Lỗi tải hóa đơn:', error);
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  loadInvoices();
});
</script>

<style scoped>
.page-title { font-size: 1.8rem; font-weight: 700; margin-bottom: 0; }
.header-actions { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; }
.mb-4 { margin-bottom: 1.5rem; }

.stats-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 1.5rem; }
.stat-card { padding: 1.5rem; border-radius: var(--radius-md); }
.stat-title { font-size: 0.9rem; color: var(--color-text-muted); text-transform: uppercase; font-weight: 600; margin-bottom: 0.5rem; }
.stat-value { font-size: 2rem; font-weight: bold; }
.text-accent { color: var(--color-accent); }
.text-primary { color: var(--color-primary); }

.table-container { border-radius: var(--radius-lg); overflow: hidden; background: var(--color-bg-card); }
.admin-table { width: 100%; border-collapse: collapse; }
.admin-table th, .admin-table td { padding: 1rem 1.5rem; text-align: left; border-bottom: 1px solid rgba(255,255,255,0.05); }
.admin-table th { background: rgba(0,0,0,0.3); font-weight: 600; color: var(--color-text-muted); text-transform: uppercase; font-size: 0.85rem; }
.admin-table tr:hover { background: rgba(255,255,255,0.02); }

.font-bold { font-weight: bold; }
.text-white { color: white; }
.small-text { font-size: 0.8rem; color: #888; }

.badge { padding: 4px 10px; border-radius: 4px; font-size: 0.8rem; font-weight: bold; }
.badge-success { background: rgba(46, 213, 115, 0.2); color: #2ed573; }
.vc-badge { background: rgba(255,255,255,0.1); padding: 2px 6px; border-radius: 4px; font-family: monospace; font-size: 0.85rem; color: #1e90ff;}

.loading-state { text-align: center; padding: 3rem; color: #888; }
.text-center { text-align: center; }
</style>
