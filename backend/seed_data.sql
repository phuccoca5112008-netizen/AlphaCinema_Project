USE AlphaCinema;

-- Xoa phim cu va them lai phim moi day du hon
DELETE FROM SuatChieu;
DELETE FROM Ghe;
DELETE FROM PhongChieu;
DELETE FROM Phim;

SET IDENTITY_INSERT Phim ON;
INSERT INTO Phim (ma_phim, ten_phim, the_loai, thoi_luong, trang_thai_phim, poster, tom_tat) VALUES
(1,  N'TIẾNG THÉT 7',               N'Kinh Dị',             123, N'Đang chiếu', 'https://image.tmdb.org/t/p/w500/wDWwtvkRRlgTiUr6TyLSMX8FCuZ.jpg', N'Sidney Prescott và những người bạn sống sót đối mặt Ghostface lần nữa.'),
(2,  N'DEADPOOL VÀ WOLVERINE',       N'Hành Động, Hài',      127, N'Đang chiếu', 'https://image.tmdb.org/t/p/w500/8cdWjvZQUExUUTzyp4t6EDMubfO.jpg', N'Deadpool hợp tác Wolverine cứu đa vũ trụ. Bom tấn hài hước nhất năm.'),
(3,  N'KUNG FU PANDA 4',             N'Hoạt Hình, Hài',       94, N'Đang chiếu', 'https://image.tmdb.org/t/p/w500/kDp1vUBnMpe8ak4rjgl3cLELqjU.jpg', N'Po tìm người kế nhiệm vị trí Chiến binh Thần Long.'),
(4,  N'DUNE 2 - HÀNH TINH CÁT',      N'Viễn Tưởng',          166, N'Đang chiếu', 'https://image.tmdb.org/t/p/w500/1pdfLvkbY9ohJlCjQH2CZjjYVvJ.jpg', N'Paul Atreides cùng người Fremen trả thù những kẻ phá hủy gia đình anh.'),
(5,  N'LẬT MẶT 7: MỘT ĐIỀU ƯỚC',    N'Gia Đình',             125, N'Đang chiếu', 'https://image.tmdb.org/t/p/w500/aSPg7viRKZUp6py0VLVTv6mo3GN.jpg', N'5 người con và 1 bà mẹ 73 tuổi – ai sẽ về chăm mẹ?'),
(6,  N'INSIDE OUT 2',                N'Hoạt Hình',             96, N'Đang chiếu', 'https://image.tmdb.org/t/p/w500/vpnVM9B6NMmQpWeZvzLvDESb2QY.jpg', N'Riley bước vào tuổi dậy thì cùng những cảm xúc mới lạ.'),
(7,  N'GODZILLA x KONG',             N'Hành Động',            115, N'Đang chiếu', 'https://image.tmdb.org/t/p/w500/z1p34vh7dEOnLDmyCrlUVLuoDzd.jpg', N'Kong và Godzilla liên minh chống lại thế lực khổng lồ từ lòng Trái Đất.'),
(8,  N'ALIEN: ROMULUS',              N'Kinh Dị, Viễn Tưởng', 119, N'Đang chiếu', 'https://image.tmdb.org/t/p/w500/2uSWRTtCG336nuBiG8jOTEUKSy8.jpg', N'Nhóm thanh niên gặp dạng sống nguy hiểm nhất vũ trụ.'),
(9,  N'MAI',                         N'Tình Cảm',             131, N'Sắp chiếu',  'https://image.tmdb.org/t/p/w500/uFsn8qBEzbeJCqH0LsViwPom5ww.jpg', N'Câu chuyện người phụ nữ làm nghề mát-xa đối mặt định kiến xã hội.'),
(10, N'MOANA 2',                     N'Hoạt Hình, Âm Nhạc',  100, N'Sắp chiếu',  'https://image.tmdb.org/t/p/w500/aLVkiINlIeCkcZIzb7XHzPYgO6L.jpg', N'Moana lên đường đến những vùng biển chưa từng khám phá.'),
(11, N'AVENGERS: SECRET WARS',       N'Hành Động',            180, N'Sắp chiếu',  'https://image.tmdb.org/t/p/w500/f0YBuh4hyiAheXhh4JnJWoKi9g5.jpg', N'Trận chiến của các Avenger vượt qua mọi đa vũ trụ bắt đầu.'),
(12, N'CAPTAIN AMERICA: NEW WORLD',  N'Hành Động',            118, N'Sắp chiếu',  'https://image.tmdb.org/t/p/w500/pzIddUEMWhWzfvLI3TwxUG2wGoi.jpg', N'Sam Wilson khoác lên mình tấm khiên Captain America trong kỷ nguyên mới.'),
(13, N'THUNDERBOLTS',                N'Hành Động',            127, N'Sắp chiếu',  'https://image.tmdb.org/t/p/w500/hqcexYHbiTBfDIdDWxrxPtVndBX.jpg', N'Biệt đội anti-hero tập hợp để đối phó mối đe dọa từ bên trong.'),
(14, N'FANTASTIC FOUR',              N'Hành Động',            130, N'Sắp chiếu',  'https://image.tmdb.org/t/p/w500/nf5qaSEvyYSNeFH0YhSs5EsBLX9.jpg', N'Bốn siêu anh hùng đầu tiên của MCU ra mắt chính thức.'),
(15, N'THE BATMAN 2',                N'Hành Động, Tội Phạm', 160, N'Sắp chiếu',  'https://image.tmdb.org/t/p/w500/hUe1G6Ziwl8b6DaaGHjhG6LQQH8.jpg', N'Bruce Wayne tiếp tục cuộc chiến bóng tối với những kẻ thù mới nguy hiểm hơn.'),
(16, N'AQUAMAN 3: THỦY THẦN',        N'Hành Động',            124, N'Sắp chiếu',  'https://image.tmdb.org/t/p/w500/7lTnXOy0iNtBAdRP3TZvaKJ77F6.jpg', N'Arthur Curry trở lại bảo vệ đại dương khỏi mối nguy hiểm chưa từng có.'),
(17, N'TRANSFORMERS ONE',            N'Hoạt hình, KHVT',     104, N'Đang chiếu', 'https://image.tmdb.org/t/p/w500/1RaSkWakWBxxYOWRrqmwo2my5zg.jpg', N'Câu chuyện về nguồn gốc của Optimus Prime và Megatron.'),
(18, N'ROBOT HOANG DÃ',              N'Hoạt hình, KHVT',     102, N'Đang chiếu', 'https://image.tmdb.org/t/p/w500/iRCgqpdVE4wyLQvGYU3ZP7pAtUc.jpg', N'Một robot sinh tồn trên đảo hoang và làm bạn với động vật.'),
(19, N'JOKER: FOLIE À DEUX',         N'Chính kịch, Hình sự', 138, N'Đang chiếu', 'https://image.tmdb.org/t/p/w500/if8QiqCI7WAGImKcJCfzp6VTyKA.jpg', N'Arthur Fleck tìm thấy tình yêu và âm nhạc tại Arkham.'),
(20, N'VÕ SĨ GIÁC ĐẤU II',           N'Hành động, Phiêu lưu', 148, N'Sắp chiếu',  'https://image.tmdb.org/t/p/w500/2cxhvwyEwRlysAmRH4iodkvo0z5.jpg', N'Lucius bước vào Đấu trường để khôi phục vinh quang La Mã.'),
(21, N'VENOM: KÈO CUỐI',             N'Hành động, KHVT',     109, N'Sắp chiếu',  'https://image.tmdb.org/t/p/w500/aosm8NMQ3UyoBVpSxyimorCQykC.jpg', N'Phần phim cuối cùng về Venom và Eddie Brock.');
SET IDENTITY_INSERT Phim OFF;

