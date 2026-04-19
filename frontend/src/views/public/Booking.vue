<template>
  <div class="booking-page">
    <!-- ── STEP INDICATOR ── -->
    <div class="step-bar">
      <div class="container">
        <div class="steps">
          <div class="step" :class="{ active: step >= 1, done: step > 1 }">
            <div class="step-circle">{{ step > 1 ? '✓' : '1' }}</div>
            <span>Chọn Phim & Suất</span>
          </div>
          <div class="step-line" :class="{ done: step > 1 }"></div>
          <div class="step" :class="{ active: step >= 2, done: step > 2 }">
            <div class="step-circle">{{ step > 2 ? '✓' : '2' }}</div>
            <span>Chọn Ghế</span>
          </div>
          <div class="step-line" :class="{ done: step > 2 }"></div>
          <div class="step" :class="{ active: step === 2.5, done: step > 2.5 }">
            <div class="step-circle">{{ step > 2.5 ? '✓' : '3' }}</div>
            <span>Bắp Nước</span>
          </div>
          <div class="step-line" :class="{ done: step > 2.5 }"></div>
          <div class="step" :class="{ active: step === 3 }">
            <div class="step-circle">{{ step === 3 ? '4' : '4' }}</div>
            <span>Thanh Toán</span>
          </div>
        </div>
      </div>
    </div>

    <div class="container booking-layout">

      <!-- ══════════ STEP 1: Chọn Phim & Suất Chiếu ══════════ -->
      <div v-if="step === 1" class="step-content">
        <h2 class="step-title">Chọn Phim & Suất Chiếu</h2>

        <!-- Movie selection area -->
        <div class="movie-selector-wrapper" :class="{ 'has-selected': selectedPhim }">
          <div class="search-box glass-panel mb-4" v-if="!selectedPhim">
            <i class="fas fa-search search-icon"></i>
            <input type="text" v-model="search" class="search-input" placeholder="Tìm kiếm phim bạn yêu thích...">
          </div>

          <div class="phim-scroll-container">
            <div class="phim-grid" :class="{ 'is-compact': selectedPhim }">
              <div v-for="p in filteredPhims" :key="p.maPhim"
                class="phim-card"
                :class="{ 'selected': selectedPhim?.maPhim === p.maPhim, 'dimmed': selectedPhim && selectedPhim.maPhim !== p.maPhim }"
                @click="selectPhim(p)">
                <div class="poster-wrapper">
                  <img :src="p.poster" :alt="p.tenPhim" class="phim-poster" @error="fallback">
                  <div class="poster-overlay">
                    <button class="btn-select">CHỌN PHIM</button>
                  </div>
                  <div class="check-badge" v-if="selectedPhim?.maPhim === p.maPhim">
                    <i class="fas fa-check"></i>
                  </div>
                </div>
                <div class="phim-info">
                  <h3 class="phim-name">{{ p.tenPhim }}</h3>
                  <div class="phim-tags">
                    <span class="tag">{{ p.theLoai }}</span>
                    <span class="tag limit">18+</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Suất chiếu của phim đã chọn -->
        <transition name="slide-up">
          <div v-if="selectedPhim" class="showtime-selection mt-5">
            <div class="selection-header glass-panel p-4">
              <div class="header-main">
                <div class="movie-mini-info">
                  <span class="text-muted">Đang chọn vé cho:</span>
                  <h3>{{ selectedPhim.tenPhim }}</h3>
                </div>
                <div class="date-selector">
                  <div class="date-scroll">
                    <button v-for="d in dateOptions" :key="d.val"
                      class="date-pill" :class="{ active: selectedDate === d.val }"
                      @click="selectedDate = d.val">
                      <span class="pill-day">{{ d.dayName }}</span>
                      <span class="pill-date">{{ d.display }}</span>
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <div class="showtime-groups mt-4">
              <div v-if="loadingSuat" class="loading-pulse">
                <div class="skeleton-row" v-for="i in 2" :key="i"></div>
              </div>

              <div v-else-if="suatByFormat.length > 0">
                <div class="format-group mb-4" v-for="grp in suatByFormat" :key="grp.format">
                  <div class="format-header">
                    <span class="format-badge">{{ grp.format }}</span>
                    <div class="format-line"></div>
                  </div>
                  <div class="times-grid">
                    <div v-for="s in grp.suats" :key="s.maSuatChieu" 
                      class="time-card"
                      :class="{ active: selectedSuat?.maSuatChieu === s.maSuatChieu }"
                      @click="selectSuat(s)">
                      <div class="time-main">
                        <span class="hour">{{ formatTime(s.thoiGianBatDau) }}</span>
                        <span class="room">{{ s.tenPhong }}</span>
                      </div>
                      <div class="time-footer">
                        <span class="price">{{ s.giaVeGoc.toLocaleString() }}đ</span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <div v-else class="empty-showtimes glass-panel">
                <i class="far fa-calendar-times icon-empty"></i>
                <p>Tiếc quá! Hiện chưa có suất chiếu cho ngày này.</p>
                <button class="btn btn-outline btn-sm mt-3" @click="selectedDate = dateOptions[0].val">Quay lại hôm nay</button>
              </div>
            </div>

            <div class="fixed-bottom-bar" v-if="selectedSuat">
              <div class="container bar-content">
                <div class="selection-summary">
                  <span class="label">Suất đã chọn:</span>
                  <span class="value">{{ formatTime(selectedSuat.thoiGianBatDau) }} - {{ selectedSuat.dinhDang }} - {{ selectedSuat.tenPhong }}</span>
                </div>
                <button class="btn btn-primary pulse-effect" @click="step = 2">
                  TIẾP TỤC ĐẶT GHẾ <i class="fas fa-arrow-right ms-2"></i>
                </button>
              </div>
            </div>
          </div>
        </transition>
      </div>

      <!-- ══════════ STEP 2: Sơ Đồ Ghế ══════════ -->
      <div v-if="step === 2" class="step-content">
        <button class="btn btn-outline mb-4" @click="step = 1">← Quay Lại</button>
        <h2 class="step-title">Chọn Ghế Ngồi</h2>

        <!-- Info banner -->
        <div class="suat-info-bar glass-panel p-3 mb-4">
          <span>🎬 <strong>{{ selectedPhim?.tenPhim }}</strong></span>
          <span class="sep">|</span>
          <span>🏠 {{ selectedSuat?.tenPhong }}</span>
          <span class="sep">|</span>
          <span>⏰ {{ formatTime(selectedSuat?.thoiGianBatDau) }} — {{ formatDate(selectedSuat?.thoiGianBatDau) }}</span>
          <span class="sep">|</span>
          <span>📽 {{ selectedSuat?.dinhDang }}</span>
        </div>

        <!-- SEAT MAP COMPONENT -->
        <SeatPicker 
          :seats="ghes" 
          :selected-ids="pickedIds" 
          :loading="loadingGhe" 
          @toggle="toggleSeat" 
        />

        <div class="mt-4 text-right" v-if="pickedSeats.length > 0">
          <button class="btn btn-primary" @click="handleSeatConfirmation" :disabled="booking">
            <span v-if="booking"><i class="fas fa-spinner fa-spin me-2"></i> ĐANG KHÓA GHẾ...</span>
            <span v-else>Chọn Bắp Nước →</span>
          </button>
        </div>
      </div>

      <!-- ══════════ STEP 2.5: Bắp Nước ══════════ -->
      <div v-if="step === 2.5" class="step-content animate-fade-in">
        <button class="btn btn-outline border-0 mb-4 px-0" @click="step = 2">
          <i class="fas fa-chevron-left me-2"></i> QUAY LẠI CHỌN GHẾ
        </button>
        <h2 class="step-title mb-2">Chọn Thức Ăn & Đồ Uống</h2>
        <p class="text-muted mb-4 subtitle-step">Nạp năng lượng cho trải nghiệm điện ảnh tuyệt vời hơn! (Có thể bỏ qua nếu muốn)</p>

        <div v-if="loadingConcessions" class="loading-state glass-panel p-5">
           <div class="spinner-large"></div>
           <p>Đang lấy thực đơn bắp nước...</p>
        </div>

        <div v-else>
          <!-- Category Tabs -->
          <div class="category-tabs mb-5">
            <button class="cat-pill" :class="{ active: selectedCategory === 'Tất cả' }" @click="selectedCategory = 'Tất cả'">
               <i class="fas fa-th-large me-2"></i> Tất cả
            </button>
            <button class="cat-pill" :class="{ active: selectedCategory === 'Bắp' }" @click="selectedCategory = 'Bắp'">
               <i class="fas fa-popcorn me-2"></i> Bắp Rang
            </button>
            <button class="cat-pill" :class="{ active: selectedCategory === 'Nước' }" @click="selectedCategory = 'Nước'">
               <i class="fas fa-glass-whiskey me-2"></i> Nước Uống
            </button>
            <button class="cat-pill" :class="{ active: selectedCategory === 'Thức ăn' }" @click="selectedCategory = 'Thức ăn'">
               <i class="fas fa-pizza-slice me-2"></i> Thức Ăn Nhẹ
            </button>
            <button class="cat-pill" :class="{ active: selectedCategory === 'Combo' }" @click="selectedCategory = 'Combo'">
               <i class="fas fa-box-open me-2"></i> Combo Ưu Đãi
            </button>
          </div>

          <div class="concessions-grid">
            <div v-for="item in filteredConcessions" :key="item.maDoAnVat" class="concession-card glass-panel" :class="{ 'has-qty': concessionQuantities[item.maDoAnVat] > 0 }">
              <div class="concession-img">
                 <img :src="item.hinhAnh" :alt="item.tenMon" @error="concessionFallback">
                 <div class="concession-type">{{ item.loai }}</div>
                 <div class="selection-overlay" v-if="concessionQuantities[item.maDoAnVat] > 0">
                    <i class="fas fa-check-circle"></i>
                 </div>
              </div>
              <div class="concession-info">
                <div class="c-title-box">
                  <h3>{{ item.tenMon }}</h3>
                  <div class="c-price">{{ item.gia.toLocaleString() }}đ</div>
                </div>
                <p class="c-desc">{{ item.moTa }}</p>
                <div class="concession-footer">
                  <div class="quantity-control-v2">
                    <button class="q-btn-v2" @click="changeQty(item.maDoAnVat, -1)" :disabled="!concessionQuantities[item.maDoAnVat]">-</button>
                    <span class="q-val-v2">{{ concessionQuantities[item.maDoAnVat] || 0 }}</span>
                    <button class="q-btn-v2" @click="changeQty(item.maDoAnVat, 1)">+</button>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="booking-footer-concession">
             <div class="footer-summary-mini" v-if="selectedConcessionItems.length > 0">
                <span class="fs-label">Đã chọn {{ selectedConcessionItems.length }} món:</span>
                <span class="fs-total">{{ concessionSubtotal.toLocaleString() }}đ</span>
             </div>
             <div class="footer-summary-mini" v-else>
                <span class="fs-label">Duyệt qua thực đơn hoặc chọn bỏ qua:</span>
             </div>
             <div class="footer-actions">
                <button class="btn btn-skip" @click="step = 3">
                   BỎ QUA & THANH TOÁN
                </button>
                <button class="btn btn-primary btn-next-step" @click="step = 3">
                   {{ selectedConcessionItems.length > 0 ? 'XÁC NHẬN & TIẾP TỤC' : 'TIẾP TỤC THANH TOÁN' }} <i class="fas fa-arrow-right ms-2"></i>
                </button>
             </div>
          </div>
        </div>
      </div>

      <!-- ══════════ STEP 3: Thanh Toán ══════════ -->
      <div v-if="step === 3" class="step-content animate-fade-in">
        <button class="btn btn-outline border-0 mb-4 px-0" @click="step = 2">
          <i class="fas fa-chevron-left me-2"></i> QUAY LẠI CHỌN GHẾ
        </button>
        
        <h2 class="step-title mb-4">Hoàn Tất Đặt Vé</h2>

        <div class="checkout-grid">
          <!-- Left: Detailed Info -->
          <div class="checkout-main">
            <div class="ticket-summary-card glass-panel mb-4">
              <div class="movie-banner">
                <img :src="selectedPhim?.poster" class="banner-poster" @error="fallback">
                <div class="banner-info">
                  <span class="movie-format-tag">{{ selectedSuat?.dinhDang }}</span>
                  <h3 class="movie-title">{{ selectedPhim?.tenPhim }}</h3>
                  <div class="movie-meta">
                    <span class="meta-item"><i class="far fa-calendar-alt text-primary me-2"></i> {{ formatDate(selectedSuat?.thoiGianBatDau) }}</span>
                    <span class="meta-item"><i class="far fa-clock text-primary me-2"></i> {{ formatTime(selectedSuat?.thoiGianBatDau) }}</span>
                  </div>
                </div>
              </div>

              <div class="ticket-details p-4">
                <div class="detail-row mb-4">
                  <div class="detail-col">
                    <label class="detail-label">RẠP CHIẾU</label>
                    <div class="detail-value">ALPHA CINEMA</div>
                  </div>
                  <div class="detail-col">
                    <label class="detail-label">PHÒNG</label>
                    <div class="detail-value">{{ selectedSuat?.tenPhong }}</div>
                  </div>
                </div>

                <div class="detail-row mb-4">
                  <div class="detail-col w-100">
                    <label class="detail-label">DANH SÁCH GHẾ ({{ pickedSeats.length }} Ghế)</label>
                    <div class="seats-display">
                      <div v-for="g in pickedSeats" :key="g.maGhe" class="seat-badge">
                        <span class="s-name">{{ g.hang }}{{ g.soGhe }}</span>
                        <span class="s-type">{{ g.loaiGhe === 'VIP' ? 'VIP' : 'Thường' }}</span>
                      </div>
                    </div>
                  </div>
                </div>

                <div class="detail-row" v-if="selectedConcessionItems.length > 0">
                  <div class="detail-col w-100">
                    <label class="detail-label">BẮP NƯỚC ĐÃ CHỌN</label>
                    <div class="concessions-display mt-2">
                       <div v-for="c in selectedConcessionItems" :key="c.maDoAnVat" class="c-display-item">
                          <span class="c-name">{{ c.tenMon }} x{{ c.soLuong }}</span>
                          <span class="c-subtotal">{{ (c.gia * c.soLuong).toLocaleString() }}đ</span>
                       </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Promo area -->
            <div class="promo-container glass-panel p-4 mb-4">
              <label class="detail-label mb-3">MÃ KHUYẾN MÃI (NẾU CÓ)</label>
              <div class="promo-input-group">
                <input type="text" v-model="promoCode" class="form-control-custom" placeholder="Nhập mã ưu đãi của bạn...">
                <button class="btn btn-primary" @click="applyPromo" :disabled="promoApplied">Áp dụng</button>
              </div>
              <transition name="fade">
                <p v-if="promoMsg" class="promo-msg mt-3" :class="{ ok: promoApplied, 'gift-msg': freeGiftItem }">
                  <i class="fas" :class="freeGiftItem ? 'fa-gift' : (promoApplied ? 'fa-check-circle' : 'fa-info-circle')"></i> 
                  {{ promoMsg }}
                </p>
              </transition>
            </div>

            <!-- Payment Method -->
            <div class="payment-container glass-panel p-4">
              <label class="detail-label mb-3">PHƯƠNG THỨC THANH TOÁN</label>
              <div class="payment-options">
                <div v-for="m in payMethods" :key="m.id"
                  class="payment-option-card"
                  :class="{ active: payMethod === m.id }"
                  @click="payMethod = m.id">
                  <div class="option-check"></div>
                  <span class="option-name">{{ m.name }}</span>
                </div>
              </div>
            </div>

            <!-- QR Payment Modal -->
            <transition name="fade">
              <div v-if="showQR" class="qr-overlay">
                <div class="qr-modal glass-panel-heavy animate-zoom">
                  <button class="qr-close" @click="showQR = false">×</button>
                  <div class="qr-header">
                    <img src="https://vietqr.net/portal-v2/img/VietQR-Portal-Primary.png" alt="VietQR" class="qr-logo">
                    <h4>Quét mã để thanh toán</h4>
                    <p>Vé sẽ được xác nhận ngay sau khi giao dịch thành công</p>
                  </div>
                  
                  <div class="qr-code-wrapper">
                    <img :src="qrUrl" alt="Payment QR" class="qr-image">
                    <div class="qr-amount">
                      <span class="label">Số tiền cần trả</span>
                      <span class="value">{{ totalAfterDiscount.toLocaleString() }}đ</span>
                    </div>
                  </div>

                  <div class="qr-instructions">
                    <p>Mở ứng dụng Ngân hàng/Ví điện tử để quét mã VietQR</p>
                  </div>

                  <div class="qr-actions">
                    <button class="btn btn-primary w-100 mb-3" @click="handlePaymentSuccess" :disabled="booking">
                      <i class="fas fa-check-circle me-2"></i> GIẢ LẬP THANH TOÁN THÀNH CÔNG
                    </button>
                    <button class="btn btn-outline w-100" @click="showQR = false">Hủy</button>
                  </div>
                </div>
              </div>
            </transition>
          </div>

          <!-- Bill / Ticket Success Modal -->
          <transition name="fade">
            <div v-if="showBill" class="bill-overlay">
              <div class="ticket-bill glass-panel-heavy animate-zoom">
                <!-- Success Header -->
                <div class="bill-header">
                  <div class="success-icon">✓</div>
                  <h2 class="bill-status">THANH TOÁN THÀNH CÔNG!</h2>
                  <p class="bill-id">Mã hóa đơn: <span>#{{ bookedResult?.maHoaDon }}</span></p>
                  
                  <div class="success-qr-container mt-4 mb-4">
                    <div class="qr-bill-border">
                      <qrcode-vue :value="bookedResult?.maVaoCong" :size="180" level="H" background="#fff" foreground="#111" />
                    </div>
                  </div>

                  <div class="ticket-code-box animate-pulse-scale">
                    <span class="t-label-code">MÃ VÀO CỔNG (QUÉT 1 LẦN)</span>
                    <h3 class="t-code">{{ bookedResult?.maVaoCong }}</h3>
                  </div>
                </div>

                <!-- Ticket Body (Visual separation) -->
                <div class="ticket-visual">
                  <div class="ticket-left">
                    <img :src="selectedPhim?.poster" class="ticket-poster" @error="fallback">
                  </div>
                  <div class="ticket-right">
                    <div class="t-movie-title">{{ selectedPhim?.tenPhim }}</div>
                    <div class="t-format">{{ selectedSuat?.dinhDang }} - ALPHA CINEMA</div>
                    
                    <div class="t-grid mt-4">
                      <div class="t-item">
                        <span class="t-label">NGÀY</span>
                        <span class="t-val">{{ formatDate(selectedSuat?.thoiGianBatDau) }}</span>
                      </div>
                      <div class="t-item">
                        <span class="t-label">SUẤT CHIẾU</span>
                        <span class="t-val">{{ formatTime(selectedSuat?.thoiGianBatDau) }}</span>
                      </div>
                      <div class="t-item">
                        <span class="t-label">PHÒNG</span>
                        <span class="t-val">{{ selectedSuat?.tenPhong }}</span>
                      </div>
                      <div class="t-item">
                        <span class="t-label">LOẠI GHẾ</span>
                        <span class="t-val">{{ pickedSeats[0]?.loaiGhe === 'VIP' ? 'VIP' : 'Thường' }}</span>
                      </div>
                    </div>

                    <div class="t-seats-box mt-4">
                      <span class="t-label">DANH SÁCH GHẾ</span>
                      <div class="t-seats-vals">
                        <span v-for="g in pickedSeats" :key="g.maGhe" class="t-seat-v">{{ g.hang }}{{ g.soGhe }}</span>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Footer Summary -->
                <div class="bill-summary">
                  <div class="bs-row">
                    <span>Số lượng vé:</span>
                    <span>{{ pickedSeats.length }} Vé</span>
                  </div>
                  <div class="bs-row" v-if="selectedConcessionItems.length > 0">
                    <span>Bắp nước:</span>
                    <span>{{ selectedConcessionItems.length }} Món</span>
                  </div>
                  <div class="bs-row gift-row" v-if="freeGiftItem">
                    <span>🎁 Quà tặng ưu đãi:</span>
                    <span class="text-primary">{{ freeGiftItem.tenMon }} (x1)</span>
                  </div>
                  <div class="bs-row">
                    <span>Phương thức:</span>
                    <span>{{ payMethods.find(m => m.id === payMethod)?.name || payMethod }}</span>
                  </div>
                  <div class="bs-total mt-2">
                    <span>TỔNG CỘNG</span>
                    <span>{{ bookedResult?.tongTien?.toLocaleString() }}đ</span>
                  </div>
                </div>

                <div class="bill-footer mt-4">
                  <p class="bill-note">* Vui lòng cung cấp mã hóa đơn cho nhân viên rạp để nhận vé cứng.</p>
                  <button class="btn btn-primary w-100 mt-3" @click="router.push('/')">VỀ TRANG CHỦ</button>
                </div>
              </div>
            </div>
          </transition>

          <!-- Right: Summary Table -->
          <div class="checkout-sidebar">
            <div class="summary-card glass-panel p-4 sticky-top-custom">
              <h3 class="summary-title mb-4">Tổng Kết Đơn Hàng</h3>
              
              <div class="summary-items">
                <div class="summary-item" v-for="g in pickedSeats" :key="g.maGhe">
                  <div class="s-item-info">
                    <span class="s-item-name">Ghế {{ g.hang }}{{ g.soGhe }}</span>
                    <span class="s-item-desc">{{ g.loaiGhe === 'VIP' ? 'Ghế VIP' : 'Ghế Thường' }}</span>
                  </div>
                  <span class="s-item-price">{{ seatPrice(g).toLocaleString() }}đ</span>
                </div>

                <div class="summary-item concession-summary" v-for="c in selectedConcessionItems" :key="c.maDoAnVat">
                   <div class="s-item-info">
                    <span class="s-item-name">{{ c.tenMon }}</span>
                    <span class="s-item-desc">Số lượng: {{ c.soLuong }}</span>
                  </div>
                  <span class="s-item-price">{{ (c.gia * c.soLuong).toLocaleString() }}đ</span>
                </div>

                <!-- Free Gift Row -->
                <div class="summary-item free-gift-summary animate-in" v-if="freeGiftItem">
                  <div class="s-item-info">
                    <span class="s-item-name text-primary">🎁 {{ freeGiftItem.tenMon }}</span>
                    <span class="s-item-desc">Món quà từ mã ưu đãi</span>
                  </div>
                  <span class="s-item-price text-primary">MIỄN PHÍ</span>
                </div>
              </div>

              <div class="summary-divider my-4"></div>

              <div class="summary-totals">
                <div class="total-row">
                  <span>Tạm tính</span>
                  <span>{{ subtotal.toLocaleString() }}đ</span>
                </div>
                <div class="total-row discount" v-if="discount > 0 && !freeGiftItem">
                  <span>Ưu đãi áp dụng</span>
                  <span>-{{ discount.toLocaleString() }}đ</span>
                </div>
                <div class="total-row main-total mt-3">
                  <span class="text-primary">TỔNG CỘNG</span>
                  <span class="price-big">{{ totalAfterDiscount.toLocaleString() }}đ</span>
                </div>
              </div>

              <button class="btn btn-primary btn-booking-confirm mt-4 w-100" 
                :disabled="booking" @click="confirmBooking">
                <span v-if="booking"><i class="fas fa-spinner fa-spin me-2"></i> ĐANG XỬ LÝ...</span>
                <span v-else>🎟 XÁC NHẬN ĐẶT VÉ</span>
              </button>
              
              <div class="disclaimer mt-3">
                <i class="fas fa-shield-alt text-muted me-2"></i>
                <span>Giao dịch của bạn luôn được bảo mật</span>
              </div>
            </div>
          </div>
        </div>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAuthStore } from '../../stores/auth';
