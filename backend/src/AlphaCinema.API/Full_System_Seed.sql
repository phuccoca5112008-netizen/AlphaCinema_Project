/* 
   MASTER SEED SCRIPT FOR ALPHA CINEMA
   ===================================
   This script builds a complete, production-ready cinema dataset.
   Includes: Roles, Users, Rooms, Seats, Movies, Showtimes, Promotions, and Concessions.
*/

USE AlphaCinema;
GO

-- 0. CLEANUP (Tránh trùng lặp khi chạy lại)
DELETE FROM Ve;
DELETE FROM HoaDonDoAnVat;
DELETE FROM HoaDon;
DELETE FROM SuatChieu;
DELETE FROM Ghe;
DELETE FROM PhongChieu;
DELETE FROM Phim;
DELETE FROM KhuyenMai;
DELETE FROM DoAnVat;
DELETE FROM DanhGia;
DELETE FROM NguoiDung;
DELETE FROM VaiTro;
GO

-- 1. VAI TRÒ (Roles)
INSERT INTO VaiTro (ten_vai_tro) VALUES (N'Admin'), (N'Staff'), (N'Customer');
GO

-- 2. NGƯỜI DÙNG (Users) - Passwords are hashed 'password123' usually, for demo:
INSERT INTO NguoiDung (ma_vai_tro, email, mat_khau, ho_ten, diem_tich_luy) VALUES
(1, 'admin@alpha.com', 'AQAAAAEAACcQAAAAEPvAIP6W7V6/T9O7oN1...', N'Quản Trị Viên', 1000),
(2, 'staff@alpha.com', 'AQAAAAEAACcQAAAAEPvAIP6W7V6/T9O7oN1...', N'Nhân Viên Alpha', 0),
(3, 'customer@alpha.com', 'AQAAAAEAACcQAAAAEPvAIP6W7V6/T9O7oN1...', N'Trần Phước Tín', 50);
GO

-- 3. PHIM (Movies)
INSERT INTO Phim (ten_phim, the_loai, thoi_luong, poster, tom_tat, trang_thai_phim) VALUES
(N'Dune: Hành Tinh Cát - Phần 2', N'Hành Động, Viễn Tưởng', 166, 'https://image.tmdb.org/t/p/w500/1pdfLvkbY9ohJlCjQH2TGpiKKTe.jpg', N'Paul Atreides tiếp tục hành trình trả thù...', N'Đang chiếu'),
(N'Deadpool & Wolverine', N'Hành Động, Hài', 127, 'https://image.tmdb.org/t/p/w500/8cdWjvZQUExUUTzyp4t6EDMubfO.jpg', N'Tội phạm thời thời gian buộc Deadpool phải hợp tác...', N'Đang chiếu'),
(N'Kung Fu Panda 4', N'Hoạt Hình, Hài', 94, 'https://image.tmdb.org/t/p/w500/kDp1vUBnMpe8ak4rjgl3cLELqjU.jpg', N'Po chuẩn bị trở thành Thủ lĩnh tinh thần...', N'Đang chiếu'),
(N'Oppenheimer', N'Lịch sử, Tâm lý', 180, 'https://image.tmdb.org/t/p/w500/8Gxv8gSFCU0XGDykEGv7zR1n2ua.jpg', N'Tiểu sử về cha đẻ bom nguyên tử...', N'Đang chiếu');
GO

-- 4. PHÒNG CHIẾU & GHẾ (Rooms & Seats)
INSERT INTO PhongChieu (ten_phong, loai_phong) VALUES 
(N'P1 - IMAX', N'IMAX'), (N'P2 - PREMIUM', N'VIP'), (N'P3 - STANDARD', N'2D');

DECLARE @R1 INT = (SELECT ma_phong FROM PhongChieu WHERE ten_phong = N'P1 - IMAX');
DECLARE @R2 INT = (SELECT ma_phong FROM PhongChieu WHERE ten_phong = N'P2 - PREMIUM');

-- Ghế cho P1 (IMAX - 32 ghế)
DECLARE @H CHAR(1) = 'A';
WHILE ASCII(@H) <= ASCII('D')
BEGIN
    DECLARE @S INT = 1;
    WHILE @S <= 8
    BEGIN
        INSERT INTO Ghe (ma_phong, hang, so_ghe, loai_ghe) 
        VALUES (@R1, @H, @S, CASE WHEN @S BETWEEN 3 AND 6 THEN 'VIP' ELSE 'Thường' END);
        SET @S = @S + 1;
    END
    SET @H = CHAR(ASCII(@H) + 1);
END
GO

