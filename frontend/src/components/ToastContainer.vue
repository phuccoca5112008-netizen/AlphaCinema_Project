<template>
  <div class="toast-container">
    <transition-group name="toast">
      <div v-for="t in toast.toasts" :key="t.id" class="toast-item" :class="t.type">
        <div class="toast-icon">
          <i v-if="t.type === 'success'" class="fas fa-check-circle"></i>
          <i v-else-if="t.type === 'error'" class="fas fa-exclamation-circle"></i>
          <i v-else class="fas fa-info-circle"></i>
        </div>
        <div class="toast-message">{{ t.message }}</div>
        <button class="toast-close" @click="toast.remove(t.id)">×</button>
      </div>
    </transition-group>
  </div>
</template>

<script setup>
import { useToastStore } from '../stores/toast';
const toast = useToastStore();
</script>

<style scoped>
.toast-container {
  position: fixed;
  top: 1.5rem;
  right: 1.5rem;
  z-index: 9999;
  display: flex;
  flex-direction: column;
  gap: 0.8rem;
  max-width: 400px;
}

.toast-item {
  display: flex;
  align-items: center;
  background: rgba(15, 15, 15, 0.95);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  padding: 1rem 1.2rem;
  border-radius: 12px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.5);
  min-width: 300px;
  animation: slideIn 0.4s cubic-bezier(0.18, 0.89, 0.32, 1.28);
}

@keyframes slideIn {
  from { transform: translateX(100%); opacity: 0; }
  to { transform: translateX(0); opacity: 1; }
}

.toast-item.success { border-left: 4px solid #10b981; }
.toast-item.error { border-left: 4px solid #ef4444; }
.toast-item.info { border-left: 4px solid #3b82f6; }

.toast-icon { margin-right: 0.8rem; font-size: 1.2rem; }
.toast-item.success .toast-icon { color: #10b981; }
.toast-item.error .toast-icon { color: #ef4444; }
.toast-item.info .toast-icon { color: #3b82f6; }

.toast-message { flex: 1; font-size: 0.95rem; font-weight: 500; color: #fff; line-height: 1.4; }
.toast-close { background: none; border: none; color: rgba(255, 255, 255, 0.5); font-size: 1.2rem; cursor: pointer; padding: 0 0 0 0.8rem; }
.toast-close:hover { color: #fff; }

.toast-leave-active { animation: slideOut 0.3s forwards; }
@keyframes slideOut {
  to { transform: translateX(100%); opacity: 0; }
}
</style>