import { movieApi } from '../../api/movieApi';
import { bookingApi } from '../../api/bookingApi';
import { promotionApi } from '../../api/promotionApi';
import SeatPicker from '../../components/public/SeatPicker.vue';
import QrcodeVue from 'qrcode.vue';

const route  = useRoute();
const router = useRouter();
const auth   = useAuthStore();

// ─── Guards ───
if (!auth.isAuthenticated) {
  router.push('/login');
}

// ─── State ───
const step         = ref(1);
const search       = ref('');
const allPhims     = ref([]);
const loadingPhims = ref(true);
const selectedPhim = ref(null);

const allSuats     = ref([]);
const loadingSuat  = ref(false);
const selectedDate = ref('');
const selectedSuat = ref(null);

const ghes         = ref([]);
const loadingGhe   = ref(false);
const pickedSeats  = ref([]);   // array of Ghe objects

const promoCode    = ref('');
const promoMsg     = ref('');
const promoApplied = ref(false);
const discount     = ref(0);

const payMethod  = ref('VNPay');
const booking    = ref(false);

// Concessions state
const concessions = ref([]);
const loadingConcessions = ref(false);
const concessionQuantities = ref({});
const selectedCategory = ref('Tất cả');

const payMethods = [
  { id: 'VNPay',     name: 'Thanh toán qua VNPay' },
  { id: 'Momo',      name: 'Ví điện tử MoMo' },
  { id: 'VietQR',    name: 'Chuyển khoản VietQR' },
];