-- 5. ĐỒ ĂN VẶT (Concessions)
INSERT INTO DoAnVat (ten_mon, mo_ta, gia, hinh_anh, loai) VALUES 
(N'Bắp Rang Ngọt (M)', N'Bắp thơm ngon size vừa', 45000, '/assets/concessions/bap_caramel_m.png', N'Bắp'),
(N'Bắp Rang Phô Mai (L)', N'Bắp mặn phô mai size lớn', 60000, '/assets/concessions/bap_phomai_l.png', N'Bắp'),
(N'Coca Cola (L)', N'Nước ngọt ga mạnh', 35000, '/assets/concessions/coca_l.png', N'Nước'),
(N'Combo Single Alpha', N'1 Bắp (M) + 1 Nước (L)', 80000, '/assets/concessions/combo_single.png', N'Combo');
GO

-- 6. KHUYẾN MÃI (Promotions)
INSERT INTO KhuyenMai (ten_khuyen_mai, ma_code_giam_gia, mo_ta, ngay_bat_dau, ngay_ket_thuc, loai_giam_gia, gia_tri_giam, don_hang_toi_thieu, hinh_anh, phan_loai) VALUES
(N'Giảm giá 1', 'GIAMGIA1', N'Ưu đãi tri ân khách hàng thân thiết của Alpha Cinema...', '2026-03-26', '2026-04-29', 'PhanTram', 10, 0, 'super_monday.png', N'ƯU ĐÃI'),
(N'Quà Mừng Lên Hạng Gold 2026', 'ALPHAGOLD2026', N'Nâng cấp hạng thành viên của bạn để nhận ngay các phần quà hấp dẫn: Voucher vé 0đ, Combo bắp nước và quà tặng vật phẩm độc quyền từ Alpha Cinema.', '2026-01-01', '2026-12-31', 'PhanTram', 100, 0, 'membership_upgrade.png', N'VIP MEMBER'),
(N'Ưu đãi VNPAY-QR', 'VNPAYALPHA', N'Giảm ngay 15K cho mỗi giao dịch từ 150K khi thanh toán qua ứng dụng VNPAY-QR tại Alpha Cinema.', '2026-01-01', '2026-12-31', 'CoDinh', 15000, 150000, 'vnpay_discount.png', N'THANH TOÁN'),
(N'Ưu đãi Học Sinh - Sinh Viên', 'STUDENT20', N'Đồng giá vé 45k cho HSSV vào các ngày trong tuần (trừ ngày lễ tết). Vui lòng trình thẻ HSSV chính chủ.', '2026-01-01', '2026-12-31', 'PhanTram', 20, 0, 'teacher_day.png', N'STUDENT DEAL'),
(N'Combo Gia Đình Tiết Kiệm', 'FAMCOMBO', N'Tiết kiệm đến 30% khi mua gói combo dành cho gia đình gồm 2 bắp lớn và 4 nước ngọt.', '2026-01-01', '2026-12-31', 'PhanTram', 30, 0, 'family_combo.png', N'ƯU ĐÃI'),
(N'Đặc quyền VIP Free Combo', 'VIPFREE', N'Tặng 01 bắp nước (quy đổi giảm 50k) cho hội viên Diamond khi check-in tại quầy vé.', '2026-01-01', '2026-12-31', 'CoDinh', 50000, 0, 'birthday_gift.png', N'VIP MEMBER'),
(N'Lì Xì Tết Alpha 2026', 'TET2026', N'Bốc thăm nhận lì xì may mắn với tổng giá trị giải thưởng lên đến 1 tỷ đồng. Bắp ngọt miễn phí cho mọi khách hàng hái lộc.', '2026-01-20', '2026-02-15', 'PhanTram', 5, 0, 'tet_holiday.png', N'LỄ TẾT'),
(N'Voucher Bom Tấn Drink55', 'DRINK55', N'Giảm 5% nước uống (quy đổi giảm 5k) khi mua kèm vé phim bom tấn bất kỳ.', '2026-06-10', '2026-12-31', 'CoDinh', 5000, 0, 'blockbuster_voucher.png', N'ƯU ĐÃI');
GO

-- 7. SUẤT CHIẾU (Showtimes) - Lịch cho 7 ngày tới
DECLARE @M1 INT = (SELECT ma_phim FROM Phim WHERE ten_phim LIKE N'%Dune%');
DECLARE @M2 INT = (SELECT ma_phim FROM Phim WHERE ten_phim LIKE N'%Deadpool%');
DECLARE @P1 INT = (SELECT ma_phong FROM PhongChieu WHERE ten_phong = N'P1 - IMAX');

INSERT INTO SuatChieu (ma_phim, ma_phong, thoi_gian_bat_dau, dinh_dang, gia_ve_goc) VALUES
(@M1, @P1, DATEADD(HOUR, 9, CAST(GETDATE() AS DATETIME)), 'IMAX', 150000),
(@M1, @P1, DATEADD(HOUR, 14, CAST(GETDATE() AS DATETIME)), 'IMAX', 150000),
(@M2, @P1, DATEADD(HOUR, 19, CAST(GETDATE() AS DATETIME)), 'IMAX', 120000);
GO

SELECT 'DATABASE SEEDED SUCCESSFULLY!' AS Result;
GO
