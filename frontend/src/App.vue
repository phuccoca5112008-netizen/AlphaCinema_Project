<template>
  <div id="app">
    <!-- Navbar -->
    <header class="navbar">
      <div class="container nav-inner">
        <!-- Logo -->
        <router-link to="/" class="logo">
          <img :src="logoImg" alt="Alpha Cinema Logo" class="brand-logo-img">
        </router-link>

        <!-- Nav Links -->
        <nav class="nav-links">
          <router-link to="/">Trang Chủ</router-link>
          
          <!-- Phim Dropdown -->
          <div class="nav-item-dropdown">
            <router-link to="/movies" class="dropdown-trigger">Phim</router-link>
            <div class="dropdown-menu">
              <router-link to="/movies?trangThai=Đang chiếu" class="dropdown-item">Phim Đang Chiếu</router-link>
              <router-link to="/movies?trangThai=Sắp chiếu" class="dropdown-item">Phim Sắp Chiếu</router-link>
            </div>
          </div>

          <router-link to="/promotions">Ưu Đãi</router-link>
          <router-link to="/booking">Đặt Vé</router-link>
        </nav>

        <!-- Auth -->
        <div class="nav-auth" v-if="!authStore.isAuthenticated">
          <router-link to="/login" class="btn-nav-link">Đăng Nhập</router-link>
          <router-link to="/register" class="btn btn-primary btn-sm">Đăng Ký</router-link>
        </div>
        <div class="nav-auth" v-else>
          <router-link v-if="authStore.isAdmin" to="/admin" class="admin-chip">⚙ Quản Trị</router-link>
          <div class="user-menu">
            <router-link to="/profile" class="user-profile-link">
              <div class="avatar">{{ authStore.user?.hoTen?.charAt(0) || 'U' }}</div>
              <span class="user-name">{{ authStore.user?.hoTen }}</span>
            </router-link>
            <button @click="logout" class="btn-nav-link text-error">Thoát</button>
          </div>
        </div>
      </div>
    </header>

    <!-- Page content -->
    <main>
      <router-view v-slot="{ Component }">
        <transition name="fade" mode="out-in">
          <component :is="Component" />
        </transition>
      </router-view>
    </main>

    <!-- Footer -->
    <footer class="site-footer">
      <div class="container footer-grid text-center-mobile">
        <div class="footer-about">
          <div class="logo mb-3">
            <img :src="logoImg" alt="Alpha Cinema Logo" class="brand-logo-img" style="height: 50px;">
          </div>
          <p class="text-sub">Hệ thống rạp chiếu phim hiện đại với công nghệ IMAX &amp; VIP chuẩn quốc tế.</p>
        </div>
        <div class="footer-links-col">
          <h4 class="footer-heading">Khám Phá</h4>
          <router-link to="/promotions" class="footer-link">Ưu Đãi</router-link>
          <router-link to="/booking" class="footer-link">Đặt Vé</router-link>
          <router-link to="/movies?trangThai=Sắp chiếu" class="footer-link">Phim Sắp Chiếu</router-link>
        </div>
        <div class="footer-links-col">
          <h4 class="footer-heading">Tài Khoản</h4>
          <router-link to="/login" class="footer-link">Đăng Nhập</router-link>
          <router-link to="/register" class="footer-link">Đăng Ký</router-link>
          <router-link to="/profile" class="footer-link">Thông tin cá nhân</router-link>
        </div>
        <div class="footer-links-col">
          <h4 class="footer-heading">Liên Hệ</h4>
          <p class="footer-link">📧 hotro@alphacinema.vn</p>
          <p class="footer-link">📞 1900 1000</p>
          <div class="social-links mt-2">...</div>
        </div>
      </div>
      <div class="footer-bar">© 2026 Alpha Cinema. All rights reserved.</div>
    </footer>

    <!-- Toasts Global -->
    <ToastContainer />
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue';
import { useAuthStore } from './stores/auth';
import { useRouter } from 'vue-router';
import ToastContainer from './components/ToastContainer.vue';
import logoImg from './assets/logo.svg';

const authStore = useAuthStore();
const router = useRouter();

const logout = () => { authStore.logout(); router.push('/'); };
</script>

<style scoped>
/* ─── Navbar ─── */
.navbar {
  position: absolute;
  top: 0; left: 0; width: 100%;
  z-index: 999;
  padding: 1.5rem 0;
  background: linear-gradient(to bottom, rgba(0,0,0,0.85) 0%, transparent 100%);
}

.nav-inner {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 2rem;
}

/* Logo */
.logo {
  display: flex;
  align-items: center;
  text-decoration: none;
  flex-shrink: 0;
  transition: transform 0.3s ease;
}
.logo:hover { transform: scale(1.05); }

.brand-logo-img {
  height: 65px; /* Giảm nhẹ để vừa vặn hơn với text bên cạnh */
  width: auto;
  object-fit: contain;
}

.logo-accent { display: none; }