const showQR = ref(false);
const showBill = ref(false);
const qrUrl = ref('');
const bookedResult = ref(null);
const pendingHoaDonId = ref(null);
const freeGiftItem = ref(null); // { maDoAnVatTang, tenDoAnVatTang }

// ─── Seat Confirmation (Lock) ───
const handleSeatConfirmation = async () => {
    if (pickedSeats.value.length === 0) return;
    
    booking.value = true;
    try {
        const res = await bookingApi.lockSeats(selectedSuat.value.maSuatChieu, pickedIds.value);
        if (res.success) {
            pendingHoaDonId.value = res.data.maHoaDon;
            step.value = 2.5; // Ir a la selección de bắp nước
            loadConcessions();
        }
    } catch (e) {
        alert(e.message || 'Ghế đã có người chọn trước đó, vui lòng chọn ghế khác!');
        // Refresh seats
        const resGhe = await bookingApi.getShowtimeSeats(selectedSuat.value.maSuatChieu);
        if (resGhe.success) ghes.value = resGhe.data;
    } finally {
        booking.value = false;
    }
};

// ─── Date Options (today + next 6 days) ───
const dateOptions = computed(() => {
  const days = ['CN','T2','T3','T4','T5','T6','T7'];
  return Array.from({ length: 7 }, (_, i) => {
    const d = new Date();
    d.setDate(d.getDate() + i);
    return {
      val:     d.toISOString().split('T')[0],
      display: `${d.getDate()}/${d.getMonth()+1}`,
      dayName: i === 0 ? 'Hôm nay' : days[d.getDay()],
    };
  });
});

