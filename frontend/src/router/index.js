import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '../stores/auth';

const routes = [
  // Public
  { path: '/', component: () => import('../views/public/Home.vue'), name: 'Home' },
  { path: '/login', component: () => import('../views/public/Login.vue'), name: 'Login' },
  { path: '/register', component: () => import('../views/public/Register.vue'), name: 'Register' },
  { path: '/movies', component: () => import('../views/public/Movies.vue'), name: 'Movies' },
  { path: '/movies/:id', component: () => import('../views/public/MovieDetail.vue'), name: 'MovieDetail' },
  { path: '/promotions', component: () => import('../views/public/Promotions.vue'), name: 'Promotions' },
  
  // Customer
  { 
    path: '/booking', 
    component: () => import('../views/public/Booking.vue'), 
    name: 'Booking',
    meta: { requiresAuth: true }
  },
  { 
    path: '/profile', 
    component: () => import('../views/public/Profile.vue'), 
    name: 'Profile',
    meta: { requiresAuth: true }
  },
  
  // Admin Route Group with Admin Layout
  {
    path: '/admin',
    component: () => import('../views/admin/AdminLayout.vue'),
    meta: { requiresAdmin: true },
    children: [
      { path: '', redirect: '/admin/dashboard' },
      { path: 'dashboard', component: () => import('../views/admin/Dashboard.vue'), name: 'AdminDashboard' },
      { path: 'movies', component: () => import('../views/admin/MovieManagement.vue'), name: 'MovieManagement' },
      { path: 'rooms', component: () => import('../views/admin/RoomManagement.vue'), name: 'RoomManagement' },
      { path: 'showtimes', component: () => import('../views/admin/ShowtimeManagement.vue'), name: 'ShowtimeManagement' },
      { path: 'users', component: () => import('../views/admin/UserManagement.vue'), name: 'UserManagement' },
      { path: 'promotions', component: () => import('../views/admin/PromotionManagement.vue'), name: 'PromotionManagement' },
      { path: 'invoices', component: () => import('../views/admin/InvoiceManagement.vue'), name: 'InvoiceManagement' },
      { path: 'reviews', component: () => import('../views/admin/ReviewManagement.vue'), name: 'ReviewManagement' },
      { path: 'ticket-check', component: () => import('../views/admin/TicketCheck.vue'), name: 'TicketCheck' }
    ]
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

// Navigation Guard
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();
  const isAuthenticated = authStore.isAuthenticated;
  
  // Xử lý Admin Route
  if (to.meta.requiresAdmin) {
    if (!isAuthenticated) return next('/login');
    if (!authStore.isAdmin) return next('/'); // Cấm khách vào admin
  }

  // Xử lý Customer Route
  if (to.meta.requiresAuth && !isAuthenticated) {
    return next('/login');
  } 
  
  // Xử lý Guest Route (như trang Đăng nhập)
  if (to.meta.requiresGuest && isAuthenticated) {
    return next('/');
  }

  next();
});

export default router;
