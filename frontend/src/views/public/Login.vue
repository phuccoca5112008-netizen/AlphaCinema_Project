<template>
  <div class="auth-page">
    <div class="auth-card glass-panel">
      <h2 class="auth-title">Chào mừng trở lại</h2>
      <p class="auth-subtitle">Đăng nhập tài khoản Alpha Cinema của bạn</p>

      <form @submit.prevent="handleLogin" class="auth-form">
        <div class="form-group">
          <label class="form-label">Email hoặc tài khoản</label>
          <input type="text" v-model="email" class="form-input" placeholder="Nhập email hoặc tài khoản của bạn" required />
        </div>
        <div class="form-group">
          <label class="form-label">Mật khẩu</label>
          <input type="password" v-model="password" class="form-input" placeholder="Nhập mật khẩu" required />
        </div>

        <div v-if="error" class="error-alert">{{ error }}</div>

        <button type="submit" class="btn btn-primary auth-submit" :disabled="loading">
          {{ loading ? 'Đang xử lý...' : 'Đăng nhập' }}
        </button>
      </form>

      <div class="auth-footer">
        Bạn chưa có tài khoản? <router-link to="/register" class="gradient-text">Đăng ký ngay</router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useAuthStore } from '../../stores/auth';
import { useRouter } from 'vue-router';

const authStore = useAuthStore();
const router = useRouter();

const email = ref('');
const password = ref('');
const error = ref('');
const loading = ref(false);

const handleLogin = async () => {
  error.value = '';
  loading.value = true;
  
  const res = await authStore.login(email.value, password.value);
  
  if (res.success) {
    if (authStore.isAdmin) router.push('/admin');
    else router.push('/');
  } else {
    // If backend returns array of errors under `errors`
    if (res.errors && res.errors.length > 0) error.value = res.errors[0];
    else error.value = res.message;
  }
  
  loading.value = false;
};
</script>

<style scoped>
.auth-page {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: calc(100vh - 100px); /* Minus navbar height */
  padding: 2rem;
}

.auth-card {
  width: 100%;
  max-width: 450px;
  padding: 3rem;
  border-radius: var(--radius-lg);
  box-shadow: 0 10px 40px rgba(0,0,0,0.5);
}

.auth-title {
  font-size: 2rem;
  margin-bottom: 0.5rem;
  text-align: center;
}

.auth-subtitle {
  text-align: center;
  color: var(--color-text-muted);
  margin-bottom: 2.5rem;
}

.auth-submit {
  width: 100%;
  margin-top: 1rem;
  padding: 1rem;
  font-size: 1.1rem;
}

.error-alert {
  background: rgba(255, 23, 68, 0.1);
  border: 1px solid var(--color-error);
  color: var(--color-error);
  padding: 0.8rem;
  border-radius: var(--radius-sm);
  margin-bottom: 1rem;
  font-size: 0.9rem;
}

.auth-footer {
  text-align: center;
  margin-top: 2rem;
  color: var(--color-text-muted);
  font-size: 0.95rem;
}

.auth-footer a {
  font-weight: 600;
  text-decoration: none;
}
</style>