// ─── Load movies ───
onMounted(async () => {
  if (!auth.isAuthenticated) return;
  selectedDate.value = dateOptions.value[0].val;
  try {
    const res = await movieApi.getMovies();
    if (res.success) allPhims.value = res.data.filter(p => p.trangThaiPhim === 'Đang chiếu');
  } catch(e){ console.error(e); }
  finally { loadingPhims.value = false; }

  // Auto-select from query param
  if (route.query.phim) {
    const pid = parseInt(route.query.phim);
    const found = allPhims.value.find(p => p.maPhim === pid);
    if (found) selectPhim(found);
  }
});

const filteredPhims = computed(() => {
  if (!search.value) return allPhims.value;
  const q = search.value.toLowerCase();
  return allPhims.value.filter(p => p.tenPhim.toLowerCase().includes(q));
});

const fetchSuatChieus = async () => {
  if (!selectedPhim.value || !selectedDate.value) return;
  
  selectedSuat.value = null;
  loadingSuat.value  = true;
  try {
    // Luôn lấy suất chiếu theo đúng Ngày và Phim đã chọn
    const res = await bookingApi.getShowtimes({ 
      maPhim: selectedPhim.value.maPhim,
      ngay: selectedDate.value 
    });
    if (res.success) allSuats.value = res.data;
  } catch(e){ console.error(e); }
  finally { loadingSuat.value = false; }
};

const selectPhim = async (p) => {
  selectedPhim.value = p;
  await fetchSuatChieus();
};

// ─── Suất theo định dạng (2D, 3D, ...) ───
const suatByFormat = computed(() => {
  const filtered = allSuats.value.filter(s => {
    const d = s.thoiGianBatDau.split('T')[0];
    return d === selectedDate.value;
  });

  const map = {};
  filtered.forEach(s => {
    const k = s.dinhDang || "DIGITAL 2D";
    if (!map[k]) map[k] = { format: k, suats: [] };
    map[k].suats.push(s);
  });

  return Object.values(map).map(g => ({
    ...g,
    suats: g.suats.sort((a,b) => new Date(a.thoiGianBatDau) - new Date(b.thoiGianBatDau))
  }));
});

const selectSuat = async (s) => {
  selectedSuat.value = s;
  // Gỡ bỏ tự động chuyển step để người dùng chủ động nhấn nút "Tiếp tục" ở thanh dưới
};

// ─── Watch date change ───
watch(selectedDate, () => { 
  fetchSuatChieus();
});

// ─── Step 2: Load seats ───
watch(step, async (v) => {
  if (v === 2 && selectedSuat.value) {
    pickedSeats.value = [];
    loadingGhe.value  = true;
    try {
      const res = await bookingApi.getShowtimeSeats(selectedSuat.value.maSuatChieu);
      if (res.success) ghes.value = res.data;
    } catch(e){ console.error(e); }
    finally { loadingGhe.value = false; }
  }
});

// ─── Seat map: group by row ───
const pickedIds = computed(() => pickedSeats.value.map(g => g.maGhe));

const filteredConcessions = computed(() => {
    if (selectedCategory.value === 'Tất cả') return concessions.value;
    return concessions.value.filter(c => c.loai === selectedCategory.value);
});

const loadConcessions = async () => {
    loadingConcessions.value = true;
    try {
        const res = await bookingApi.getConcessions();
        if (res.success) {
            concessions.value = res.data;
            // Initialize quantities if not set
            res.data.forEach(c => {
                if (concessionQuantities.value[c.maDoAnVat] === undefined) {
                    concessionQuantities.value[c.maDoAnVat] = 0;
                }
            });
        }
    } catch(e) { console.error(e); }
    finally { loadingConcessions.value = false; }
};

const changeQty = (id, delta) => {
    const current = concessionQuantities.value[id] || 0;
    const next = Math.max(0, current + delta);
    if (next > 10) return;
    concessionQuantities.value[id] = next;
};

const selectedConcessionItems = computed(() => {
    return concessions.value
        .filter(c => concessionQuantities.value[c.maDoAnVat] > 0)
        .map(c => ({
            ...c,
            soLuong: concessionQuantities.value[c.maDoAnVat]
        }));
});

const concessionSubtotal = computed(() => {
    return selectedConcessionItems.value.reduce((total, item) => total + (item.gia * item.soLuong), 0);
});

const toggleSeat = (ghe) => {
  if (ghe.daDat) return;
  const idx = pickedIds.value.indexOf(ghe.maGhe);
  if (idx >= 0) {
    pickedSeats.value.splice(idx, 1);
  } else {
    if (pickedSeats.value.length >= 8) { alert('Tối đa 8 ghế mỗi lần đặt!'); return; }
    pickedSeats.value.push(ghe);
  }
};

// ─── Pricing ───
const basePrice = computed(() => selectedSuat.value?.giaVeGoc || 0);
const seatPrice = (g) => g.loaiGhe === 'VIP' ? basePrice.value * 1.5 : basePrice.value;
const subtotal  = computed(() => {
    const seatTotal = pickedSeats.value.reduce((s, g) => s + seatPrice(g), 0);
    return seatTotal + concessionSubtotal.value;
});
const totalAfterDiscount = computed(() => Math.max(0, subtotal.value - discount.value));