/* Nav links */
.nav-links { display: flex; gap: 2rem; }
.nav-links a {
  color: rgba(255,255,255,0.8);
  text-decoration: none;
  font-weight: 600;
  font-size: 0.95rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  position: relative;
  transition: color 0.2s;
  padding-bottom: 4px;
}
.nav-links a::after {
  content: '';
  position: absolute;
  bottom: -2px; left: 0;
  height: 2px; width: 0;
  background: var(--color-primary);
  border-radius: 2px;
  transition: width 0.25s ease;
}
.nav-links a:hover, .nav-links a.router-link-active {
  color: #fff;
}
.nav-links a:hover::after, .nav-links a.router-link-active::after { width: 100%; }

/* Dropdown */
.nav-item-dropdown {
  position: relative;
  display: flex;
  align-items: center;
}
.dropdown-menu {
  position: absolute;
  top: 100%;
  left: 50%;
  transform: translateX(-50%) translateY(10px);
  background: rgba(13, 13, 13, 0.95);
  border: 1px solid rgba(255,255,255,0.08);
  border-radius: 8px;
  min-width: 200px;
  opacity: 0;
  visibility: hidden;
  transition: all 0.3s ease;
  box-shadow: 0 10px 30px rgba(0,0,0,0.5);
  padding: 0.5rem 0;
  backdrop-filter: blur(10px);
}
.nav-item-dropdown:hover .dropdown-menu {
  opacity: 1;
  visibility: visible;
  transform: translateX(-50%) translateY(0);
}
.dropdown-item {
  display: block !important;
  padding: 0.8rem 1.5rem !important;
  color: rgba(255,255,255,0.7) !important;
  text-decoration: none !important;
  font-size: 0.88rem !important;
  font-weight: 500 !important;
  text-transform: none !important;
  transition: all 0.2s !important;
  letter-spacing: 0 !important;
}
.dropdown-item::after { display: none !important; }
.dropdown-item:hover {
  background: rgba(255,255,255,0.05);
  color: var(--color-primary) !important;
  padding-left: 1.8rem !important;
}

/* Auth */
.nav-auth { display: flex; align-items: center; gap: 1rem; }
.btn-nav-link {
  background: none; border: none; cursor: pointer;
  color: rgba(255,255,255,0.75);
  font-family: var(--font-family);
  font-weight: 600;
  font-size: 0.95rem;
  text-decoration: none;
  transition: color 0.2s;
  text-transform: uppercase;
}
.btn-nav-link:hover { color: #fff; }
.btn-sm { padding: 0.5rem 1.2rem; font-size: 0.88rem; }

.admin-chip {
  background: rgba(232,136,42,0.12);
  color: var(--color-primary);
  border: 1px solid rgba(232,136,42,0.3);
  border-radius: 20px;
  padding: 0.3rem 0.9rem;
  font-size: 0.85rem;
  font-weight: 700;
  text-decoration: none;
  transition: 0.2s;
}
.admin-chip:hover { background: rgba(232,136,42,0.2); }

.user-menu { display: flex; align-items: center; gap: 0.75rem; }
.user-profile-link { display: flex; align-items: center; gap: 0.75rem; text-decoration: none; transition: 0.2s; }
.user-profile-link:hover { opacity: 0.8; }
.avatar {
  width: 36px; height: 36px;
  border-radius: 50%;
  background: var(--color-primary);
  display: flex; align-items: center; justify-content: center;
  font-weight: 800; font-size: 1rem;
  color: #fff;
  flex-shrink: 0;
}
.user-name {
  font-weight: 600;
  font-size: 0.9rem;
  color: var(--color-text-main);
}
.text-error { color: var(--color-error) !important; }

main { min-height: 100vh; padding-top: 70px; }

/* ─── Footer ─── */
.site-footer {
  background: #0a0a0a;
  border-top: 1px solid rgba(255,255,255,0.05);
  padding: 4rem 0 0 0;
  margin-top: 5rem;
}
.footer-grid {
  display: grid;
  grid-template-columns: 1.5fr 1fr 1fr 1fr;
  gap: 4rem;
  padding-bottom: 4rem;
}
.footer-about p { font-size: 0.95rem; line-height: 1.8; opacity: 0.8; }
.footer-heading {
  font-size: 0.9rem;
  text-transform: uppercase;
  letter-spacing: 1.5px;
  color: #fff;
  font-weight: 800;
  margin-bottom: 1.6rem;
}
.footer-link {
  display: block;
  color: var(--color-text-sub);
  text-decoration: none;
  font-size: 0.95rem;
  margin-bottom: 0.8rem;
  transition: all 0.2s;
}
.footer-link:hover { color: var(--color-primary); transform: translateX(5px); }
.footer-bar {
  border-top: 1px solid rgba(255,255,255,0.06);
  text-align: center;
  padding: 1.5rem;
  font-size: 0.88rem;
  color: var(--color-text-muted);
  background: #080808;
}

@media (max-width: 992px) {
  .footer-grid { grid-template-columns: 1fr 1fr; gap: 3rem; }
}

@media (max-width: 768px) {
  .nav-links { display: none; }
  .footer-grid { grid-template-columns: 1fr; gap: 2rem; text-align: center; }
  .footer-link:hover { transform: none; }
}
</style>
