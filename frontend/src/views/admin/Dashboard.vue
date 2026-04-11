<template>
  <div class="dashboard">
    <div class="dashboard-header mb-5">
      <h1 class="gradient-text">Bảng Điều Khiển Tổng Quan</h1>
      <p class="text-muted">Chào mừng trở lại, đây là hiệu suất kinh doanh của bạn theo thời gian thực.</p>
    </div>

    <!-- Stats Grid -->
    <div class="stats-grid">
      <div class="stat-card glass-panel" v-for="stat in summaryStats" :key="stat.label">
        <div class="stat-icon" :style="{ color: stat.color, background: stat.color + '15' }">
          <i :class="stat.icon"></i>
        </div>
        <div class="stat-info">
          <span class="stat-label">{{ stat.label }}</span>
          <div class="stat-value-group">
            <h2 class="stat-value">{{ stat.value }}</h2>
            <span v-if="stat.trend" class="stat-trend" :class="stat.trend > 0 ? 'up' : 'down'">
              {{ stat.trend > 0 ? '↑' : '↓' }} {{ Math.abs(stat.trend) }}%
            </span>
          </div>
        </div>
      </div>
    </div>

    <div class="dashboard-content mt-5">
      <!-- Chart Section -->
      <div class="chart-section glass-panel">
        <div class="section-header">
          <h3>Cơ Cấu Doanh Thu (30 Ngày)</h3>
          <p class="text-muted">Phân bổ doanh thu theo từng đầu phim</p>
        </div>
        <div class="chart-container">
          <Pie v-if="!loading && chartData.labels.length > 0" :data="chartData" :options="chartOptions" />
          <div v-else-if="loading" class="loader-box">Đang tính toán dữ liệu...</div>
          <div v-else class="no-data">Chưa có dữ liệu giao dịch</div>
        </div>
      </div>

      <!-- Table Section -->
      <div class="table-section glass-panel">
        <div class="section-header">
          <h3>Bảng Doanh Thu Chi Tiết</h3>
          <button class="btn-text">Xem tất cả báo cáo</button>
        </div>
        
        <div v-if="loading" class="mt-4 text-muted p-4">Đang truy vấn database...</div>
        <div v-else class="table-wrapper">
          <table class="premium-table">
            <thead>
              <tr>
                <th>Phim</th>
                <th class="text-right">Doanh Thu</th>
                <th class="text-center">Tỷ Trọng</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, idx) in doanhThuData" :key="idx">
                <td>
                  <div class="movie-cell">
                    <span class="movie-dot" :style="{ background: getColor(idx) }"></span>
                    <strong>{{ item.tenPhim }}</strong>
                  </div>
                </td>
                <td class="text-right text-success">{{ item.doanhThu.toLocaleString() }}đ</td>
                <td class="text-center">
                   <div class="progress-bar">
                     <div class="progress-fill" :style="{ width: getPercentage(item.doanhThu) + '%', background: getColor(idx) }"></div>
                   </div>
                   <small class="text-muted">{{ getPercentage(item.doanhThu) }}%</small>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { adminApi } from '../../api/adminApi';
import { Pie } from 'vue-chartjs';
import { Chart as ChartJS, Title, Tooltip, Legend, ArcElement, CategoryScale, PieController } from 'chart.js';

ChartJS.register(Title, Tooltip, Legend, ArcElement, CategoryScale, PieController);

const doanhThuData = ref([]);
const totalRevenue = ref(0);
const totalTickets = ref(0);
const totalInvoices = ref(0);
const loading = ref(true);

const colors = [
  '#E8882A', '#FF3366', '#4FC3F7', '#81C784', '#BA68C8', 
  '#FFD54F', '#4DB6AC', '#A1887F', '#90A4AE', '#64B5F6'
];

const summaryStats = computed(() => [
  { 
    label: 'Tổng Doanh Thu', 
    value: totalRevenue.value.toLocaleString() + 'đ', 
    icon: 'fas fa-wallet', 
    color: '#E8882A',
    trend: 12.5
  },
  { 
    label: 'Vé Đã Bán', 
    value: totalTickets.value.toString(), 
    icon: 'fas fa-ticket-alt', 
    color: '#FF3366',
    trend: 0
  },
  { 
    label: 'Tổng Hóa Đơn', 
    value: totalInvoices.value.toString(), 
    icon: 'fas fa-file-invoice-dollar', 
    color: '#4FC3F7',
    trend: 8.2
  },
  { 
    label: 'Tỷ Lệ Lấp Đầy', 
    value: '42%', 
    icon: 'fas fa-chair', 
    color: '#81C784',
    trend: -2.4
  }
]);