// ─── Promo ───
const applyPromo = async () => {
  if (!promoCode.value) return;
  if (subtotal.value === 0) { promoMsg.value = 'Vui lòng chọn ghế trước!'; return; }
  try {
    const res = await promotionApi.applyPromo(promoCode.value, subtotal.value);
    if (res.success) {
      discount.value  = res.data.tienGiam;
      freeGiftItem.value = res.data.maDoAnVatTang ? { 
        maDoAnVat: res.data.maDoAnVatTang, 
        tenMon: res.data.tenDoAnVatTang 
      } : null;

      if (freeGiftItem.value) {
        promoMsg.value = `✓ Nhận ngay: ${res.data.tenDoAnVatTang} (Món quà tặng kèm)`;
      } else {
        promoMsg.value  = `✓ Giảm ${res.data.tienGiam.toLocaleString()}đ — ${res.data.tenKhuyenMai}`;
      }
      promoApplied.value = true;
    }
  } catch(e) {
    promoMsg.value  = '✗ Mã không hợp lệ hoặc đã hết hạn.';
    discount.value  = 0;
    promoApplied.value = false;
  }
};

// ─── Confirm booking & show QR ───
const confirmBooking = () => {
  if (!payMethod.value) { alert('Vui lòng chọn phương thức thanh toán!'); return; }
  
  // Generate VietQR URL (example info: MB Bank, Name: Alpha Cinema)
  const bankId = 'MB'; 
  const accountNo = '0905595408'; // Using number from user image
  const accountName = 'TRAN VIET TAM PHUC';
  const amount = totalAfterDiscount.value;
  const info = encodeURIComponent(`ALPHACINEMA ${selectedPhim.value.tenPhim} ${pickedSeats.value.length}VE`);
  
  qrUrl.value = `https://img.vietqr.io/image/${bankId}-${accountNo}-compact2.png?amount=${amount}&addInfo=${info}&accountName=${encodeURIComponent(accountName)}`;
  showQR.value = true;
};

const handlePaymentSuccess = async () => {
  booking.value = true;
  try {
    const res = await bookingApi.confirmBooking({
      maHoaDon: pendingHoaDonId.value,
      maSuatChieu: selectedSuat.value.maSuatChieu,
      maGheIds: pickedIds.value,
      maCodeGiamGia: promoApplied.value ? promoCode.value : null,
      phuongThucThanhToan: payMethod.value,
      concessions: [
          ...selectedConcessionItems.value.map(c => ({
              maDoAnVat: c.maDoAnVat,
              soLuong: c.soLuong
          }))
      ]
    });
    if (res.success) {
      showQR.value = false;
      bookedResult.value = res.data;
      showBill.value = true;
      // alert(`🎉 Thanh toán thành công!\nMã hóa đơn: #${res.data.maHoaDon}\nTổng tiền: ${res.data.tongTien?.toLocaleString()}đ`);
      // router.push('/');
    }
    } catch(e) {
      console.error("Booking Error:", e);
      const msg = typeof e === 'string' ? e : (e.message || 'Lỗi hệ thống');
      alert(`❌ LỖI THANH TOÁN:\n\n${msg}\n\nVui lòng báo nhân viên hoặc thử lại.`);
    } finally {
      booking.value = false;
    }
};

// ─── Helpers ───
const concessionFallback = (e) => { e.target.src = 'https://placehold.co/200x200/1a1a1a/555?text=Food'; };
const fallback   = (e) => { e.target.src = 'https://placehold.co/400x600/1a1a1a/555?text=No+Poster'; };
const formatTime = (dt) => dt ? new Date(dt).toLocaleTimeString('vi-VN', { hour:'2-digit', minute:'2-digit', hour12: false }) : '';
const formatDate = (dt) => dt ? new Date(dt).toLocaleDateString('vi-VN', { weekday:'long', day:'numeric', month:'numeric' }) : '';
</script>

<style scoped>
.booking-page { min-height: 100vh; padding-bottom: 8rem; }

/* ─── STEP BAR ─── */
.step-bar {
  background: rgba(0,0,0,0.2);
  border-bottom: 1px solid rgba(255,255,255,0.05);
  padding: 3rem 0;
  margin-bottom: 1rem;
  z-index: 10;
}
.steps { display: flex; align-items: center; justify-content: center; gap: 0.5rem; }
.step { 
  display: flex; 
  flex-direction: column;
  align-items: center; 
  gap: 0.8rem; 
  font-size: 0.85rem; 
  font-weight: 700; 
  color: #444; 
  transition: all 0.5s cubic-bezier(0.4, 0, 0.2, 1);
  width: 120px;
  position: relative;
}
.step.active { color: white; transform: scale(1.1); }
.step.done { color: var(--color-success); }

.step-circle {
  width: 40px; height: 40px; border-radius: 50%; border: 2px solid #222;
  display: flex; align-items: center; justify-content: center; font-size: 1rem;
  background: #0f0f0f;
  transition: all 0.5s cubic-bezier(0.4, 0, 0.2, 1);
  z-index: 2;
}
.step.active .step-circle { 
  border-color: var(--color-primary); 
  color: var(--color-primary); 
  box-shadow: 0 0 20px rgba(232, 136, 42, 0.4);
  transform: translateY(-5px);
}
.step.done .step-circle { 
  background: var(--color-success); 
  border-color: var(--color-success); 
  color: black; 
  transform: scale(0.9);
}

.step-line { 
  width: 80px; 
  height: 2px; 
  background: #1a1a1a; 
  margin-top: -10px; /* Cân bằng với text bên dưới vòng tròn */
  border-radius: 10px; 
  position: relative;
  overflow: hidden;
  align-self: center;
}
.step-line::after {
  content: '';
  position: absolute;
  left: 0; top: 0; bottom: 0;
  width: 0%;
  background: linear-gradient(90deg, #e8882a, #00e676);
  transition: width 0.8s cubic-bezier(0.65, 0, 0.35, 1);
}
.step-line.done::after { width: 100%; }

/* ─── MOVIE SELECTOR ─── */
.movie-selector-wrapper { transition: all 0.5s cubic-bezier(0.4, 0, 0.2, 1); }

.search-box { display: flex; align-items: center; padding: 0.5rem 1.5rem; border-radius: 20px; border: 1px solid rgba(255,255,255,0.05); }
.search-icon { color: #555; margin-right: 1rem; }
.search-input { background: none; border: none; font-size: 1rem; color: white; width: 100%; outline: none; padding: 0.6rem 0; }

.phim-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(200px, 1fr)); gap: 2rem; transition: all 0.5s; }
.phim-grid.is-compact { display: flex; overflow-x: auto; padding-bottom: 1rem; scrollbar-width: none; gap: 1.5rem; }
.phim-grid.is-compact .phim-card { flex: 0 0 160px; }
.phim-grid.is-compact .phim-info { display: none; }

.phim-card { cursor: pointer; transition: 0.4s; position: relative; }
.phim-card.dimmed { opacity: 0.4; transform: scale(0.95); }
.phim-card.selected { opacity: 1; transform: scale(1.05); }

.poster-wrapper { position: relative; border-radius: 16px; overflow: hidden; box-shadow: 0 10px 30px rgba(0,0,0,0.5); border: 2px solid transparent; }
.phim-card.selected .poster-wrapper { border-color: var(--color-primary); box-shadow: 0 0 20px rgba(232, 136, 42, 0.4); }

.phim-poster { width: 100%; aspect-ratio: 2/3; object-fit: cover; }
.poster-overlay { position: absolute; inset: 0; background: linear-gradient(to top, rgba(0,0,0,0.9), transparent); opacity: 0; display: flex; align-items: center; justify-content: center; transition: 0.3s; }
.phim-card:hover .poster-overlay { opacity: 1; }
.btn-select { background: var(--color-primary); color: white; padding: 0.6rem 1.2rem; border-radius: 50px; border: none; font-weight: 800; font-size: 0.75rem; }

.check-badge { position: absolute; top: 12px; right: 12px; background: var(--color-primary); color: white; width: 28px; height: 28px; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 0.8rem; box-shadow: 0 4px 10px rgba(0,0,0,0.3); }

