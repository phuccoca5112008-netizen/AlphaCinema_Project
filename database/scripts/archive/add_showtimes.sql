USE AlphaCinema;

DECLARE @D0 DATETIME = CAST(GETDATE() AS DATE);
DECLARE @D1 DATETIME = DATEADD(day,1,@D0);
DECLARE @D2 DATETIME = DATEADD(day,2,@D0);
DECLARE @D3 DATETIME = DATEADD(day,3,@D0);

-- Xóa bớt suất cũ nếu cần, hoặc chỉ thêm mới. Tôi sẽ thêm mới.

INSERT INTO SuatChieu (ma_phim, ma_phong, thoi_gian_bat_dau, dinh_dang, gia_ve_goc) VALUES
-- Transformers One (17)
(17,1,DATEADD(hour,10,@D0),'IMAX',120000), (17,3,DATEADD(hour,15,@D0),'3D',95000), (17,4,DATEADD(hour,19,@D0),'2D',75000),
(17,1,DATEADD(hour,14,@D1),'IMAX',125000), (17,4,DATEADD(hour,21,@D1),'2D',80000),

-- Robot Hoang Dã (18)
(18,3,DATEADD(hour,9,@D0),'3D',90000), (18,4,DATEADD(hour,14,@D0),'2D',70000),
(18,3,DATEADD(hour,11,@D1),'3D',95000), (18,2,DATEADD(hour,17,@D1),'VIP',150000),

-- Joker: Folie à Deux (19)
(19,2,DATEADD(hour,20,@D0),'VIP',160000), (19,3,DATEADD(hour,17,@D1),'3D',100000),
(19,2,DATEADD(hour,21,@D2),'VIP',160000),

-- Dune 2 (4) - thêm suất cho D0
(4,1,DATEADD(hour,21,@D0),'IMAX',140000),

-- Inside Out 2 (6) - thêm suất cho D1
(6,4,DATEADD(hour,10,@D1),'2D',75000),

-- Tiếng thét 7 (1) - cho ngày D2
(1,3,DATEADD(hour,22,@D2),'3D',100000);

PRINT 'Lịch chiếu mới đã được bổ sung thành công!';
