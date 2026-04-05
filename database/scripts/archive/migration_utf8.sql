Build started...
Build succeeded.
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [KhuyenMai] (
    [ma_khuyen_mai] int NOT NULL IDENTITY,
    [ten_khuyen_mai] nvarchar(255) NOT NULL,
    [ma_code_giam_gia] nvarchar(50) NOT NULL,
    [mo_ta] nvarchar(max) NULL,
    [ngay_bat_dau] date NOT NULL,
    [ngay_ket_thuc] date NOT NULL,
    [loai_giam_gia] nvarchar(50) NOT NULL,
    [gia_tri_giam] decimal(18,2) NOT NULL,
    [giam_toi_da] decimal(18,2) NULL,
    [don_hang_toi_thieu] decimal(18,2) NULL,
    CONSTRAINT [PK_KhuyenMai] PRIMARY KEY ([ma_khuyen_mai])
);

CREATE TABLE [Phim] (
    [ma_phim] int NOT NULL IDENTITY,
    [ten_phim] nvarchar(255) NOT NULL,
    [the_loai] nvarchar(100) NOT NULL,
    [thoi_luong] int NOT NULL,
    [poster] nvarchar(255) NULL,
    [tom_tat] nvarchar(max) NULL,
    [trang_thai_phim] nvarchar(50) NOT NULL DEFAULT N'Sáº¯p chiáº¿u',
    CONSTRAINT [PK_Phim] PRIMARY KEY ([ma_phim])
);

CREATE TABLE [PhongChieu] (
    [ma_phong] int NOT NULL IDENTITY,
    [ten_phong] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_PhongChieu] PRIMARY KEY ([ma_phong])
);

CREATE TABLE [VaiTro] (
    [ma_vai_tro] int NOT NULL IDENTITY,
    [ten_vai_tro] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_VaiTro] PRIMARY KEY ([ma_vai_tro])
);

CREATE TABLE [Ghe] (
    [ma_ghe] int NOT NULL IDENTITY,
    [ma_phong] int NOT NULL,
    [hang] char(1) NOT NULL,
    [so_ghe] int NOT NULL,
    [loai_ghe] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_Ghe] PRIMARY KEY ([ma_ghe]),
    CONSTRAINT [FK_Ghe_PhongChieu_ma_phong] FOREIGN KEY ([ma_phong]) REFERENCES [PhongChieu] ([ma_phong]) ON DELETE NO ACTION
);

CREATE TABLE [SuatChieu] (
    [ma_suat_chieu] int NOT NULL IDENTITY,
    [ma_phong] int NOT NULL,
    [ma_phim] int NOT NULL,
    [thoi_gian_bat_dau] datetime2 NOT NULL,
    [dinh_dang] nvarchar(10) NOT NULL,
    [gia_ve_goc] decimal(18,2) NOT NULL DEFAULT 0.0,
    CONSTRAINT [PK_SuatChieu] PRIMARY KEY ([ma_suat_chieu]),
    CONSTRAINT [FK_SuatChieu_Phim_ma_phim] FOREIGN KEY ([ma_phim]) REFERENCES [Phim] ([ma_phim]) ON DELETE NO ACTION,
    CONSTRAINT [FK_SuatChieu_PhongChieu_ma_phong] FOREIGN KEY ([ma_phong]) REFERENCES [PhongChieu] ([ma_phong]) ON DELETE NO ACTION
);

CREATE TABLE [NguoiDung] (
    [ma_nguoi_dung] int NOT NULL IDENTITY,
    [ma_vai_tro] int NOT NULL,
    [email] nvarchar(100) NOT NULL,
    [mat_khau] nvarchar(255) NOT NULL,
    [ho_ten] nvarchar(100) NOT NULL,
    [diem_tich_luy] int NOT NULL DEFAULT 0,
    CONSTRAINT [PK_NguoiDung] PRIMARY KEY ([ma_nguoi_dung]),
    CONSTRAINT [FK_NguoiDung_VaiTro_ma_vai_tro] FOREIGN KEY ([ma_vai_tro]) REFERENCES [VaiTro] ([ma_vai_tro]) ON DELETE NO ACTION
);