.phim-info { margin-top: 1rem; }
.phim-name { font-size: 1rem; font-weight: 800; line-height: 1.4; margin-bottom: 0.5rem; height: 2.8rem; overflow: hidden; display: -webkit-box; -webkit-line-clamp: 2; line-clamp: 2; -webkit-box-orient: vertical; }
.phim-tags { display: flex; gap: 0.5rem; }
.tag { font-size: 0.7rem; font-weight: 700; background: rgba(255,255,255,0.05); padding: 2px 8px; border-radius: 4px; color: #888; }
.tag.limit { color: #ff3366; background: rgba(255, 51, 102, 0.1); }

/* ─── SHOWTIME SELECTION ─── */
.selection-header { border-radius: 20px; border-left: 5px solid var(--color-primary); }
.header-main { display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 2rem; }

.date-scroll { display: flex; gap: 0.8rem; overflow-x: auto; padding: 0.5rem 0; scrollbar-width: none; }
.date-pill { background: rgba(255,255,255,0.03); border: 1px solid rgba(255,255,255,0.08); padding: 0.6rem 1.2rem; border-radius: 14px; color: white; cursor: pointer; transition: 0.3s; display: flex; flex-direction: column; align-items: center; min-width: 75px; }
.date-pill:hover { border-color: rgba(255,255,255,0.2); }
.date-pill.active { background: var(--color-primary); border-color: var(--color-primary); color: white; box-shadow: 0 8px 15px rgba(232, 136, 42, 0.3); }
.pill-day { font-size: 0.7rem; text-transform: uppercase; font-weight: 700; opacity: 0.7; }
.pill-date { font-size: 1.1rem; font-weight: 800; }

.format-group { margin-bottom: 2.5rem; }
.format-header { display: flex; align-items: center; gap: 1rem; margin-bottom: 1.2rem; }
.format-badge { background: rgba(255,255,255,0.08); padding: 0.4rem 1rem; border-radius: 8px; font-weight: 800; font-size: 0.85rem; color: #ccc; letter-spacing: 1px; }
.format-line { flex: 1; height: 1px; background: linear-gradient(to right, rgba(255,255,255,0.1), transparent); }

.times-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(130px, 1fr)); gap: 1rem; }
.time-card { background: rgba(255,255,255,0.03); border: 1px solid rgba(255,255,255,0.06); padding: 1rem; border-radius: 12px; cursor: pointer; transition: 0.3s; text-align: center; }
.time-card:hover { border-color: var(--color-primary); transform: translateY(-3px); background: rgba(255,255,255,0.06); }
.time-card.active { border-color: var(--color-primary); background: rgba(232, 136, 42, 0.1); box-shadow: inset 0 0 10px rgba(232, 136, 42, 0.1); }

.time-main { display: flex; flex-direction: column; }
.time-main .hour { font-size: 1.25rem; font-weight: 800; color: white; }
.time-main .room { font-size: 0.7rem; color: #666; font-weight: 700; margin-bottom: 0.4rem; }
.time-footer .price { font-size: 0.85rem; font-weight: 800; color: #00e676; }

.empty-showtimes { padding: 4rem; text-align: center; color: #555; border-radius: 24px; }
.icon-empty { font-size: 3rem; margin-bottom: 1.5rem; opacity: 0.3; }

/* ─── FIXED BOTTOM BAR ─── */
.fixed-bottom-bar {
  position: fixed; bottom: 0; left: 0; width: 100%;
  background: rgba(15,15,20,0.95); backdrop-filter: blur(20px);
  border-top: 1px solid rgba(255,255,255,0.08); padding: 1.5rem 0;
  z-index: 1000; animation: slideUp 0.4s ease-out;
}
@keyframes slideUp { from { transform: translateY(100%); } to { transform: translateY(0); } }

.bar-content { display: flex; justify-content: space-between; align-items: center; }
.selection-summary { display: flex; flex-direction: column; }
.selection-summary .label { font-size: 0.8rem; color: #777; font-weight: 700; text-transform: uppercase; }
.selection-summary .value { font-size: 1.1rem; font-weight: 800; color: white; }

.pulse-effect { animation: pulse 2s infinite; padding: 1rem 2.5rem; font-weight: 800; border-radius: 12px; box-shadow: 0 4px 15px rgba(232, 136, 42, 0.3); }
@keyframes pulse { 0% { transform: scale(1); } 50% { transform: scale(1.03); } 100% { transform: scale(1); } }

/* ─── Transitions ─── */
.slide-up-enter-active, .slide-up-leave-active { transition: all 0.5s ease; }
.slide-up-enter-from { opacity: 0; transform: translateY(30px); }
.slide-up-leave-to { opacity: 0; transform: translateY(-30px); }

/* ─── STEP 3: Checkout Revamp ─── */
.checkout-grid { display: grid; grid-template-columns: 1fr 380px; gap: 2.5rem; align-items: flex-start; margin-top: 1rem; }

.ticket-summary-card { border-radius: 24px; overflow: hidden; border: 1px solid rgba(255,255,255,0.08); background: rgba(255,255,255,0.02); }
.movie-banner { display: flex; gap: 2rem; padding: 2.5rem; background: linear-gradient(135deg, rgba(232,136,42,0.1), rgba(0,0,0,0.3)); border-bottom: 1px solid rgba(255,255,255,0.05); }
.banner-poster { width: 120px; aspect-ratio: 2/3; object-fit: cover; border-radius: 12px; box-shadow: 0 10px 25px rgba(0,0,0,0.4); }
.banner-info { flex:1; display:flex; flex-direction:column; justify-content:center; }
.movie-format-tag { background: var(--color-primary); color: white; padding: 2px 10px; border-radius: 4px; font-size: 0.72rem; font-weight: 800; width: fit-content; margin-bottom: 0.8rem; letter-spacing: 1px; }
.movie-title { font-size: 1.8rem; font-weight: 900; margin-bottom: 1rem; color: white; line-height: 1.2; }
.movie-meta { display: flex; gap: 1.5rem; }
.meta-item { font-size: 0.9rem; font-weight: 600; display: flex; align-items: center; color: #aaa; }

.detail-row { display: flex; gap: 2rem; }
.detail-col { flex: 1; }
.detail-label { font-size: 0.72rem; font-weight: 800; color: #555; letter-spacing: 1.5px; text-transform: uppercase; display: block; }
.detail-value { font-size: 1.15rem; font-weight: 700; color: #eee; margin-top: 0.4rem; }

.seats-display { display: flex; flex-wrap: wrap; gap: 0.8rem; margin-top: 1rem; }
.seat-badge { background: rgba(255,255,255,0.04); border: 1px solid rgba(255,255,255,0.1); border-radius: 12px; padding: 0.8rem 1.2rem; display: flex; flex-direction: column; align-items: center; min-width: 80px; transition: 0.3s; }
.seat-badge:hover { border-color: var(--color-primary); background: rgba(232,136,42,0.05); transform: translateY(-2px); }
.s-name { font-size: 1.2rem; font-weight: 800; color: white; }
.s-type { font-size: 0.68rem; font-weight: 700; color: var(--color-primary); text-transform: uppercase; margin-top: 2px; opacity: 0.8; }

/* Promo Input */
.promo-container { border-radius: 20px; }
.promo-input-group { display: flex; gap: 1rem; }
.form-control-custom { flex: 1; background: rgba(0,0,0,0.25); border: 1px solid rgba(255,255,255,0.1); border-radius: 12px; padding: 1rem 1.5rem; color: white; outline: none; transition: 0.3s; font-size: 0.95rem; }
.form-control-custom:focus { border-color: var(--color-primary); box-shadow: 0 0 15px rgba(232,136,42,0.1); }

/* Payment Options */
.payment-container { border-radius: 20px; }
.payment-options { display: grid; grid-template-columns: repeat(auto-fill, minmax(220px, 1fr)); gap: 1rem; }
.payment-option-card { background: rgba(255,255,255,0.03); border: 1.5px solid rgba(255,255,255,0.08); padding: 1.2rem 1.5rem; border-radius: 16px; cursor: pointer; transition: 0.3s; display: flex; align-items: center; gap: 1.2rem; }
.payment-option-card:hover { background: rgba(255,255,255,0.06); border-color: rgba(255,255,255,0.2); }
.payment-option-card.active { border-color: var(--color-primary); background: rgba(232,136,42,0.1); box-shadow: 0 8px 20px rgba(0,0,0,0.2); }
.option-check { width: 20px; height: 20px; border-radius: 50%; border: 2px solid rgba(255,255,255,0.2); position: relative; flex-shrink: 0; }
.payment-option-card.active .option-check { border-color: var(--color-primary); }
.payment-option-card.active .option-check::after { content: ''; position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); width: 10px; height: 10px; background: var(--color-primary); border-radius: 50%; box-shadow: 0 0 8px var(--color-primary); }
.option-name { font-weight: 700; font-size: 0.95rem; color: #888; }
.payment-option-card.active .option-name { color: white; }

/* Sidebar Summary */
.checkout-sidebar { position: sticky; top: 150px; }
.summary-card { border-radius: 24px; border: 1px solid rgba(255,255,255,0.05); }
.summary-title { font-size: 1.4rem; font-weight: 900; color: white; }
.summary-item { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.2rem; }
.s-item-info { display: flex; flex-direction: column; }
.s-item-name { font-size: 0.95rem; font-weight: 700; color: white; }
.s-item-desc { font-size: 0.75rem; color: #555; font-weight: 600; }
.s-item-price { font-size: 0.95rem; font-weight: 700; color: #bbb; }

.summary-divider { height: 1px; background: linear-gradient(90deg, transparent, rgba(255,255,255,0.1), transparent); }
.total-row { display: flex; justify-content: space-between; margin-bottom: 0.8rem; font-size: 0.95rem; color: #777; font-weight: 600; }
.total-row.discount { color: #00e676; }
.main-total { font-weight: 900; color: white; padding-top: 1.2rem; border-top: 1px solid rgba(255,255,255,0.08); }
.price-big { font-size: 2rem; color: var(--color-primary); text-shadow: 0 0 25px rgba(232, 136, 42, 0.3); }

.btn-booking-confirm { padding: 1.2rem; font-size: 1.1rem; font-weight: 900; letter-spacing: 1px; border-radius: 16px; transition: 0.4s; }
.btn-booking-confirm:hover { transform: translateY(-3px); box-shadow: 0 12px 30px rgba(232, 136, 42, 0.4); }
.disclaimer { display: flex; align-items: center; justify-content: center; font-size: 0.75rem; color: #444; font-weight: 600; }

@media (max-width: 1100px) {
  .checkout-grid { grid-template-columns: 1fr; }
  .checkout-sidebar { position: static; }
}

/* ─── Concessions CSS Revamp ─── */
.subtitle-step { font-size: 0.95rem; font-weight: 500; opacity: 0.6; }

.category-tabs {
  display: flex;
  gap: 1.2rem;
  overflow-x: auto;
  padding: 0.5rem 0.2rem;
  scrollbar-width: none;
  -ms-overflow-style: none;
}
.category-tabs::-webkit-scrollbar { display: none; }

.cat-pill {
  background: rgba(255,255,255,0.02);
  border: 1.5px solid rgba(255,255,255,0.08);
  padding: 0.9rem 1.8rem;
  border-radius: 50px;
  color: #777;
  font-weight: 700;
  cursor: pointer;
  white-space: nowrap;
  transition: 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  font-size: 0.9rem;
  display: flex;
  align-items: center;
}

.cat-pill i { font-size: 1.1rem; }

.cat-pill:hover {
  background: rgba(255,255,255,0.05);
  border-color: rgba(255,255,255,0.2);
  color: #bbb;
}

.cat-pill.active {
  background: linear-gradient(135deg, var(--color-primary), #ff9800);
  border-color: transparent;
  color: white;
  box-shadow: 0 8px 25px rgba(232, 136, 42, 0.5);
  transform: translateY(-2px);
}

.concessions-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(340px, 1fr));
  gap: 2.5rem;
  margin-top: 1rem;
}

.concession-card {
  display: flex;
  flex-direction: row;
  height: 190px;
  border-radius: 24px;
  overflow: hidden;
  transition: 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  border: 1px solid rgba(255,255,255,0.05);
  background: rgba(255,255,255,0.01);
  position: relative;
}

.concession-card.has-qty {
  border-color: var(--color-primary);
  background: rgba(232, 136, 42, 0.05);
  box-shadow: 0 15px 40px rgba(0,0,0,0.4);
}

.concession-card:hover {
  transform: translateY(-8px) scale(1.02);
  border-color: rgba(255,255,255,0.15);
  background: rgba(255,255,255,0.03);
}

.concession-img {
  position: relative;
  width: 150px;
  height: 100%;
  overflow: hidden;
  flex-shrink: 0;
}

.concession-img img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: 1s cubic-bezier(0.4, 0, 0.2, 1);
}

.concession-card:hover .concession-img img {
  transform: scale(1.2);
}

.selection-overlay {
  position: absolute; inset: 0;
  background: rgba(232, 136, 42, 0.5);
  backdrop-filter: blur(2px);
  display: flex; align-items: center; justify-content: center;
  font-size: 3rem; color: white;
  animation: fadeIn 0.3s;
}

@keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }

.concession-type {
  position: absolute;
  top: 12px;
  left: 12px;
  background: rgba(0,0,0,0.7);
  backdrop-filter: blur(8px);
  color: white;
  padding: 3px 10px;
  border-radius: 8px;
  font-size: 0.65rem;
  font-weight: 800;
  text-transform: uppercase;
  z-index: 2;
  border: 1px solid rgba(255,255,255,0.1);
}

.concession-info {
  padding: 1.5rem;
  flex: 1;
  display: flex;
  flex-direction: column;
}

.c-title-box { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 0.6rem; gap: 0.5rem; }
.c-title-box h3 { font-size: 1.15rem; font-weight: 800; color: white; line-height: 1.2; letter-spacing: -0.5px; }
.c-price { font-size: 1.2rem; font-weight: 900; color: #00e676; text-shadow: 0 0 15px rgba(0,230,118,0.2); }

.c-desc {
  font-size: 0.85rem;
  color: #777;
  line-height: 1.5;
  margin-bottom: 1.2rem;
  overflow: hidden;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
  flex: 1;
}

.concession-footer {
  display: flex;
  justify-content: flex-end;
  padding-top: 0.8rem;
}

.quantity-control-v2 {
  display: flex;
  align-items: center;
  gap: 1.2rem;
  background: rgba(0,0,0,0.4);
  padding: 5px;
  border-radius: 14px;
  border: 1px solid rgba(255,255,255,0.1);
  box-shadow: inset 0 2px 5px rgba(0,0,0,0.3);
}

.q-btn-v2 {
  width: 30px;
  height: 30px;
  border-radius: 10px;
  border: none;
  background: rgba(255,255,255,0.05);
  color: white;
  font-size: 1.2rem;
  font-weight: 700;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: 0.2s;
}

.q-btn-v2:disabled { opacity: 0.2; cursor: default; }
.q-btn-v2:not(:disabled):hover { background: var(--color-primary); transform: scale(1.1); }
.q-btn-v2:active { transform: scale(0.9); }

.q-val-v2 { font-size: 1.1rem; font-weight: 800; color: white; min-width: 24px; text-align: center; }

/* Booking Footer Concession */
.booking-footer-concession {
  margin-top: 6rem;
  padding: 3rem;
  background: linear-gradient(135deg, rgba(255,255,255,0.03), rgba(255,255,255,0.01));
  border: 1px solid rgba(255,255,255,0.08);
  border-radius: 32px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 2rem;
  box-shadow: 0 20px 50px rgba(0,0,0,0.3);
}

.footer-summary-mini { display: flex; flex-direction: column; }
.fs-label { font-size: 0.9rem; color: #555; font-weight: 800; text-transform: uppercase; margin-bottom: 0.5rem; letter-spacing: 1px; }
.fs-total { font-size: 2.2rem; font-weight: 950; color: var(--color-primary); text-shadow: 0 0 30px rgba(232,136,42,0.3); }

.footer-actions { display: flex; gap: 1.5rem; }
.btn-skip { background: transparent; color: #666; border: 2px solid rgba(255,255,255,0.1); padding: 1.1rem 2.2rem; border-radius: 18px; font-weight: 800; font-size: 0.95rem; transition: 0.3s; }
.btn-skip:hover { background: rgba(255,255,255,0.05); color: #999; border-color: rgba(255,255,255,0.3); }
.btn-next-step { padding: 1.1rem 3.5rem; font-size: 1.15rem; font-weight: 900; border-radius: 18px; box-shadow: 0 12px 35px rgba(232, 136, 42, 0.4); text-transform: uppercase; letter-spacing: 1px; }
.btn-next-step:hover { transform: translateY(-3px); box-shadow: 0 15px 45px rgba(232, 136, 42, 0.5); }

/* Concession Display in Bill */
.concessions-display {
  display: flex;
  flex-direction: column;
  gap: 0.6rem;
}

.c-display-item {
  display: flex;
  justify-content: space-between;
  padding: 0.8rem 1.2rem;
  background: rgba(255,255,255,0.03);
  border-radius: 12px;
  border: 1px solid rgba(255,255,255,0.05);
}

.c-name { font-size: 0.95rem; font-weight: 700; color: #eee; }
.c-subtotal { font-size: 0.95rem; font-weight: 700; color: #aaa; }

.loading-state {
  text-align: center;
  border-radius: 30px;
  margin-top: 3rem;
}

.spinner-large {
  width: 60px;
  height: 60px;
  border: 6px solid rgba(232, 136, 42, 0.1);
  border-top-color: var(--color-primary);
  border-radius: 50%;
  margin: 0 auto 1.5rem;
  animation: spin 1s linear infinite;
}

@keyframes spin { to { transform: rotate(360deg); } }

/* ─── VietQR Modal ─── */
.qr-overlay {
  position: fixed; inset: 0; background: rgba(0,0,0,0.85);
  backdrop-filter: blur(8px); z-index: 2000;
  display: flex; align-items: center; justify-content: center; padding: 2rem;
}
.qr-modal {
  background: #fff; color: #111; width: 100%; max-width: 450px;
  border-radius: 28px; padding: 2.5rem; position: relative;
  box-shadow: 0 30px 60px rgba(0,0,0,0.5);
}
.glass-panel-heavy { background: rgba(255, 255, 255, 0.95); }
.qr-close {
  position: absolute; top: 1.5rem; right: 1.5rem;
  background: #eee; border: none; width: 32px; height: 32px;
  border-radius: 50%; font-size: 1.5rem; line-height: 1;
  color: #666; cursor: pointer; transition: 0.2s;
}
.qr-close:hover { background: #ddd; color: #000; }

.qr-header { text-align: center; margin-bottom: 2rem; }
.qr-logo { height: 30px; margin-bottom: 1rem; }
.qr-header h4 { font-weight: 800; font-size: 1.3rem; margin-bottom: 0.5rem; color: #111; }
.qr-header p { font-size: 0.85rem; color: #666; padding: 0 1rem; }

.qr-code-wrapper {
  background: #f8f9fa; border-radius: 20px; padding: 2rem;
  text-align: center; margin-bottom: 2rem; border: 1px solid #eee;
}
.qr-image { width: 100%; max-width: 250px; border-radius: 12px; margin-bottom: 1.5rem; }
.qr-amount { display: flex; flex-direction: column; gap: 0.4rem; }
.qr-amount .label { font-size: 0.75rem; font-weight: 700; color: #888; text-transform: uppercase; }
.qr-amount .value { font-size: 1.8rem; font-weight: 900; color: #111; }

.qr-instructions { text-align: center; font-size: 0.85rem; color: #555; margin-bottom: 2rem; line-height: 1.5; }
.qr-actions { display: flex; flex-direction: column; gap: 0.8rem; }

.animate-zoom { animation: zoomIn 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275); }
@keyframes zoomIn { from { opacity: 0; transform: scale(0.9); } to { opacity: 1; transform: scale(1); } }

/* ─── Success Bill / Ticket UI ─── */
.bill-overlay {
  position: fixed; inset: 0; background: rgba(0,0,0,0.9);
  backdrop-filter: blur(10px); z-index: 3000;
  display: flex; align-items: flex-start; justify-content: center; 
  padding: 2rem; overflow-y: auto;
}
.ticket-bill {
  background: #fff; color: #111; width: 100%; max-width: 480px;
  border-radius: 28px; padding: 2rem; position: relative;
  box-shadow: 0 40px 100px rgba(0,0,0,0.6);
  margin: auto 0;
}
.bill-header { text-align: center; margin-bottom: 1.5rem; }
.success-icon { 
  width: 50px; height: 50px; background: #00e676; color: white; 
  border-radius: 50%; display: flex; align-items: center; justify-content: center;
  font-size: 1.5rem; margin: 0 auto 1rem; box-shadow: 0 10px 20px rgba(0, 230, 118, 0.3);
}
.bill-status { font-weight: 900; font-size: 1.5rem; letter-spacing: -1px; margin-bottom: 0.3rem; }
.bill-id { font-size: 1rem; color: #666; font-weight: 600; }
.bill-id span { color: var(--color-primary); font-weight: 800; }

.success-qr-container {
  display: flex;
  justify-content: center;
  align-items: center;
  margin-bottom: 1rem;
}

.qr-bill-border {
  padding: 8px;
  background: white;
  border-radius: 12px;
  box-shadow: 0 10px 30px rgba(0,0,0,0.1);
  display: flex;
}

.ticket-visual { 
  display: flex; gap: 1.2rem; background: #f8f9fa; border: 2px dashed #ddd;
  padding: 1.2rem; border-radius: 16px; position: relative;
}
.ticket-visual::before, .ticket-visual::after {
  content: ''; position: absolute; top: 50%; width: 30px; height: 30px;
  background: #fff; border-radius: 50%; transform: translateY(-50%);
}
.ticket-visual::before { left: -17px; }
.ticket-visual::after { right: -17px; }

.ticket-poster { width: 120px; border-radius: 12px; box-shadow: 0 8px 20px rgba(0,0,0,0.2); }
.ticket-right { flex: 1; }
.t-movie-title { font-size: 1.5rem; font-weight: 900; line-height: 1.2; margin-bottom: 0.5rem; }
.t-format { font-size: 0.8rem; font-weight: 800; color: #888; letter-spacing: 1px; }

.t-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 1.2rem; }
.t-label { font-size: 0.65rem; font-weight: 800; color: #999; text-transform: uppercase; letter-spacing: 1px; display: block; margin-bottom: 0.2rem; }
.t-val { font-size: 1rem; font-weight: 700; color: #333; }

.t-seats-box { border-top: 1px solid #eee; padding-top: 1.2rem; }
.t-seats-vals { display: flex; flex-wrap: wrap; gap: 0.5rem; margin-top: 0.5rem; }
.t-seat-v { background: #111; color: #fff; padding: 0.3rem 0.8rem; border-radius: 6px; font-weight: 800; font-size: 0.9rem; }

.bill-summary { margin-top: 2rem; padding: 0 1rem; }
.bs-row { display: flex; justify-content: space-between; font-size: 1rem; color: #666; font-weight: 600; margin-bottom: 0.5rem; }
.bs-total { display: flex; justify-content: space-between; font-weight: 900; font-size: 1.4rem; border-top: 1px solid #eee; padding-top: 1rem; color: #111; }

.bill-note { font-size: 0.8rem; color: #888; text-align: center; }

/* ─── Ticket Code Box ─── */
.ticket-code-box {
  background: #111; color: #fff; padding: 0.8rem; border-radius: 12px;
  display: inline-block; min-width: 200px; margin-top: 0.8rem;
}
.t-label-code { font-size: 0.65rem; font-weight: 800; color: var(--color-primary); letter-spacing: 1.5px; display: block; margin-bottom: 0.3rem; }
.t-code { font-size: 1.5rem; font-weight: 900; letter-spacing: 3px; margin: 0; font-family: 'Courier New', Courier, monospace; }

.animate-pulse-scale { animation: pulseScale 2s infinite ease-in-out; }
@keyframes pulseScale { 0%, 100% { transform: scale(1); } 50% { transform: scale(1.03); } }
.gift-row {
  margin-top: 5px;
  padding: 8px 0;
  border-top: 1px dashed #eee;
  animation: slideIn 0.5s ease;
}
.gift-row span:last-child {
  font-weight: 800;
}
</style>
