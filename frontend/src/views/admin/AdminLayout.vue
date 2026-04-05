<template>
  <div class="admin-layout">
    <!-- Sidebar -->
    <aside class="sidebar glass-panel">
      <div class="sidebar-header">
        <h2 class="logo">Alpha<span class="gradient-text">Admin</span></h2>
      </div>
      
      <nav class="sidebar-nav">
        <router-link to="/admin/dashboard" class="nav-item">
          <span class="icon">📊</span>
          <span class="text">Thống kê</span>
        </router-link>
        <router-link to="/admin/movies" class="nav-item">
          <span class="icon">🎬</span>
          <span class="text">Phim</span>
        </router-link>
        <router-link to="/admin/rooms" class="nav-item">
          <span class="icon">🏢</span>
          <span class="text">Phòng Chiếu & Ghế</span>
        </router-link>
        <router-link to="/admin/showtimes" class="nav-item">
          <span class="icon">🍿</span>
          <span class="text">Suất Chiếu</span>
        </router-link>
        <router-link to="/admin/promotions" class="nav-item">
          <span class="icon">🎁</span>
          <span class="text">Khuyến Mãi</span>
        </router-link>
        <router-link to="/admin/invoices" class="nav-item">
          <span class="icon">🧾</span>
          <span class="text">Giao Dịch</span>
        </router-link>
        <router-link to="/admin/reviews" class="nav-item">
          <span class="icon">💬</span>
          <span class="text">Đánh Giá</span>
        </router-link>
        <router-link to="/admin/ticket-check" class="nav-item">
          <span class="icon">🔍</span>
          <span class="text">Kiểm Tra Vé</span>
        </router-link>
        <router-link to="/admin/users" class="nav-item">
          <span class="icon">👥</span>
          <span class="text">Người Dùng</span>
        </router-link>
      </nav>

      <div class="sidebar-footer">
        <div class="user-info">
          <div class="avatar">A</div>
          <div class="details">
            <span class="name">{{ authStore.user?.hoTen }}</span>
            <span class="role">Administrator</span>
          </div>
        </div>
        <button class="btn btn-outline logout-btn" @click="logout">Đăng xuất</button>
      </div>
    </aside>

    <!-- Main Content -->
    <main class="main-content">
      <header class="topbar glass-panel">
        <div class="breadcrumb">Bảng điều khiển &gt; {{ $route.name }}</div>
        <div class="actions">
          <router-link to="/" class="btn btn-primary" style="padding: 0.5rem 1rem;">Về Web Khách</router-link>
        </div>
      </header>
      
      <div class="content-wrapper">
        <router-view v-slot="{ Component }">
          <transition name="fade" mode="out-in">
            <component :is="Component" />
          </transition>
        </router-view>
      </div>
    </main>
  </div>
</template>

<script setup>
import { useAuthStore } from '../../stores/auth';
import { useRouter } from 'vue-router';

const authStore = useAuthStore();
const router = useRouter();

const logout = () => {
  authStore.logout();
  router.push('/login');
}
</script>

<style scoped>
.admin-layout {
  display: flex;
  height: 100vh;
  overflow: hidden;
  background: var(--color-bg-base);
}

.sidebar {
  width: 260px;
  display: flex;
  flex-direction: column;
  border-right: 1px solid rgba(255,255,255,0.05);
  border-radius: 0;
  z-index: 10;
}

.sidebar-header {
  padding: 1.5rem;
  border-bottom: 1px solid rgba(255,255,255,0.05);
}

.logo { margin: 0; font-size: 1.5rem; }

.sidebar-nav {
  flex: 1;
  padding: 1.5rem 0;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.nav-item {
  display: flex;
  align-items: center;
  padding: 1rem 1.5rem;
  color: var(--color-text-muted);
  text-decoration: none;
  font-weight: 500;
  transition: all 0.2s;
  border-left: 3px solid transparent;
}

.nav-item .icon { margin-right: 1rem; font-size: 1.2rem; }

.nav-item:hover, .nav-item.router-link-active {
  background: rgba(255, 51, 102, 0.05);
  color: var(--color-primary);
  border-left-color: var(--color-primary);
}

.sidebar-footer {
  padding: 1.5rem;
  border-top: 1px solid rgba(255,255,255,0.05);
}

.user-info {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1rem;
}

.avatar {
  width: 40px; height: 40px;
  border-radius: 50%;
  background: var(--color-primary);
  display: flex; align-items: center; justify-content: center;
  font-weight: bold; color: white;
}

.details { display: flex; flex-direction: column; }
.name { font-size: 0.95rem; font-weight: 600; }
.role { font-size: 0.8rem; color: var(--color-text-muted); }

.logout-btn { width: 100%; padding: 0.5rem; }

.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.topbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 2rem;
  border-bottom: 1px solid rgba(255,255,255,0.05);
  border-radius: 0;
  z-index: 5;
}

.breadcrumb {
  font-size: 0.95rem;
  color: var(--color-text-muted);
}

.content-wrapper {
  flex: 1;
  padding: 2rem;
  overflow-y: auto;
}
</style>