-- PHONG CHIEU
SET IDENTITY_INSERT PhongChieu ON;
INSERT INTO PhongChieu (ma_phong, ten_phong) VALUES (1,'IMAX 1'), (2,'VIP 2'), (3,'3D Standard'), (4,'2D Classic');
SET IDENTITY_INSERT PhongChieu OFF;

-- GHE - PHONG 1 IMAX (8 hang A-H, 12 ghe/hang)
DECLARE @h INT = 65;
WHILE @h <= 72
BEGIN
    DECLARE @s INT = 1;
    WHILE @s <= 12
    BEGIN
        DECLARE @loai NVARCHAR(20) = N'Thường';
        IF (@h >= 68 AND @h <= 71 AND @s >= 3 AND @s <= 10) SET @loai = N'VIP';
        INSERT INTO Ghe (ma_phong, hang, so_ghe, loai_ghe) VALUES (1, CHAR(@h), @s, @loai);
        SET @s += 1;
    END
    SET @h += 1;
END

-- GHE - PHONG 2 VIP (6 hang A-F, 8 ghe/hang - toan VIP)
SET @h = 65;
WHILE @h <= 70
BEGIN
    SET @s = 1;
    WHILE @s <= 8
    BEGIN
        INSERT INTO Ghe (ma_phong, hang, so_ghe, loai_ghe) VALUES (2, CHAR(@h), @s, N'VIP');
        SET @s += 1;
    END
    SET @h += 1;
