USE AlphaCinema;
GO

-- 1. Xóa hết dữ liệu cũ để reset lại cho chuẩn UTF-8
DELETE FROM DoAnVat;

-- 2. Đảm bảo các cột là NVARCHAR để hỗ trợ tiếng Việt
ALTER TABLE DoAnVat ALTER COLUMN ten_mon NVARCHAR(200) NOT NULL;
ALTER TABLE DoAnVat ALTER COLUMN mo_ta NVARCHAR(500);
ALTER TABLE DoAnVat ALTER COLUMN loai NVARCHAR(50);

-- 3. Reset IDENTITY nếu cần (tùy chọn)
DBCC CHECKIDENT ('DoAnVat', RESEED, 0);

-- 4. Chèn lại dữ liệu với tiền tố N'' để hỗ trợ Unicode
INSERT INTO DoAnVat (ten_mon, mo_ta, gia, hinh_anh, loai)
VALUES 
(N'Bắp Rang Ngọt (M)', N'Bắp rang vị caramel truyền thống, size vừa', 55000, 'https://cdn.shopify.com/s/files/1/0212/6126/products/Caramel-Popcorn-Cello-Bag_1024x1024.jpg?v=1543354395', N'Bắp'),
(N'Bắp Rang Phô Mai (L)', N'Bắp rang vị phô mai đậm đà, size lớn', 75000, 'https://www.thespruceeats.com/thmb/hveK9i-TDPxWdZzC8m_Yw8S6IQA=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/cheese-popcorn-recipe-1328723-hero-01-f0b1b1b1b1b14b1b1b1b1b1b1b1b1b1b.jpg', N'Bắp'),
(N'Coca Cola (L)', N'Nước giải khát có gas lạnh, size lớn', 39000, 'https://m.media-amazon.com/images/I/51v8ny56pRL._SL1500_.jpg', N'Nước'),
(N'Nước Cam Tươi', N'Nước cam ép nguyên chất, giàu vitamin C', 45000, 'https://images.unsplash.com/photo-1621506289937-41a457494ce8?q=80&w=1000&auto=format&fit=crop', N'Nước'),
(N'Pizza Xúc Xích Mini', N'Pizza đế mỏng, xúc xích và phô mai nóng hổi', 95000, 'https://media.istockphoto.com/id/1442417585/photo/person-getting-a-piece-of-cheesy-pepperoni-pizza.jpg?s=612x612&w=0&k=20&c=S_79OnSIn97N2T8ZSaO_N-OAtA_FmCpgvA7UUp07X0Q=', N'Thức ăn'),
(N'Mì Trộn Tương Đen', N'Mì trộn chuẩn vị Hàn Quốc kèm trứng', 75000, 'https://i.ytimg.com/vi/qYJpS7lU7tI/maxresdefault.jpg', N'Thức ăn'),
(N'Combo Single Alpha', N'1 Bắp (M) + 1 Nước (L)', 85000, 'https://www.lottecinemavn.com/LCC/Image/Image_Concession/CON120191219154323_0.png', N'Combo'),
(N'Combo Couple Alpha', N'1 Bắp (L) + 2 Nước (L)', 135000, 'https://www.lottecinemavn.com/LCC/Image/Image_Concession/CON120191219154347_0.png', N'Combo');
GO