CREATE TABLE [DanhGia] (
    [ma_danh_gia] int NOT NULL IDENTITY,
    [ma_phim] int NOT NULL,
    [ma_nguoi_dung] int NOT NULL,
    [noi_dung] nvarchar(max) NULL,
    [diem_so] int NOT NULL,
    [ngay_tao] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_DanhGia] PRIMARY KEY ([ma_danh_gia]),
    CONSTRAINT [FK_DanhGia_NguoiDung_ma_nguoi_dung] FOREIGN KEY ([ma_nguoi_dung]) REFERENCES [NguoiDung] ([ma_nguoi_dung]) ON DELETE NO ACTION,
    CONSTRAINT [FK_DanhGia_Phim_ma_phim] FOREIGN KEY ([ma_phim]) REFERENCES [Phim] ([ma_phim]) ON DELETE NO ACTION
);

CREATE TABLE [HoaDon] (
    [ma_hoa_don] int NOT NULL IDENTITY,
    [ma_khuyen_mai] int NULL,
    [ma_nguoi_dung] int NOT NULL,
    [tong_tien] decimal(18,2) NOT NULL,
    [phuong_thuc_thanh_toan] nvarchar(50) NOT NULL,
    [ngay_giao_dich] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_HoaDon] PRIMARY KEY ([ma_hoa_don]),
    CONSTRAINT [FK_HoaDon_KhuyenMai_ma_khuyen_mai] FOREIGN KEY ([ma_khuyen_mai]) REFERENCES [KhuyenMai] ([ma_khuyen_mai]) ON DELETE SET NULL,
    CONSTRAINT [FK_HoaDon_NguoiDung_ma_nguoi_dung] FOREIGN KEY ([ma_nguoi_dung]) REFERENCES [NguoiDung] ([ma_nguoi_dung]) ON DELETE NO ACTION
);

CREATE TABLE [Ve] (
    [ma_ve] int NOT NULL IDENTITY,
    [ma_hoa_don] int NOT NULL,
    [ma_ghe] int NOT NULL,
    [ma_suat_chieu] int NOT NULL,
    [ma_qr] nvarchar(255) NULL,
    [trang_thai] nvarchar(50) NOT NULL,
    [gia_ve] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Ve] PRIMARY KEY ([ma_ve]),
    CONSTRAINT [FK_Ve_Ghe_ma_ghe] FOREIGN KEY ([ma_ghe]) REFERENCES [Ghe] ([ma_ghe]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Ve_HoaDon_ma_hoa_don] FOREIGN KEY ([ma_hoa_don]) REFERENCES [HoaDon] ([ma_hoa_don]) ON DELETE CASCADE,
    CONSTRAINT [FK_Ve_SuatChieu_ma_suat_chieu] FOREIGN KEY ([ma_suat_chieu]) REFERENCES [SuatChieu] ([ma_suat_chieu]) ON DELETE NO ACTION
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ma_vai_tro', N'ten_vai_tro') AND [object_id] = OBJECT_ID(N'[VaiTro]'))
    SET IDENTITY_INSERT [VaiTro] ON;
INSERT INTO [VaiTro] ([ma_vai_tro], [ten_vai_tro])
VALUES (1, N'Admin'),
(2, N'Staff'),
(3, N'Customer');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ma_vai_tro', N'ten_vai_tro') AND [object_id] = OBJECT_ID(N'[VaiTro]'))
    SET IDENTITY_INSERT [VaiTro] OFF;

CREATE INDEX [IX_DanhGia_ma_nguoi_dung] ON [DanhGia] ([ma_nguoi_dung]);

CREATE INDEX [IX_DanhGia_ma_phim] ON [DanhGia] ([ma_phim]);

CREATE INDEX [IX_Ghe_ma_phong] ON [Ghe] ([ma_phong]);

CREATE INDEX [IX_HoaDon_ma_khuyen_mai] ON [HoaDon] ([ma_khuyen_mai]);

CREATE INDEX [IX_HoaDon_ma_nguoi_dung] ON [HoaDon] ([ma_nguoi_dung]);

CREATE UNIQUE INDEX [IX_NguoiDung_email] ON [NguoiDung] ([email]);

CREATE INDEX [IX_NguoiDung_ma_vai_tro] ON [NguoiDung] ([ma_vai_tro]);

CREATE INDEX [IX_SuatChieu_ma_phim] ON [SuatChieu] ([ma_phim]);

CREATE INDEX [IX_SuatChieu_ma_phong] ON [SuatChieu] ([ma_phong]);

CREATE INDEX [IX_Ve_ma_ghe] ON [Ve] ([ma_ghe]);

CREATE INDEX [IX_Ve_ma_hoa_don] ON [Ve] ([ma_hoa_don]);

