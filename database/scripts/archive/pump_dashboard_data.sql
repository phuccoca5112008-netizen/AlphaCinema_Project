USE AlphaCinema;
GO

-- 1. Xóa bớt các hóa đơn nháp để dữ liệu thật trông gọn gàng
DELETE HoaDon WHERE trang_thai = N'Đang chờ';
DELETE Ve WHERE trang_thai = N'Đang chờ';

-- 2. Tự động tìm ID suất chiếu thực tế đang có trong máy bạn
DECLARE @SuatChieu1 INT, @SuatChieu2 INT, @SuatChieu3 INT;
SELECT TOP 1 @SuatChieu1 = ma_suat_chieu FROM SuatChieu ORDER BY ma_suat_chieu ASC;
SELECT TOP 1 @SuatChieu2 = ma_suat_chieu FROM SuatChieu WHERE ma_suat_chieu > @SuatChieu1 ORDER BY ma_suat_chieu ASC;
SELECT TOP 1 @SuatChieu3 = ma_suat_chieu FROM SuatChieu WHERE ma_suat_chieu > @SuatChieu2 ORDER BY ma_suat_chieu ASC;

-- 3. Tự động tìm ghế của các suất chiếu này
DECLARE @Ghe1_1 INT, @Ghe1_2 INT, @Ghe2_1 INT, @Ghe3_1 INT;
SELECT TOP 1 @Ghe1_1 = ma_ghe FROM Ghe WHERE ma_phong = (SELECT ma_phong FROM SuatChieu WHERE ma_suat_chieu = @SuatChieu1);
SELECT TOP 1 @Ghe1_2 = ma_ghe FROM Ghe WHERE ma_phong = (SELECT ma_phong FROM SuatChieu WHERE ma_suat_chieu = @SuatChieu1) AND ma_ghe <> @Ghe1_1;
SELECT TOP 1 @Ghe2_1 = ma_ghe FROM Ghe WHERE ma_phong = (SELECT ma_phong FROM SuatChieu WHERE ma_suat_chieu = @SuatChieu2);
SELECT TOP 1 @Ghe3_1 = ma_ghe FROM Ghe WHERE ma_phong = (SELECT ma_phong FROM SuatChieu WHERE ma_suat_chieu = @SuatChieu3);

IF (@SuatChieu1 IS NOT NULL)
BEGIN
    BEGIN TRANSACTION;
    
    -- Giao dịch 1: Cho Phim 1 (3 vé - 2 ngày trước)
    INSERT INTO HoaDon (ma_nguoi_dung, tong_tien, phuong_thuc_thanh_toan, ngay_giao_dich, ma_vao_cong, trang_thai)
    VALUES (1, 300000, N'Ví MoMo', DATEADD(DAY, -2, GETDATE()), 'BLOCK-01', N'Đã thanh toán');
    DECLARE @HD1 INT = SCOPE_IDENTITY();
    INSERT INTO Ve (ma_hoa_don, ma_ghe, ma_suat_chieu, ma_qr, gia_ve, trang_thai) VALUES
    (@HD1, @Ghe1_1, @SuatChieu1, 'QR-001', 100000, N'Đã thanh toán'),
    (@HD1, @Ghe1_2, @SuatChieu1, 'QR-002', 100000, N'Đã thanh toán');

    -- Giao dịch 2: Cho Phim 2 (2 vé - 1 ngày trước)
    INSERT INTO HoaDon (ma_nguoi_dung, tong_tien, phuong_thuc_thanh_toan, ngay_giao_dich, ma_vao_cong, trang_thai)
    VALUES (1, 180000, N'ZaloPay', DATEADD(DAY, -1, GETDATE()), 'BLOCK-02', N'Đã sử dụng');
    DECLARE @HD2 INT = SCOPE_IDENTITY();
    INSERT INTO Ve (ma_hoa_don, ma_ghe, ma_suat_chieu, ma_qr, gia_ve, trang_thai) VALUES
    (@HD2, @Ghe2_1, @SuatChieu2, 'QR-003', 90000, N'Đã sử dụng');

    -- Giao dịch 3: Cho Phim 3 (1 vé - hôm nay)
    INSERT INTO HoaDon (ma_nguoi_dung, tong_tien, phuong_thuc_thanh_toan, ngay_giao_dich, ma_vao_cong, trang_thai)
    VALUES (1, 120000, N'Tiền mặt', GETDATE(), 'BLOCK-03', N'Đã thanh toán');
    DECLARE @HD3 INT = SCOPE_IDENTITY();
    INSERT INTO Ve (ma_hoa_don, ma_ghe, ma_suat_chieu, ma_qr, gia_ve, trang_thai) VALUES
    (@HD3, @Ghe3_1, @SuatChieu3, 'QR-004', 120000, N'Đã thanh toán');

    COMMIT TRANSACTION;
    PRINT 'Dashboard Data PUMPED successfully!';
END
ELSE
BEGIN
    PRINT 'Error: No SuatChieu found. Please create some showtimes first!';
END
