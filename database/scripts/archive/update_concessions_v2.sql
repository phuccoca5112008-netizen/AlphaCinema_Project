-- Update concessions list with variety including snacks and light food
SET IDENTITY_INSERT DoAnVat ON;

-- Clear old data if needed or just add more (assuming they might already exist, we'll check)
-- This is a one-time setup script or can be run manually.
IF EXISTS (SELECT * FROM DoAnVat WHERE ma_do_an_vat <= 6)
BEGIN
    DELETE FROM DoAnVat WHERE ma_do_an_vat > 0;
END

INSERT INTO DoAnVat (ma_do_an_vat, ten_mon, mo_ta, gia, loai, hinh_anh) VALUES 
-- Categories: Bắp, Nước, Combo, Thức ăn
(1, N'Bắp Caramel (Size M)', N'Bắp rang vị caramel ngọt lịm size vừa', 55000, N'Bắp', 'https://cdn.pixabay.com/photo/2024/02/09/16/09/ai-generated-8563319_1280.jpg'),
(2, N'Bắp Phô Mai (Size L)', N'Bắp rang vị phô mai mặn béo size lớn', 75000, N'Bắp', 'https://cdn.pixabay.com/photo/2024/02/09/16/10/ai-generated-8563321_1280.png'),
(3, N'Coca Cola (Size L)', N'Nước giải khát có ga Coca Cola mát lạnh', 39000, N'Nước', 'https://cdn.pixabay.com/photo/2024/04/13/11/39/ai-generated-8693766_1280.jpg'),
(4, N'Pepsi (Size L)', N'Nước giải khát có ga Pepsi sảng khoái', 39000, N'Nước', 'https://cdn.pixabay.com/photo/2024/04/13/11/39/ai-generated-8693766_1280.jpg'),
(5, N'Nước Cam Tươi', N'Nước cam nguyên chất từ cam sành', 45000, N'Nước', 'https://cdn.pixabay.com/photo/2023/07/26/15/34/orange-juice-8151475_1280.jpg'),
(6, N'Combo Single Alpha', N'1 Bắp M + 1 Nước L tự chọn', 85000, N'Combo', 'https://cdn.pixabay.com/photo/2022/01/05/20/06/movie-6918076_1280.jpg'),
(7, N'Combo Couple Alpha', N'1 Bắp L + 2 Nước L tự chọn', 135000, N'Combo', 'https://cdn.pixabay.com/photo/2022/01/05/20/06/movie-6918076_1280.jpg'),
(8, N'Pizza Xúc Xích Mini', N'Pizza đế mỏng, xúc xích and phô mai (Phục vụ nóng)', 95000, N'Thức ăn', 'https://cdn.pixabay.com/photo/2023/12/12/18/31/ai-generated-8445722_1280.jpg'),
(9, N'Mì Trộn Tương Đen', N'Mì trộn kiểu Hàn Quốc kèm trứng and rau củ', 75000, N'Thức ăn', 'https://cdn.pixabay.com/photo/2020/02/15/20/38/noodles-4851917_1280.jpg'),
(10, N'Bánh Mì Que Hải Phòng', N'Set 3 bánh mì que pate thơm nồng', 45000, N'Thức ăn', 'https://cdn.pixabay.com/photo/2024/05/26/13/50/bread-8788771_1280.jpg');

SET IDENTITY_INSERT DoAnVat OFF;