END

-- GHE - PHONG 3 3D (7 hang A-G, 10 ghe/hang)
SET @h = 65;
WHILE @h <= 71
BEGIN
    SET @s = 1;
    WHILE @s <= 10
    BEGIN
        SET @loai = N'Thường';
        IF (@h >= 67 AND @h <= 70 AND @s >= 2 AND @s <= 9) SET @loai = N'VIP';
        INSERT INTO Ghe (ma_phong, hang, so_ghe, loai_ghe) VALUES (3, CHAR(@h), @s, @loai);
        SET @s += 1;
    END
    SET @h += 1;
END

-- GHE - PHONG 4 2D (6 hang A-F, 10 ghe/hang)
SET @h = 65;
WHILE @h <= 70
BEGIN
    SET @s = 1;
    WHILE @s <= 10
    BEGIN
        INSERT INTO Ghe (ma_phong, hang, so_ghe, loai_ghe) VALUES (4, CHAR(@h), @s, N'Thường');
        SET @s += 1;
    END
    SET @h += 1;
END

-- SUAT CHIEU
DECLARE @D0 DATETIME = CAST(GETDATE() AS DATE);
DECLARE @D1 DATETIME = DATEADD(day,1,@D0);
DECLARE @D2 DATETIME = DATEADD(day,2,@D0);
DECLARE @D3 DATETIME = DATEADD(day,3,@D0);

INSERT INTO SuatChieu (ma_phim, ma_phong, thoi_gian_bat_dau, dinh_dang, gia_ve_goc) VALUES
-- Phim 1
(1,1,DATEADD(hour,10,@D0),'IMAX',120000),(1,1,DATEADD(hour,14,@D0),'IMAX',120000),(1,1,DATEADD(hour,20,@D0),'IMAX',130000),
(1,3,DATEADD(hour,13,@D0),'3D',90000),(1,4,DATEADD(hour,17,@D0),'2D',75000),
(1,1,DATEADD(hour,9,@D1),'IMAX',120000),(1,3,DATEADD(hour,15,@D1),'3D',90000),
-- Phim 2
(2,2,DATEADD(hour,11,@D0),'VIP',150000),(2,2,DATEADD(hour,16,@D0),'VIP',160000),(2,2,DATEADD(hour,21,@D0),'VIP',160000),
(2,3,DATEADD(hour,14,@D0),'3D',95000),
(2,2,DATEADD(hour,14,@D1),'VIP',150000),(2,3,DATEADD(hour,19,@D1),'3D',95000),
-- Phim 3
(3,4,DATEADD(hour,10,@D0),'2D',75000),(3,4,DATEADD(hour,13,@D0),'2D',75000),(3,3,DATEADD(hour,16,@D0),'3D',90000),
(3,4,DATEADD(hour,11,@D1),'2D',75000),
-- Phim 4
(4,1,DATEADD(hour,9,@D1),'IMAX',130000),(4,1,DATEADD(hour,14,@D1),'IMAX',130000),(4,2,DATEADD(hour,19,@D1),'VIP',160000),
(4,1,DATEADD(hour,10,@D2),'IMAX',130000),
-- Phim 5
(5,4,DATEADD(hour,16,@D0),'2D',80000),(5,3,DATEADD(hour,20,@D1),'3D',90000),
-- Phim 6
(6,3,DATEADD(hour,11,@D0),'3D',90000),(6,2,DATEADD(hour,15,@D0),'VIP',150000),(6,4,DATEADD(hour,18,@D1),'2D',75000),
-- Phim 7
(7,1,DATEADD(hour,11,@D1),'IMAX',125000),(7,3,DATEADD(hour,18,@D2),'3D',95000),
-- Phim 8
(8,2,DATEADD(hour,20,@D1),'VIP',155000),(8,3,DATEADD(hour,21,@D2),'3D',90000);

PRINT N'Done! 16 phim, 4 phong, suat chieu day du.';
