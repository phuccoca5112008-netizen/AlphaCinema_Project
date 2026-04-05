USE AlphaCinema;

DECLARE @D0 DATETIME = CAST(GETDATE() AS DATE);
DECLARE @D1 DATETIME = DATEADD(day,1,@D0);
DECLARE @D2 DATETIME = DATEADD(day,2,@D0);
DECLARE @D3 DATETIME = DATEADD(day,3,@D0);

INSERT INTO SuatChieu (ma_phim, ma_phong, thoi_gian_bat_dau, dinh_dang, gia_ve_goc) VALUES
-- Deadpool 2 (2)
(2,1,DATEADD(hour,16,@D2),'IMAX',130000), (2,2,DATEADD(hour,20,@D3),'VIP',165000),

-- Kung Fu Panda 4 (3)
(3,4,DATEADD(hour,9,@D2),'2D',75000), (3,4,DATEADD(hour,11,@D3),'2D',75000),

-- Lật Mặt 7 (5)
(5,4,DATEADD(hour,14,@D1),'2D',85000), (5,4,DATEADD(hour,20,@D2),'2D',85000),

-- Godzilla x Kong (7)
(7,1,DATEADD(hour,13,@D3),'IMAX',130000), (7,3,DATEADD(hour,19,@D3),'3D',100000),

-- Alien: Romulus (8)
(8,2,DATEADD(hour,22,@D0),'VIP',160000), (8,2,DATEADD(hour,22,@D1),'VIP',160000);

PRINT 'Lịch chiếu thêm cho các phim cũ đã được bổ sung thành công!';
