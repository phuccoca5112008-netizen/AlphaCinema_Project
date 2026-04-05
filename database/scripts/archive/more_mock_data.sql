USE AlphaCinema;

-- 1. ADD MORE ROOMS
SET IDENTITY_INSERT PhongChieu ON;
INSERT INTO PhongChieu (ma_phong, ten_phong) VALUES 
(5, N'Gold Class 5'), 
(6, N'Bed Cinema 6'), 
(7, N'ScreenX 7');
SET IDENTITY_INSERT PhongChieu OFF;

-- 2. ADD SEATS FOR NEW ROOMS
-- Room 5 Gold Class (5 rows, 6 seats each, all LUXURY/VIP)
DECLARE @h INT = 65;
WHILE @h <= 69
BEGIN
    DECLARE @s INT = 1;
    WHILE @s <= 6
    BEGIN
        INSERT INTO Ghe (ma_phong, hang, so_ghe, loai_ghe) VALUES (5, CHAR(@h), @s, N'VIP');
        SET @s += 1;
    END
    SET @h += 1;
END

-- Room 6 Bed Cinema (4 rows, 4 seats each, big beds)
SET @h = 65;
WHILE @h <= 68
BEGIN
    SET @s = 1;
    WHILE @s <= 4
    BEGIN
        INSERT INTO Ghe (ma_phong, hang, so_ghe, loai_ghe) VALUES (6, CHAR(@h), @s, N'VIP');
        SET @s += 1;
    END
    SET @h += 1;
END

-- Room 7 ScreenX (8 rows, 14 seats each)
SET @h = 65;
WHILE @h <= 72
BEGIN
    SET @s = 1;
    WHILE @s <= 14
    BEGIN
        DECLARE @loai NVARCHAR(20) = N'Thường';
        IF (@h >= 67 AND @h <= 71 AND @s >= 4 AND @s <= 11) SET @loai = N'VIP';
        INSERT INTO Ghe (ma_phong, hang, so_ghe, loai_ghe) VALUES (7, CHAR(@h), @s, @loai);
        SET @s += 1;
    END
    SET @h += 1;
END

-- 3. UPDATE SOME MOVIES TO "Đang chiếu" TO HAVE MORE VARIETY
UPDATE Phim SET trang_thai_phim = N'Đang chiếu' WHERE ma_phim IN (9, 10, 11, 21);

-- 4. ADD MORE SHOWTIMES (SPREAD ACROSS NEXT 4 DAYS)
DECLARE @D0 DATETIME = CAST(GETDATE() AS DATE);
DECLARE @D1 DATETIME = DATEADD(day,1,@D0);
DECLARE @D2 DATETIME = DATEADD(day,2,@D0);
DECLARE @D3 DATETIME = DATEADD(day,3,@D0);

INSERT INTO SuatChieu (ma_phim, ma_phong, thoi_gian_bat_dau, dinh_dang, gia_ve_goc) VALUES
-- Gold Class (Room 5)
(17, 5, DATEADD(hour,19,@D0), '2D', 250000),
(17, 5, DATEADD(hour,21,@D0), '2D', 250000),
(4, 5, DATEADD(hour,15,@D1), '2D', 250000),
(4, 5, DATEADD(hour,18,@D1), '2D', 250000),
(11, 5, DATEADD(hour,14,@D2), '2D', 300000),

-- Bed Cinema (Room 6)
(2, 6, DATEADD(hour,20,@D0), '2D', 400000),
(9, 6, DATEADD(hour,19,@D1), '2D', 400000),
(19, 6, DATEADD(hour,22,@D1), '2D', 450000),

-- ScreenX (Room 7)
(7, 7, DATEADD(hour,10,@D0), 'ScreenX', 150000),
(7, 7, DATEADD(hour,13,@D0), 'ScreenX', 150000),
(7, 7, DATEADD(hour,16,@D0), 'ScreenX', 160000),
(21, 7, DATEADD(hour,19,@D0), 'ScreenX', 180000),
(21, 7, DATEADD(hour,22,@D0), 'ScreenX', 180000),

-- Standard Rooms (Filling more gaps)
(18, 4, DATEADD(hour,8,@D0), '2D', 65000),
(18, 4, DATEADD(hour,12,@D0), '2D', 75000),
(3,  3, DATEADD(hour,9,@D0), '3D', 85000),
(10, 3, DATEADD(hour,11,@D0), '3D', 90000),
(10, 4, DATEADD(hour,15,@D0), '2D', 75000),
(10, 3, DATEADD(hour,20,@D0), '3D', 95000);

-- Add tomorrow showtimes
INSERT INTO SuatChieu (ma_phim, ma_phong, thoi_gian_bat_dau, dinh_dang, gia_ve_goc) VALUES
(17, 1, DATEADD(hour,9,@D1), 'IMAX', 120000),
(17, 1, DATEADD(hour,12,@D1), 'IMAX', 120000),
(17, 7, DATEADD(hour,15,@D1), 'ScreenX', 160000),
(21, 7, DATEADD(hour,18,@D1), 'ScreenX', 180000),
(11, 1, DATEADD(hour,21,@D1), 'IMAX', 150000),
(6,  4, DATEADD(hour,10,@D1), '2D', 70000),
(5,  4, DATEADD(hour,13,@D1), '2D', 80000),
(5,  3, DATEADD(hour,16,@D1), '3D', 95000);

PRINT 'Successfully added 3 new rooms, seats, and 27 new showtimes.';
