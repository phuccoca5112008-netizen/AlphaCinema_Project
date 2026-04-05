IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DoAnVat')
BEGIN
    CREATE TABLE DoAnVat (
        ma_do_an_vat INT IDENTITY(1,1) PRIMARY KEY,
        ten_mon NVARCHAR(100) NOT NULL,
        mo_ta NVARCHAR(500),
        gia DECIMAL(18,2) NOT NULL,
        hinh_anh NVARCHAR(500),
        loai NVARCHAR(50)
    );
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HoaDonDoAnVat')
BEGIN
    CREATE TABLE HoaDonDoAnVat (
        ma_hoa_don_do_an_vat INT IDENTITY(1,1) PRIMARY KEY,
        ma_hoa_don INT NOT NULL,
        ma_do_an_vat INT NOT NULL,
        so_luong INT NOT NULL,
        don_gia DECIMAL(18,2) NOT NULL,
        CONSTRAINT FK_HoaDonDoAnVat_HoaDon FOREIGN KEY (ma_hoa_don) REFERENCES HoaDon(ma_hoa_don) ON DELETE CASCADE,
        CONSTRAINT FK_HoaDonDoAnVat_DoAnVat FOREIGN KEY (ma_do_an_vat) REFERENCES DoAnVat(ma_do_an_vat)
    );
END

-- Seed bắp nước
IF NOT EXISTS (SELECT * FROM DoAnVat)
BEGIN
    INSERT INTO DoAnVat (ten_mon, mo_ta, gia, loai, hinh_anh) VALUES 
    (N'Bắp rang ngọt Caramel M', N'Bắp rang vị caramel size vừa', 45000, N'Bắp', '/assets/concessions/bap_m.png'),
    (N'Bắp rang ngọt Caramel L', N'Bắp rang vị caramel size lớn', 55000, N'Bắp', '/assets/concessions/bap_l.png'),
    (N'Coca Cola L', N'Nước ngọt Coca Cola size lớn', 35000, N'Nước', '/assets/concessions/coca_l.png'),
    (N'Pepsi L', N'Nước ngọt Pepsi size lớn', 35000, N'Nước', '/assets/concessions/pepsi_l.png'),
    (N'Combo Đơn Alpha', N'1 Bắp M + 1 Nước L', 75000, N'Combo', '/assets/concessions/combo_1.png'),
    (N'Combo Đôi Alpha', N'1 Bắp L + 2 Nước L', 115000, N'Combo', '/assets/concessions/combo_2.png');
END
