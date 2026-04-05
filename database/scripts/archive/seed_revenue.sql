
-- Seed Script for AlphaCinema Revenue Mock Data
-- Created on 2026-04-01

BEGIN TRANSACTION;

DECLARE @UserId INT = 1;
DECLARE @Ghe1 INT = 365, @Ghe2 INT = 366, @Ghe3 INT = 367, @Ghe4 INT = 368, @Ghe5 INT = 369;

-- 1. ALIEN: ROMULUS (SuatChieu 1083) - 300,000 VND
INSERT INTO HoaDon (ma_nguoi_dung, tong_tien, phuong_thuc_thanh_toan, ngay_giao_dich, ma_vao_cong, trang_thai)
VALUES (@UserId, 300000, N'Ví MoMo', '2026-03-25 14:30:00', 'AL-1083-SEED-01', N'Đã thanh toán');
DECLARE @HD1 INT = SCOPE_IDENTITY();
INSERT INTO Ve (ma_hoa_don, ma_ghe, ma_suat_chieu, ma_qr, gia_ve, trang_thai)
VALUES (@HD1, @Ghe1, 1083, 'QR-01', 150000, N'Đã thanh toán'),
       (@HD1, @Ghe2, 1083, 'QR-02', 150000, N'Đã thanh toán');

-- 2. GODZILLA x KONG (SuatChieu 1081) - 180,000 VND
INSERT INTO HoaDon (ma_nguoi_dung, tong_tien, phuong_thuc_thanh_toan, ngay_giao_dich, ma_vao_cong, trang_thai)
VALUES (@UserId, 180000, N'ZaloPay', '2026-03-28 19:15:00', 'GZ-1081-SEED-02', N'Đã sử dụng');
DECLARE @HD2 INT = SCOPE_IDENTITY();
INSERT INTO Ve (ma_hoa_don, ma_ghe, ma_suat_chieu, ma_qr, gia_ve, trang_thai)
VALUES (@HD2, @Ghe3, 1081, 'QR-03', 90000, N'Đã sử dụng'),
       (@HD2, @Ghe4, 1081, 'QR-04', 90000, N'Đã sử dụng');

-- 3. DEADPOOL VÀ WOLVERINE (SuatChieu 36) - 450,000 VND
INSERT INTO HoaDon (ma_nguoi_dung, tong_tien, phuong_thuc_thanh_toan, ngay_giao_dich, ma_vao_cong, trang_thai)
VALUES (@UserId, 450000, N'Thẻ Tín Dụng', '2026-03-31 21:00:00', 'DP-36-SEED-03', N'Đã thanh toán');
DECLARE @HD3 INT = SCOPE_IDENTITY();
INSERT INTO Ve (ma_hoa_don, ma_ghe, ma_suat_chieu, ma_qr, gia_ve, trang_thai)
VALUES (@HD3, @Ghe1, 36, 'QR-05', 150000, N'Đã thanh toán'),
       (@HD3, @Ghe2, 36, 'QR-06', 150000, N'Đã thanh toán'),
       (@HD3, @Ghe3, 36, 'QR-07', 150000, N'Đã thanh toán');

-- 4. INSIDE OUT 2 (SuatChieu 51) - 240,000 VND (Hôm nay)
INSERT INTO HoaDon (ma_nguoi_dung, tong_tien, phuong_thuc_thanh_toan, ngay_giao_dich, ma_vao_cong, trang_thai)
VALUES (@UserId, 240000, N'Ví MoMo', '2026-04-01 09:20:00', 'IO-51-SEED-04', N'Đã thanh toán');
DECLARE @HD4 INT = SCOPE_IDENTITY();
INSERT INTO Ve (ma_hoa_don, ma_ghe, ma_suat_chieu, ma_qr, gia_ve, trang_thai)
VALUES (@HD4, @Ghe4, 51, 'QR-08', 120000, N'Đã thanh toán'),
       (@HD4, @Ghe5, 51, 'QR-09', 120000, N'Đã thanh toán');

COMMIT TRANSACTION;

SELECT 'Successfully seeded mock data for 4 movies!' AS Message;