const getPercentage = (value) => {
  if (totalRevenue.value === 0) return 0;
  return ((value / totalRevenue.value) * 100).toFixed(1);
};

const getColor = (idx) => colors[idx % colors.length];

const chartData = computed(() => ({
  labels: doanhThuData.value.map(d => d.tenPhim),
  datasets: [{
    data: doanhThuData.value.map(d => d.doanhThu),
    backgroundColor: doanhThuData.value.map((_, i) => colors[i % colors.length]),
    borderWidth: 0,
    hoverOffset: 15
  }]
}));

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'bottom',
      labels: { color: '#ccc', font: { size: 11 }, padding: 15, usePointStyle: true }
    },
    tooltip: {
      padding: 12,
      backgroundColor: 'rgba(0,0,0,0.8)',
      titleFont: { size: 14, weight: 'bold' },
      bodyFont: { size: 13 },
      callbacks: {
        label: (context) => ` ${context.label}: ${context.raw.toLocaleString()}đ`
      }
    }
  }
};

onMounted(async () => {
  try {
    const today = new Date();
    const thirtyDaysAgo = new Date();
    thirtyDaysAgo.setDate(today.getDate() - 30);
    
    // Use local date strings to avoid UTC skew
    const formatDate = (d) => d.getFullYear() + '-' + 
                              String(d.getMonth() + 1).padStart(2, '0') + '-' + 
                              String(d.getDate()).padStart(2, '0');

    const tuNgay = formatDate(thirtyDaysAgo);
    const denNgay = formatDate(today);

    const res = await adminApi.getRevenueStats(tuNgay, denNgay);
    if (res.success) {
      doanhThuData.value = res.data.doanhThuTheoPhims || [];
      totalRevenue.value = res.data.tongDoanhThu || 0;
      totalTickets.value = res.data.tongVeBan || 0;
      totalInvoices.value = res.data.tongHoaDon || 0;
    }
  } catch (error) {
    console.error("Lỗi lấy doanh thu:", error);
  } finally {
    loading.value = false;
  }
});
</script>

<style scoped>
.dashboard { padding: 1rem; }

.mb-5 { margin-bottom: 3rem; }
.mt-5 { margin-top: 3rem; }
.text-muted { color: #888; font-size: 0.9rem; }

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: 1.5rem;
}

.stat-card {
  display: flex;
  align-items: center;
  padding: 1.5rem;
  gap: 1.5rem;
  border-radius: 24px;
  transition: all 0.3s;
}

.stat-card:hover { transform: translateY(-5px); border-color: var(--color-primary); }

.stat-icon {
  width: 56px; height: 56px;
  border-radius: 16px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.4rem;
}

.stat-info .stat-label { font-size: 0.85rem; color: #888; text-transform: uppercase; letter-spacing: 0.5px; }
.stat-info .stat-value { font-size: 1.8rem; font-weight: 800; margin: 0.2rem 0; }

.stat-value-group { display: flex; align-items: center; gap: 0.8rem; }
.stat-trend { font-size: 0.75rem; font-weight: 700; padding: 2px 8px; border-radius: 10px; }
.stat-trend.up { background: rgba(0, 230, 118, 0.1); color: #00e676; }
.stat-trend.down { background: rgba(255, 51, 102, 0.1); color: #FF3366; }

.dashboard-content {
  display: grid;
  grid-template-columns: 1fr 1.5fr;
  gap: 2rem;
}

@media (max-width: 1200px) {
  .dashboard-content { grid-template-columns: 1fr; }
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.section-header h3 { font-size: 1.25rem; font-weight: 700; }

.chart-section { padding: 2rem; border-radius: 24px; }
.chart-container { height: 350px; position: relative; }

.table-section { padding: 2rem; border-radius: 24px; }
.premium-table { width: 100%; border-collapse: collapse; }
.premium-table th { 
  text-align: left; padding: 1rem; color: #555; 
  font-size: 0.8rem; border-bottom: 1px solid rgba(255,255,255,0.05);
}
.premium-table td { padding: 1.2rem 1rem; border-bottom: 1px solid rgba(255,255,255,0.03); }

.movie-cell { display: flex; align-items: center; gap: 0.8rem; }
.movie-dot { width: 8px; height: 8px; border-radius: 50%; }

.progress-bar {
  width: 100%; height: 6px;
  background: rgba(255,255,255,0.05);
  border-radius: 10px; overflow: hidden;
  margin-bottom: 4px;
}
.progress-fill { height: 100%; border-radius: 10px; }

.text-success { color: #00e676; font-weight: 700; }
.btn-text { background: none; border: none; color: var(--color-primary); font-weight: 600; cursor: pointer; }

.loader-box, .no-data {
  height: 100%; display: flex; align-items: center; justify-content: center; color: #888;
}
</style>