CREATE UNIQUE INDEX [IX_Ve_ma_suat_chieu_ma_ghe] ON [Ve] ([ma_suat_chieu], [ma_ghe]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260325115810_InitialCreate', N'9.0.3');

ALTER TABLE [Ve] ADD [ma_vao_cong] nvarchar(50) NULL;

ALTER TABLE [PhongChieu] ADD [LoaiPhong] nvarchar(max) NOT NULL DEFAULT N'';

ALTER TABLE [HoaDon] ADD [ma_vao_cong] nvarchar(50) NULL;

ALTER TABLE [HoaDon] ADD [trang_thai] nvarchar(50) NOT NULL DEFAULT N'ÄĂ£ thanh toĂ¡n';

CREATE TABLE [DoAnVat] (
    [ma_do_an_vat] int NOT NULL IDENTITY,
    [ten_mon] nvarchar(100) NOT NULL,
    [mo_ta] nvarchar(500) NULL,
    [gia] decimal(18,2) NOT NULL,
    [hinh_anh] nvarchar(500) NULL,
    [loai] nvarchar(50) NULL,
    CONSTRAINT [PK_DoAnVat] PRIMARY KEY ([ma_do_an_vat])
);

CREATE TABLE [HoaDonDoAnVat] (
    [ma_hoa_don_do_an_vat] int NOT NULL IDENTITY,
    [ma_hoa_don] int NOT NULL,
    [ma_do_an_vat] int NOT NULL,
    [so_luong] int NOT NULL,
    [don_gia] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_HoaDonDoAnVat] PRIMARY KEY ([ma_hoa_don_do_an_vat]),
    CONSTRAINT [FK_HoaDonDoAnVat_DoAnVat_ma_do_an_vat] FOREIGN KEY ([ma_do_an_vat]) REFERENCES [DoAnVat] ([ma_do_an_vat]) ON DELETE NO ACTION,
    CONSTRAINT [FK_HoaDonDoAnVat_HoaDon_ma_hoa_don] FOREIGN KEY ([ma_hoa_don]) REFERENCES [HoaDon] ([ma_hoa_don]) ON DELETE CASCADE
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ma_do_an_vat', N'gia', N'hinh_anh', N'loai', N'mo_ta', N'ten_mon') AND [object_id] = OBJECT_ID(N'[DoAnVat]'))
    SET IDENTITY_INSERT [DoAnVat] ON;
INSERT INTO [DoAnVat] ([ma_do_an_vat], [gia], [hinh_anh], [loai], [mo_ta], [ten_mon])
VALUES (1, 45000.0, N'/assets/concessions/bap_m.png', N'Báº¯p', N'Báº¯p rang vá»‹ caramel size vá»«a', N'Báº¯p rang ngá»t Caramel M'),
(2, 55000.0, N'/assets/concessions/bap_l.png', N'Báº¯p', N'Báº¯p rang vá»‹ caramel size lá»›n', N'Báº¯p rang ngá»t Caramel L'),
(3, 35000.0, N'/assets/concessions/coca_l.png', N'NÆ°á»›c', N'NÆ°á»›c ngá»t Coca Cola size lá»›n', N'Coca Cola L'),
(4, 35000.0, N'/assets/concessions/pepsi_l.png', N'NÆ°á»›c', N'NÆ°á»›c ngá»t Pepsi size lá»›n', N'Pepsi L'),
(5, 75000.0, N'/assets/concessions/combo_1.png', N'Combo', N'1 Báº¯p M + 1 NÆ°á»›c L', N'Combo ÄÆ¡n Alpha'),
(6, 115000.0, N'/assets/concessions/combo_2.png', N'Combo', N'1 Báº¯p L + 2 NÆ°á»›c L', N'Combo ÄĂ´i Alpha');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ma_do_an_vat', N'gia', N'hinh_anh', N'loai', N'mo_ta', N'ten_mon') AND [object_id] = OBJECT_ID(N'[DoAnVat]'))
    SET IDENTITY_INSERT [DoAnVat] OFF;

CREATE INDEX [IX_HoaDonDoAnVat_ma_do_an_vat] ON [HoaDonDoAnVat] ([ma_do_an_vat]);

CREATE INDEX [IX_HoaDonDoAnVat_ma_hoa_don] ON [HoaDonDoAnVat] ([ma_hoa_don]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260401100513_AddLoaiPhongToPhongChieu', N'9.0.3');

COMMIT;
GO


