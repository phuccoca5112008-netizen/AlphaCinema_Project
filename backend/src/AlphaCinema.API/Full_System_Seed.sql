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
DELETE FROM DanhGia;
DELETE FROM HoaDonDoAnVat;
DELETE FROM HoaDon;
DELETE FROM SuatChieu;
DELETE FROM Ghe;
DELETE FROM PhongChieu;
DELETE FROM Phim;
DELETE FROM KhuyenMai;
DELETE FROM DoAnVat;
DELETE FROM NguoiDung;
DELETE FROM VaiTro;
GO

-- 1. VAI TRÒ (Roles)
-- Sử dụng MERGE hoặc check tồn tại để tránh trùng lặp nếu không muốn xóa hết
INSERT INTO VaiTro (ten_vai_tro) VALUES (N'Admin'), (N'Staff'), (N'Customer');
GO

-- Lấy ID động để tránh lỗi Foreign Key nếu Identity không bắt đầu từ 1
DECLARE @AdminId INT = (SELECT TOP 1 ma_vai_tro FROM VaiTro WHERE ten_vai_tro = N'Admin');
DECLARE @StaffId INT = (SELECT TOP 1 ma_vai_tro FROM VaiTro WHERE ten_vai_tro = N'Staff');
DECLARE @CustomerId INT = (SELECT TOP 1 ma_vai_tro FROM VaiTro WHERE ten_vai_tro = N'Customer');

-- 2. NGƯỜI DÙNG (Users)
INSERT INTO NguoiDung (ma_vai_tro, email, mat_khau, ho_ten, diem_tich_luy) VALUES
(@AdminId, 'admin@alpha.com', 'AQAAAAEAACcQAAAAEPvAIP6W7V6/T9O7oN1...', N'Quản Trị Viên', 1000),
(@StaffId, 'staff@alpha.com', 'AQAAAAEAACcQAAAAEPvAIP6W7V6/T9O7oN1...', N'Nhân Viên Alpha', 0),
(@CustomerId, 'customer@alpha.com', 'AQAAAAEAACcQAAAAEPvAIP6W7V6/T9O7oN1...', N'Trần Phước Tín', 50);
GO

