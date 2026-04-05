USE AlphaCinema;

-- 1. ADD MORE ROOMS (Starting from ID 9)
SET IDENTITY_INSERT PhongChieu ON;
INSERT INTO PhongChieu (ma_phong, ten_phong) VALUES 
(9,  N'Gold Class 9'), 
(10, N'Bed Cinema 10'), 
(11, N'ScreenX 11');
SET IDENTITY_INSERT PhongChieu OFF;

-- 2. ADD SEATS FOR NEW ROOMS
-- Room 9 Gold Class (5 rows, 6 seats each)
DECLARE @h INT = 65;
WHILE @h <= 69
BEGIN
    DECLARE @s INT = 1;
    WHILE @s <= 6
    BEGIN
        INSERT INTO Ghe (ma_phong, hang, so_ghe, loai_ghe) VALUES (9, CHAR(@h), @s, N'VIP');
        SET @s += 1;
    END
    SET @h += 1;
END

-- Room 10 Bed Cinema (4 rows, 4 seats each)
SET @h = 65;
WHILE @h <= 68
BEGIN
    SET @s = 1;
    WHILE @s <= 4
    BEGIN
        INSERT INTO Ghe (ma_phong, hang, so_ghe, loai_ghe) VALUES (10, CHAR(@h), @s, N'VIP');
        SET @s += 1;
    END
    SET @h += 1;
END

-- Room 11 ScreenX (8 rows, 14 seats each)
SET @h = 65;
WHILE @h <= 72
BEGIN
    SET @s = 1;
    WHILE @s <= 14
    BEGIN
        DECLARE @loai NVARCHAR(20) = N'Thường';
        IF (@h >= 67 AND @h <= 71 AND @s >= 4 AND @s <= 11) SET @loai = N'VIP';
        INSERT INTO Ghe (ma_phong, hang, so_ghe, loai_ghe) VALUES (11, CHAR(@h), @s, @loai);
        SET @s += 1;
    END
    SET @h += 1;
END

-- 3. ADD MANY MORE SHOWTIMES (TODAY AND NEXT 3 DAYS)
DECLARE @D0 DATETIME = CAST(GETDATE() AS DATE);
DECLARE @D1 DATETIME = DATEADD(day,1,@D0);
DECLARE @D2 DATETIME = DATEADD(day,2,@D0);

-- Room 9 (Today + Tomorrow)
INSERT INTO SuatChieu (ma_phim, ma_phong, thoi_gian_bat_dau, dinh_dang, gia_ve_goc) VALUES
(17, 9, DATEADD(hour,19,@D0), '2D', 250000),
(11, 9, DATEADD(hour,21,@D0), '2D', 300000),
(4,  9, DATEADD(hour,15,@D1), '2D', 250000),
(2,  9, DATEADD(hour,18,@D1), '2D', 250000);

-- Room 10 (Bed Cinema)
INSERT INTO SuatChieu (ma_phim, ma_phong, thoi_gian_bat_dau, dinh_dang, gia_ve_goc) VALUES
(9,  10, DATEADD(hour,20,@D0), '2D', 400000),
(19, 10, DATEADD(hour,19,@D1), '2D', 450000),
(2,  10, DATEADD(hour,22,@D1), '2D', 400000);

-- Room 11 (ScreenX)
INSERT INTO SuatChieu (ma_phim, ma_phong, thoi_gian_bat_dau, dinh_dang, gia_ve_goc) VALUES
(7,  11, DATEADD(hour,10,@D0), 'ScreenX', 150000),
(21, 11, DATEADD(hour,13,@D0), 'ScreenX', 180000),
(17, 11, DATEADD(hour,16,@D0), 'ScreenX', 160000),
(11, 11, DATEADD(hour,19,@D0), 'ScreenX', 200000),
(7,  11, DATEADD(hour,11,@D1), 'ScreenX', 150000),
(21, 11, DATEADD(hour,14,@D1), 'ScreenX', 180000);

-- Room 7 (ALPHA IMAX) - Extra showtimes
INSERT INTO SuatChieu (ma_phim, ma_phong, thoi_gian_bat_dau, dinh_dang, gia_ve_goc) VALUES
(4,  7, DATEADD(hour,9,@D0),  'IMAX', 130000),
(4,  7, DATEADD(hour,14,@D0), 'IMAX', 130000),
(11, 7, DATEADD(hour,19,@D0), 'IMAX', 150000);

PRINT 'SUCCESS: Seeded Rooms 9, 10, 11 and 16 new showtimes.';
