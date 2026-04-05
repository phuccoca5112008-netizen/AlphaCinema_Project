
USE AlphaCinema;
GO

BEGIN TRANSACTION;

-- Xanh sạch đẹp: Xóa liên kết trước (nếu có dữ liệu rác trong dev)
DELETE FROM HoaDonDoAnVat WHERE ma_do_an_vat IN (1,2,3,4,5,6,7,8,9,10,11);
DELETE FROM DoAnVat;

-- Chèn lại dữ liệu với tiền tố N chuẩn UTF-8
SET IDENTITY_INSERT DoAnVat ON;

INSERT INTO DoAnVat (ma_do_an_vat, ten_mon, mo_ta, gia, loai, hinh_anh) VALUES
(1, N'Bắp Rang Ngọt Caramel (M)', N'Bắp rang vị caramel thơm ngọt size vừa', 45000, N'Bắp', N'/assets/concessions/bap_caramel_m.png'),
(2, N'Bắp Rang Ngọt Caramel (L)', N'Bắp rang vị caramel thơm ngọt size lớn', 55000, N'Bắp', N'/assets/concessions/bap_caramel_m.png'),
(3, N'Bắp Rang Phô Mai (M)', N'Bắp rang vị phô mai mặn mà size vừa', 45000, N'Bắp', N'/assets/concessions/bap_phomai_l.png'),
(4, N'Bắp Rang Phô Mai (L)', N'Bắp rang vị phô mai mặn mà size lớn', 55000, N'Bắp', N'/assets/concessions/bap_phomai_l.png'),
(5, N'Coca Cola (L)', N'Nước ngọt Coca Cola mát lạnh size lớn', 35000, N'Nước', N'/assets/concessions/coca_l.png'),
(6, N'Pepsi (L)', N'Nước ngọt Pepsi mát lạnh size lớn', 35000, N'Nước', N'/assets/concessions/coca_l.png'),
(7, N'Nước Cam Tươi', N'Nước cam ép nguyên chất giàu vitamin', 45000, N'Nước', N'/assets/concessions/cam_tuoi.png'),
(8, N'Milo Trà Xanh Đậu Đen', N'Thức uống sáng tạo kết hợp Milo và Trà xanh', 50000, N'Nước', N'/assets/concessions/milo_trachanh.png'),
(9, N'Pizza Xúc Xích Mini', N'Pizza cỡ nhỏ phủ xúc xích và phô mai', 75000, N'Thức ăn', N'/assets/concessions/pizza_mini.png'),
(10, N'Combo Single Alpha', N'1 Bắp (M) + 1 Nước (L) - Tiết kiệm hơn', 75000, N'Combo', N'/assets/concessions/combo_single.png'),
(11, N'Combo Couple Alpha', N'1 Bắp (L) + 2 Nước (L) - Cho cặp đôi', 115000, N'Combo', N'/assets/concessions/combo_couple.png');

SET IDENTITY_INSERT DoAnVat OFF;

COMMIT TRANSACTION;
GO