-- 3. PHIM (Movies)
INSERT INTO Phim (ten_phim, the_loai, thoi_luong, poster, tom_tat, trang_thai_phim) VALUES
(N'Dune: Hành Tinh Cát - Phần 2', N'Hành Động, Viễn Tưởng', 166, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/original/6izwz7rsy95ARzTR3poZ8H6c5pp.jpg', N'Paul Atreides tiếp tục hành trình trả thù những kẻ đã hủy diệt gia đình mình...', N'Đang chiếu'),
(N'Deadpool & Wolverine', N'Hành Động, Hài', 127, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/w500/8cdWjvZQUExUUTzyp4t6EDMubfO.jpg', N'Một nhiệm vụ giải cứu đa vũ trụ đầy tiếng cười và bạo lực của bộ đôi lầy lội nhất Marvel...', N'Đang chiếu'),
(N'Kung Fu Panda 4', N'Hoạt Hình, Hài', 94, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/w500/kDp1vUBnMpe8ak4rjgl3cLELqjU.jpg', N'Po chuẩn bị trở thành Thủ lĩnh tinh thần của Thung lũng Hòa bình...', N'Đang chiếu'),
(N'Oppenheimer', N'Lịch sử, Tâm lý', 180, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/w500/8Gxv8gSFCU0XGDykEGv7zR1n2ua.jpg', N'Câu chuyện về cha đẻ của bom nguyên tử J. Robert Oppenheimer...', N'Đang chiếu'),
(N'Interstellar: Hố Đen Tử Thần', N'Khoa Học, Phiêu Lưu', 169, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/w500/yQvGrMoipbRoddT0ZR8tPoR7NfX.jpg', N'Một nhóm các nhà thám hiểm du hành xuyên qua một lỗ đen ngoài không gian để cứu nhân loại...', N'Đang chiếu'),
(N'Moana 2', N'Hoạt Hình, Phiêu Lưu', 100, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/w500/aLVkiINlIeCkcZIzb7XHzPYgO6L.jpg', N'Moana và Maui tái hợp trong một hành trình mới đầy kỳ thú vượt đại dương...', N'Đang chiếu'),
(N'Sonic the Hedgehog 3', N'Gia Đình, Hành Động', 110, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/w500/d8Ryb8AunYAuycVKDp5HpdWPKgC.jpg', N'Sonic, Tails và Knuckles phải đối mặt với một đối thủ mới mạnh mẽ: Shadow...', N'Sắp chiếu'),
(N'Spider-Man: Beyond the Spider-Verse', N'Hoạt Hình, Hành Động', 140, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/w500/9PIhQqqI6Q4a5YjwMjxvzZcPJhf.jpg', N'Phần kết đầy kịch tính cho hành trình của Miles Morales qua đa vũ trụ nhện...', N'Sắp chiếu'),
(N'Joker: Folie à Deux', N'Tâm Lý, Nhạc Kịch', 138, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/w500/aciP8Km0waTLXEYf5ybFK5CSUxl.jpg', N'Cuộc gặp gỡ định mệnh giữa Arthur Fleck và Harley Quinn tại Arkham...', N'Sắp chiếu'),
(N'Gladiator II', N'Hành Động, Sử Thi', 148, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/w500/2cxhvwyEwRlysAmRH4iodkvo0z5.jpg', N'Nhiều năm sau cái chết của Maximus, Lucius buộc phải bước vào Đấu trường La Mã...', N'Sắp chiếu'),
(N'Despicable Me 4', N'Hoạt Hình, Hài', 95, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/w500/wWba3TaojhK7NdycRhoQpsG0FaH.jpg', N'Gru và gia đình chào đón thành viên mới trong khi phải đối mặt với kẻ thù mới Maximus Le Mal...', N'Đang chiếu'),
(N'Beetlejuice Beetlejuice', N'Hài, Kinh Dị', 104, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/w500/kKgQzkUCnQmeTPkyIwHly2t6ZFI.jpg', N'Sau ba thập kỷ, Beetlejuice trở lại để ám ảnh gia đình Deetz một lần nữa...', N'Đang chiếu'),
(N'Wicked', N'Fantasy, Drama, Musical', 160, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/original/j5AVKdTm8mG7tjoxIPU5V7riaDy.jpg', N'Câu chuyện xoay quanh Elphaba và Glinda trước khi họ trở thành những phù thủy nổi tiếng của Oz...', N'Đang chiếu'),
(N'Venom: The Last Dance', N'Hành Động, Viễn Tưởng', 110, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/w500/aosm8NMQ3UyoBVpSxyimorCQykC.jpg', N'Eddie và Venom phải chạy trốn khi bị truy đuổi bởi cả hai thế giới.', N'Đang chiếu'),
(N'Smile 2', N'Kinh Dị, Bí Ẩn', 127, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/w500/ht8Uv9QPv9y7K0RvUyJIaXOZTfd.jpg', N'Ngôi sao nhạc pop Skye Riley bắt đầu trải qua những sự kiện kinh hoàng trước chuyến lưu diễn.', N'Đang chiếu'),
(N'The Wild Robot', N'Hoạt Hình, Viễn Tưởng', 102, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/w500/eG9lz41mJqsI4J6ubMtVqD26q2J.jpg', N'Robot Roz bị trôi dạt vào đảo hoang và phải học cách thích nghi với thiên nhiên.', N'Đang chiếu'),
(N'Terrifier 3', N'Kinh Dị', 125, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/w500/ju10W5gl3PPK3b7TjEmVOZap51I.jpg', N'Tên hề Art trở lại để gieo rắc nỗi kinh hoàng ngay trong đêm Giáng sinh.', N'Đang chiếu'),
(N'Avatar: Fire and Ash', N'Hành Động, Viễn Tưởng', 180, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/w500/bRBeSHfGHwkEpImlhxPmOcUsaeg.jpg', N'Gia đình Sully đối mặt với bộ tộc Người Tro hung hãn trên Pandora.', N'Sắp chiếu'),
(N'Captain America: Brave New World', N'Hành Động, Viễn Tưởng', 130, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/w500/pzIddUEMWhWzfvLI3TwxUG2wGoi.jpg', N'Sam Wilson dấn thân vào một âm mưu quốc tế nguy hiểm trong vai trò Captain America.', N'Sắp chiếu'),
(N'Mickey 17', N'Viễn Tưởng, Hài', 120, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/w500/edKpE9B5qN3e559OuMCLZdW1iBZ.jpg', N'Mickey 17 là một nhân viên có thể thay thế trong chuyến hành trình thuộc địa hóa thế giới...', N'Sắp chiếu'),
(N'Mission: Impossible - Final Reckoning', N'Hành Động, Thriller', 150, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/w500/z53D72EAOxGRqdr7KXXWp9dJiDe.jpg', N'Ethan Hunt và đội của anh truy lùng trí tuệ nhân tạo đáng sợ mang tên The Entity.', N'Sắp chiếu'),
(N'The Fantastic Four: First Steps', N'Hành Động, Phiêu Lưu', 130, 'https://wsrv.nl/?url=https://image.tmdb.org/t/p/w500/nf5qaSEvyYSNeFH0YhSs5EsBLX9.jpg', N'Gia đình siêu anh hùng đầu tiên của Marvel đối mặt với chúa tể Galactus rình rập Trái đất.', N'Sắp chiếu');
GO

-- 4. PHÒNG CHIẾU & GHẾ (Rooms & Seats)
INSERT INTO PhongChieu (ten_phong, loai_phong) VALUES 
(N'P1 - IMAX', N'IMAX'), (N'P2 - PREMIUM', N'VIP'), (N'P3 - STANDARD', N'2D');

DECLARE @R1 INT = (SELECT ma_phong FROM PhongChieu WHERE ten_phong = N'P1 - IMAX');
DECLARE @R2 INT = (SELECT ma_phong FROM PhongChieu WHERE ten_phong = N'P2 - PREMIUM');
DECLARE @R3 INT = (SELECT ma_phong FROM PhongChieu WHERE ten_phong = N'P3 - STANDARD');

-- Ghế cho các phòng
DECLARE @Rooms TABLE (MaPhong INT);
INSERT INTO @Rooms VALUES (@R1), (@R2), (@R3);

DECLARE @CurrentRoom INT;
DECLARE RoomCursor CURSOR FOR SELECT MaPhong FROM @Rooms;
OPEN RoomCursor;
FETCH NEXT FROM RoomCursor INTO @CurrentRoom;

WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE @H CHAR(1) = 'A';
    WHILE ASCII(@H) <= ASCII('D')
    BEGIN
        DECLARE @S INT = 1;
        WHILE @S <= 8
        BEGIN
            INSERT INTO Ghe (ma_phong, hang, so_ghe, loai_ghe) 
            VALUES (@CurrentRoom, @H, @S, CASE WHEN @S BETWEEN 3 AND 6 THEN 'VIP' ELSE 'Thường' END);
            SET @S = @S + 1;
        END
        SET @H = CHAR(ASCII(@H) + 1);
    END
    FETCH NEXT FROM RoomCursor INTO @CurrentRoom;
END
CLOSE RoomCursor;
DEALLOCATE RoomCursor;
GO

-- 5. ĐỒ ĂN VẶT (Concessions)
INSERT INTO DoAnVat (ten_mon, mo_ta, gia, hinh_anh, loai) VALUES 
(N'Bắp Rang Ngọt (M)', N'Bắp thơm ngon size vừa', 45000, '/assets/concessions/bap_caramel_m.png', N'Bắp'),
(N'Bắp Rang Phô Mai (L)', N'Bắp mặn phô mai size lớn', 60000, '/assets/concessions/bap_phomai_l.png', N'Bắp'),
(N'Coca Cola (L)', N'Nước ngọt ga mạnh', 35000, '/assets/concessions/coca_l.png', N'Nước'),
(N'Combo Single Alpha', N'1 Bắp (M) + 1 Nước (L)', 80000, '/assets/concessions/combo_single.png', N'Combo'),
(N'Combo Double Alpha', N'1 Bắp (L) + 2 Nước (L)', 115000, '/assets/concessions/combo_double.png', N'Combo');
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

-- 7. SUẤT CHIẾU (Showtimes)
DECLARE @P1 INT = (SELECT ma_phong FROM PhongChieu WHERE ten_phong = N'P1 - IMAX');
DECLARE @P2 INT = (SELECT ma_phong FROM PhongChieu WHERE ten_phong = N'P2 - PREMIUM');
DECLARE @P3 INT = (SELECT ma_phong FROM PhongChieu WHERE ten_phong = N'P3 - STANDARD');

DECLARE @MovieID INT;
DECLARE MovieCursor CURSOR FOR 
    SELECT ma_phim FROM Phim WHERE trang_thai_phim = N'Đang chiếu';

OPEN MovieCursor;
FETCH NEXT FROM MovieCursor INTO @MovieID;

DECLARE @Counter INT = 0;

WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE @TargetRoom INT = CASE 
        WHEN @Counter % 3 = 0 THEN @P1 
        WHEN @Counter % 3 = 1 THEN @P2 
        ELSE @P3 END;
        
    DECLARE @Format NVARCHAR(20) = CASE 
        WHEN @TargetRoom = @P1 THEN 'IMAX' 
        WHEN @TargetRoom = @P2 THEN '3D' 
        ELSE '2D' END;

    INSERT INTO SuatChieu (ma_phim, ma_phong, thoi_gian_bat_dau, dinh_dang, gia_ve_goc) VALUES
    (@MovieID, @TargetRoom, DATEADD(HOUR, 8 + (@Counter % 12), CAST(GETDATE() AS DATETIME)), @Format, 120000);

    SET @Counter = @Counter + 1;
    FETCH NEXT FROM MovieCursor INTO @MovieID;
END

CLOSE MovieCursor;
DEALLOCATE MovieCursor;
GO

SELECT 'DATABASE SEEDED SUCCESSFULLY!' AS Result;
GO
