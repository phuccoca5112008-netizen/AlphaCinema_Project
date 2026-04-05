export const promotions = [
  {
    id: 1,
    type: 'member',
    tag: 'Ưu Đãi Lên Hạng',
    title: 'QUÀ MỪNG LÊN HẠNG - ALPHA MEMBER 2026',
    description: 'Nâng cấp hạng thành viên của bạn để nhận ngay các phần quà hấp dẫn: Voucher vé 0đ, Combo bắp nước và quà tặng vật phẩm độc quyền từ Alpha Cinema.',
    imageName: 'membership_upgrade.png',
    startDate: '01/01/2026',
    endDate: '31/12/2026',
    details: {
      intro: 'Chương trình tri ân dành riêng cho các thành viên tích cực của Alpha Cinema. Khi đạt mốc tích lũy mới, bạn sẽ nhận được gói quà tặng giá trị.',
      howTo: '1. Đăng nhập vào tài khoản trên website/app\n2. Kiểm tra tiến trình tích lũy trong mục Cá nhân\n3. Khi đủ điểm, nút "Nhận Quà" sẽ hiện lên.',
      voucher: 'ALPHAGOLD2026 (Giảm 100% cho 01 vé phim bất kỳ)',
      isFree: true
    }
  },
  {
    id: 2,
    type: 'ticket',
    tag: 'Ưu Đãi Hot',
    title: 'THỨ 3 VUI VẺ - ĐỒNG GIÁ 50K MỌI SUẤT CHIẾU',
    description: 'Thứ Ba hàng tuần, tận hưởng điện ảnh không giới hạn với ưu đãi đồng giá 50.000đ cho tất cả các suất chiếu 2D toàn hệ thống Alpha Cinema.',
    imageName: 'super_monday.png',
    startDate: '01/01/2026',
    endDate: '31/12/2026',
    details: {
      intro: 'Ngày hội điện ảnh lớn nhất tuần tại Alpha Cinema với mức giá không tưởng.',
      howTo: 'Áp dụng trực tiếp khi mua tại quầy hoặc đặt vé online vào ngày Thứ Ba hàng tuần.',
      voucher: 'Hệ thống tự động áp dụng giá 50k - KHÔNG CẦN NHẬP MÃ',
      isFree: false
    }
  },
  {
    id: 3,
    type: 'member',
    tag: 'VIP Member',
    title: 'THÀNH VIÊN VIP: TÍCH ĐIỂM - ĐỔI VÉ MIỄN PHÍ',
    description: 'Trở thành VIP Member của Alpha Cinema để tận hưởng đặc quyền tích điểm gấp đôi, đổi vé xem phim và bắp nước hoàn toàn miễn phí cùng vô vàn quà tặng limited.',
    imageName: 'membership_upgrade.png',
    startDate: '01/01/2026',
    endDate: '31/12/2026',
    details: {
      intro: 'Lớp thành viên cao cấp nhất với tỷ lệ tích điểm 10% trên mỗi giao dịch.',
      howTo: 'Duy trì chi tiêu tối thiểu 2.000.000đ/năm để giữ hạng VIP.',
      voucher: 'VIPFREECOMBO (Tặng 01 bắp nước cho mỗi lần xem phim)',
      isFree: true
    }
  },
  {
    id: 4,
    type: 'payment',
    tag: 'Partner Deal',
    title: 'XEM PHIM ALPHA CINEMA CÙNG VNPAY-QR - GIẢM 15K',
    description: 'Nhập mã VNPAYALPHA khi thanh toán qua ví VNPAY-QR để nhận ngay ưu đãi giảm giá 15K cho mỗi giao dịch từ 150K trở lên.',
    imageName: 'vnpay_discount.png',
    startDate: '01/01/2026',
    endDate: '31/12/2026',
    details: {
      intro: 'Ưu đãi thanh toán thông minh cùng đối tác VNPAY.',
      howTo: '1. Chọn phim và suất chiếu\n2. Chọn cổng thanh toán VNPAY-QR\n3. Nhập mã voucher trên ứng dụng ngân hàng của bạn.',
      voucher: 'VNPAYALPHA (Giảm 15k)',
      isFree: false
    }
  },
  {
    id: 5,
    type: 'member',
    tag: 'Student Deal',
    title: 'ƯU ĐÃI HỌC SINH - SINH VIÊN: GIẢM NGAY 20%',
    description: 'Chỉ cần xuất trình thẻ HSSV chính chủ, bạn sẽ được giảm ngay 20% giá vé niêm yết tại quầy vé Alpha Cinema. Deal hời cho các mọt phim trẻ!',
    imageName: 'early_bird.png',
    startDate: '01/01/2026',
    endDate: '31/12/2026',
    details: {
      intro: 'Tiếp sức đam mê điện ảnh cho các bạn học sinh - sinh viên trên toàn quốc.',
      howTo: 'Đưa thẻ HSSV cho nhân viên quầy vé khi thanh toán trực tiếp.',
      voucher: 'STUDENTR20 (Áp dụng tại quầy)',
      isFree: false
    }
  },
  {
    id: 6,
    type: 'ticket',
    tag: 'Mua 2 Tặng 1',
    title: 'SỰ KIỆN PHIM CONAN - MUA 2 VÉ TẶNG 1 VÉ PHIM',
    description: 'Ưu đãi cực lớn dành cho các fan của Thám Tử Lừng Danh Conan! Mua 2 vé xem phim Conan tại rạp, tặng ngay 1 vé xem phim hoặc vật phẩm đặc biệt.',
    imageName: 'conan_promo.png',
    startDate: '01/08/2026',
    endDate: '31/08/2026',
    details: {
      intro: 'Đón chào thám tử lừng danh trở lại màn ảnh rộng.',
      howTo: 'Mua 02 vé cùng suất chiếu phim Conan để nhận ngay coupon vé tặng cho phim tiếp theo.',
      voucher: 'CONANGIFT (Số lượng có hạn tại rạp)',
      isFree: true
    }
  },
  {
    id: 7,
    type: 'member',
    tag: 'Quà Tết Alpha',
    title: 'LÌ XÌ TẾT VUI NHƯ Ý - KHỞI ĐẦU NĂM MỚI MAY MẮN',
    description: 'Đến Alpha Cinema xem phim những ngày Tết để nhận ngay bao lì xì độc quyền và cơ hội trúng thẻ VIP cả năm hoặc thẻ bắp nước miễn phí.',
    imageName: 'tet_holiday.png',
    startDate: '20/01/2026',
    endDate: '15/02/2026',
    details: {
      intro: 'Khai xuân rạng rỡ cùng những phần quà Lì xì may mắn đầu năm.',
      howTo: 'Mỗi giao dịch vé trong 3 ngày mùng 1, 2, 3 Tết đều nhận được 01 bao lì xì.',
      voucher: 'TETALPHA2026 (Bắp ngọt Miễn Phí)',
      isFree: true
    }
  },
  {
    id: 8,
    type: 'ticket',
    tag: 'Suất Chiếu Sớm',
    title: 'NHẬN NGAY CARD BO GÓC ĐỘC QUYỀN KHI XEM SỚM',
    description: 'Khi xem các suất chiếu sớm (Sneak Show) của các phim bom tấn, Alpha Cinema dành tặng bộ sưu tập Card bo góc Limited cho những fan nhanh nhất.',
    imageName: 'early_bird.png',
    startDate: '01/01/2026',
    endDate: '31/12/2026',
    details: {
      intro: 'Sưu tầm trọn bộ Movie Card độc quyền từ các phim bom tấn thế giới.',
      howTo: 'Hiện diện tại rạp trước 15 phút suất chiếu sớm để nhận quà.',
      voucher: 'SNEAKCARD (Tặng Card Bo Góc)',
      isFree: true
    }
  },
  {
    id: 9,
    type: 'ticket',
    tag: 'Voucher Bom Tấn',
    title: 'TẶNG VOUCHER GIẢM 5% CHO PHIM BLOCKBUSTER',
    description: 'Khi mua vé xem các phim siêu anh hùng hoặc phim bom tấn tại Alpha Cinema, bạn sẽ nhận được voucher giảm giá 5% cho đồ uống tại hệ thống quầy bar.',
    imageName: 'blockbuster_voucher.png',
    startDate: '10/06/2026',
    endDate: '31/12/2026',
    details: {
      intro: 'Nạp năng lượng cho những trận chiến hoành tráng trên màn ảnh.',
      howTo: 'Mã voucher sẽ được in trực tiếp vào phần đuôi của vé phim khi bạn mua vé.',
      voucher: 'DRINK55 (Giảm 5% nước uống)',
      isFree: false
    }
  },
  {
    id: 10,
    type: 'payment',
    tag: 'Combo Gia Đình',
    title: 'COMBO GIA ĐÌNH - TIẾT KIỆM ĐẾN 30% TRONG NĂM',
    description: 'Gói ưu đãi 4 vé xem phim kèm bắp nước dành riêng cho gia đình. Tiết kiệm hơn, vui hơn khi tận hưởng điện ảnh cùng những người thân yêu.',
    imageName: 'family_combo.png',
    startDate: '01/01/2026',
    endDate: '31/12/2026',
    details: {
      intro: 'Gắn kết gia đình qua những khoảnh khắc điện ảnh tuyệt vời.',
      howTo: 'Chọn loại vé "FAMILY BUNDLE" trên hệ thống đặt vé hoặc quầy vé.',
      voucher: 'FAMILYSAVE30 (Áp dụng cho đoàn từ 4 người)',
      isFree: false
    }
  }
];
