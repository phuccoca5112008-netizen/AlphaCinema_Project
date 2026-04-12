using AlphaCinema.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlphaCinema.Infrastructure.Data;

public class AlphaCinemaDbContext : DbContext
{
    public AlphaCinemaDbContext(DbContextOptions<AlphaCinemaDbContext> options) : base(options) { }

    public DbSet<VaiTro> VaiTros { get; set; }
    public DbSet<NguoiDung> NguoiDungs { get; set; }
    public DbSet<PhongChieu> PhongChieus { get; set; }
    public DbSet<Ghe> Ghes { get; set; }
    public DbSet<Phim> Phims { get; set; }
    public DbSet<SuatChieu> SuatChieus { get; set; }
    public DbSet<KhuyenMai> KhuyenMais { get; set; }
    public DbSet<HoaDon> HoaDons { get; set; }
    public DbSet<Ve> Ves { get; set; }
    public DbSet<DanhGia> DanhGias { get; set; }
    public DbSet<DoAnVat> DoAnVats { get; set; }
    public DbSet<HoaDonDoAnVat> HoaDonDoAnVats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // VaiTro
        modelBuilder.Entity<VaiTro>(e =>
        {
            e.ToTable("VaiTro");
            e.HasKey(x => x.MaVaiTro);
            e.Property(x => x.MaVaiTro).HasColumnName("ma_vai_tro").ValueGeneratedOnAdd();
            e.Property(x => x.TenVaiTro).HasColumnName("ten_vai_tro").HasMaxLength(50).IsRequired();
        });

        // NguoiDung
        modelBuilder.Entity<NguoiDung>(e =>
        {
            e.ToTable("NguoiDung");
            e.HasKey(x => x.MaNguoiDung);
            e.Property(x => x.MaNguoiDung).HasColumnName("ma_nguoi_dung").ValueGeneratedOnAdd();
            e.Property(x => x.MaVaiTro).HasColumnName("ma_vai_tro").IsRequired();
            e.Property(x => x.Email).HasColumnName("email").HasMaxLength(100).IsRequired();
            e.Property(x => x.MatKhau).HasColumnName("mat_khau").HasMaxLength(255).IsRequired();
            e.Property(x => x.HoTen).HasColumnName("ho_ten").HasMaxLength(100).IsUnicode(true);
            e.Property(x => x.DiemTichLuy).HasColumnName("diem_tich_luy").HasDefaultValue(0);
            e.HasIndex(x => x.Email).IsUnique();
            e.HasOne(x => x.VaiTro).WithMany(v => v.NguoiDungs)
                .HasForeignKey(x => x.MaVaiTro).OnDelete(DeleteBehavior.Restrict);
        });

        // PhongChieu
        modelBuilder.Entity<PhongChieu>(e =>
        {
            e.ToTable("PhongChieu");
            e.HasKey(x => x.MaPhong);
            e.Property(x => x.MaPhong).HasColumnName("ma_phong").ValueGeneratedOnAdd();
            e.Property(x => x.TenPhong).HasColumnName("ten_phong").HasMaxLength(100).IsUnicode(true).IsRequired();
            e.Property(x => x.LoaiPhong).HasColumnName("loai_phong").HasMaxLength(50).IsUnicode(true);
        });

        // Ghe
        modelBuilder.Entity<Ghe>(e =>
        {
            e.ToTable("Ghe");
            e.HasKey(x => x.MaGhe);
            e.Property(x => x.MaGhe).HasColumnName("ma_ghe").ValueGeneratedOnAdd();
            e.Property(x => x.MaPhong).HasColumnName("ma_phong").IsRequired();
            e.Property(x => x.Hang).HasColumnName("hang").HasColumnType("char(1)");
            e.Property(x => x.SoGhe).HasColumnName("so_ghe");
            e.Property(x => x.LoaiGhe).HasColumnName("loai_ghe").HasMaxLength(20);
            e.HasOne(x => x.PhongChieu).WithMany(p => p.Ghes)
                .HasForeignKey(x => x.MaPhong).OnDelete(DeleteBehavior.Restrict);
        });

        // Phim
        modelBuilder.Entity<Phim>(e =>
        {
            e.ToTable("Phim");
            e.HasKey(x => x.MaPhim);
            e.Property(x => x.MaPhim).HasColumnName("ma_phim").ValueGeneratedOnAdd();
            e.Property(x => x.TenPhim).HasColumnName("ten_phim").HasMaxLength(255).IsUnicode(true).IsRequired();
            e.Property(x => x.TheLoai).HasColumnName("the_loai").HasMaxLength(100).IsUnicode(true);
            e.Property(x => x.ThoiLuong).HasColumnName("thoi_luong");
            e.Property(x => x.Poster).HasColumnName("poster").HasMaxLength(255);
            e.Property(x => x.TomTat).HasColumnName("tom_tat").HasColumnType("nvarchar(max)");
            e.Property(x => x.TrangThaiPhim).HasColumnName("trang_thai_phim").HasMaxLength(50)
                .HasDefaultValue("Sắp chiếu");
        });

        // SuatChieu
        modelBuilder.Entity<SuatChieu>(e =>
        {
            e.ToTable("SuatChieu");
            e.HasKey(x => x.MaSuatChieu);
            e.Property(x => x.MaSuatChieu).HasColumnName("ma_suat_chieu").ValueGeneratedOnAdd();
            e.Property(x => x.MaPhong).HasColumnName("ma_phong").IsRequired();
            e.Property(x => x.MaPhim).HasColumnName("ma_phim").IsRequired();
            e.Property(x => x.ThoiGianBatDau).HasColumnName("thoi_gian_bat_dau");
            e.Property(x => x.DinhDang).HasColumnName("dinh_dang").HasMaxLength(10);
            e.Property(x => x.GiaVeGoc).HasColumnName("gia_ve_goc").HasColumnType("decimal(18,2)").HasDefaultValue(0);
            e.HasOne(x => x.PhongChieu).WithMany(p => p.SuatChieus).HasForeignKey(x => x.MaPhong).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Phim).WithMany(p => p.SuatChieus).HasForeignKey(x => x.MaPhim).OnDelete(DeleteBehavior.Restrict);
        });

        // KhuyenMai
        modelBuilder.Entity<KhuyenMai>(e =>
        {
            e.ToTable("KhuyenMai");
            e.HasKey(x => x.MaKhuyenMai);
            e.Property(x => x.MaKhuyenMai).HasColumnName("ma_khuyen_mai").ValueGeneratedOnAdd();
            e.Property(x => x.TenKhuyenMai).HasColumnName("ten_khuyen_mai").HasMaxLength(255).IsUnicode(true);
            e.Property(x => x.MaCodeGiamGia).HasColumnName("ma_code_giam_gia").HasMaxLength(50);
            e.Property(x => x.MoTa).HasColumnName("mo_ta").HasColumnType("nvarchar(max)");
            e.Property(x => x.NgayBatDau).HasColumnName("ngay_bat_dau").HasColumnType("date");
            e.Property(x => x.NgayKetThuc).HasColumnName("ngay_ket_thuc").HasColumnType("date");
            e.Property(x => x.LoaiGiamGia).HasColumnName("loai_giam_gia").HasMaxLength(50);
            e.Property(x => x.GiaTriGiam).HasColumnName("gia_tri_giam").HasColumnType("decimal(18,2)");
            e.Property(x => x.GiamToiDa).HasColumnName("giam_toi_da").HasColumnType("decimal(18,2)");
            e.Property(x => x.DonHangToiThieu).HasColumnName("don_hang_toi_thieu").HasColumnType("decimal(18,2)");
            e.Property(x => x.HinhAnh).HasColumnName("hinh_anh").HasMaxLength(255);
            e.Property(x => x.PhanLoai).HasColumnName("phan_loai").HasMaxLength(50);
            e.Property(x => x.MaDoAnVatTang).HasColumnName("ma_do_an_vat_tang");
            e.Property(x => x.MaNguoiDung).HasColumnName("ma_nguoi_dung");
            e.HasOne(x => x.NguoiDung).WithMany().HasForeignKey(x => x.MaNguoiDung).OnDelete(DeleteBehavior.Cascade);
        });

        // DanhGia
        modelBuilder.Entity<DanhGia>(e =>
        {
            e.ToTable("DanhGia");
            e.HasKey(x => x.MaDanhGia);
            e.Property(x => x.MaDanhGia).HasColumnName("ma_danh_gia").ValueGeneratedOnAdd();
            e.Property(x => x.MaPhim).HasColumnName("ma_phim").IsRequired();
            e.Property(x => x.MaNguoiDung).HasColumnName("ma_nguoi_dung").IsRequired();
            e.Property(x => x.NoiDung).HasColumnName("noi_dung").HasColumnType("nvarchar(max)");
            e.Property(x => x.DiemSo).HasColumnName("diem_so");
            e.Property(x => x.NgayTao).HasColumnName("ngay_tao").HasDefaultValueSql("GETDATE()");
            e.HasOne(x => x.Phim).WithMany(p => p.DanhGias).HasForeignKey(x => x.MaPhim).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.NguoiDung).WithMany(n => n.DanhGias).HasForeignKey(x => x.MaNguoiDung).OnDelete(DeleteBehavior.Restrict);
        });

        // HoaDon
        modelBuilder.Entity<HoaDon>(e =>
        {
            e.ToTable("HoaDon");
            e.HasKey(x => x.MaHoaDon);
            e.Property(x => x.MaHoaDon).HasColumnName("ma_hoa_don").ValueGeneratedOnAdd();
            e.Property(x => x.MaKhuyenMai).HasColumnName("ma_khuyen_mai");
            e.Property(x => x.MaNguoiDung).HasColumnName("ma_nguoi_dung").IsRequired();
            e.Property(x => x.TongTien).HasColumnName("tong_tien").HasColumnType("decimal(18,2)");
            e.Property(x => x.PhuongThucThanhToan).HasColumnName("phuong_thuc_thanh_toan").HasMaxLength(50);
            e.Property(x => x.NgayGiaoDich).HasColumnName("ngay_giao_dich").HasDefaultValueSql("GETDATE()");
            e.Property(x => x.MaVaoCong).HasColumnName("ma_vao_cong").HasMaxLength(50);
            e.Property(x => x.TrangThai).HasColumnName("trang_thai").HasMaxLength(50).HasDefaultValue("Đã thanh toán");
            e.HasOne(x => x.KhuyenMai).WithMany(k => k.HoaDons).HasForeignKey(x => x.MaKhuyenMai).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.NguoiDung).WithMany(n => n.HoaDons).HasForeignKey(x => x.MaNguoiDung).OnDelete(DeleteBehavior.Restrict);
        });

        // Ve
        modelBuilder.Entity<Ve>(e =>
        {
            e.ToTable("Ve");
            e.HasKey(x => x.MaVe);
            e.Property(x => x.MaVe).HasColumnName("ma_ve").ValueGeneratedOnAdd();
            e.Property(x => x.MaHoaDon).HasColumnName("ma_hoa_don").IsRequired();
            e.Property(x => x.MaGhe).HasColumnName("ma_ghe").IsRequired();
            e.Property(x => x.MaSuatChieu).HasColumnName("ma_suat_chieu").IsRequired();
            e.Property(x => x.MaQR).HasColumnName("ma_qr").HasMaxLength(255);
            e.Property(x => x.MaVaoCong).HasColumnName("ma_vao_cong").HasMaxLength(50);
            e.Property(x => x.TrangThai).HasColumnName("trang_thai").HasMaxLength(50);
            e.Property(x => x.GiaVe).HasColumnName("gia_ve").HasColumnType("decimal(18,2)");
            e.HasOne(x => x.HoaDon).WithMany(h => h.Ves).HasForeignKey(x => x.MaHoaDon).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Ghe).WithMany(g => g.Ves).HasForeignKey(x => x.MaGhe).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.SuatChieu).WithMany(s => s.Ves).HasForeignKey(x => x.MaSuatChieu).OnDelete(DeleteBehavior.Restrict);
            // Ràng buộc không đặt trùng ghế trong cùng suất chiếu
            e.HasIndex(x => new { x.MaSuatChieu, x.MaGhe })
                .IsUnique()
                .HasFilter("[trang_thai] <> N'Đã hủy'");
        });

        // Seed Data
        modelBuilder.Entity<VaiTro>().HasData(
            new VaiTro { MaVaiTro = 1, TenVaiTro = "Admin" },
            new VaiTro { MaVaiTro = 2, TenVaiTro = "Staff" },
            new VaiTro { MaVaiTro = 3, TenVaiTro = "Customer" }
        );

        // DoAnVat
        modelBuilder.Entity<DoAnVat>(e =>
        {
            e.ToTable("DoAnVat");
            e.HasKey(x => x.MaDoAnVat);
            e.Property(x => x.MaDoAnVat).HasColumnName("ma_do_an_vat").ValueGeneratedOnAdd();
            e.Property(x => x.TenMon).HasColumnName("ten_mon").HasMaxLength(100).IsUnicode(true).IsRequired();
            e.Property(x => x.MoTa).HasColumnName("mo_ta").HasMaxLength(500).IsUnicode(true);
            e.Property(x => x.Gia).HasColumnName("gia").HasColumnType("decimal(18,2)");
            e.Property(x => x.HinhAnh).HasColumnName("hinh_anh").HasMaxLength(500);
            e.Property(x => x.Loai).HasColumnName("loai").HasMaxLength(50).IsUnicode(true);
        });

        // HoaDonDoAnVat
        modelBuilder.Entity<HoaDonDoAnVat>(e =>
        {
            e.ToTable("HoaDonDoAnVat");
            e.HasKey(x => x.MaHoaDonDoAnVat);
            e.Property(x => x.MaHoaDonDoAnVat).HasColumnName("ma_hoa_don_do_an_vat").ValueGeneratedOnAdd();
            e.Property(x => x.MaHoaDon).HasColumnName("ma_hoa_don").IsRequired();
            e.Property(x => x.MaDoAnVat).HasColumnName("ma_do_an_vat").IsRequired();
            e.Property(x => x.SoLuong).HasColumnName("so_luong").IsRequired();
            e.Property(x => x.DonGia).HasColumnName("don_gia").HasColumnType("decimal(18,2)").IsRequired();
            e.HasOne(x => x.HoaDon).WithMany(h => h.HoaDonDoAnVats).HasForeignKey(x => x.MaHoaDon).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.DoAnVat).WithMany(d => d.HoaDonDoAnVats).HasForeignKey(x => x.MaDoAnVat).OnDelete(DeleteBehavior.Restrict);
        });

        // Seed DoAnVat
        modelBuilder.Entity<DoAnVat>().HasData(
            new DoAnVat { MaDoAnVat = 1, TenMon = "Bắp Rang Ngọt Caramel (M)", MoTa = "Bắp rang vị caramel thơm ngọt size vừa", Gia = 45000, Loai = "Bắp", HinhAnh = "/assets/concessions/bap_caramel_m.png" },
            new DoAnVat { MaDoAnVat = 2, TenMon = "Bắp Rang Ngọt Caramel (L)", MoTa = "Bắp rang vị caramel thơm ngọt size lớn", Gia = 55000, Loai = "Bắp", HinhAnh = "/assets/concessions/bap_caramel_m.png" },
            new DoAnVat { MaDoAnVat = 3, TenMon = "Bắp Rang Phô Mai (M)", MoTa = "Bắp rang vị phô mai mặn mà size vừa", Gia = 45000, Loai = "Bắp", HinhAnh = "/assets/concessions/bap_phomai_l.png" },
            new DoAnVat { MaDoAnVat = 4, TenMon = "Bắp Rang Phô Mai (L)", MoTa = "Bắp rang vị phô mai mặn mà size lớn", Gia = 55000, Loai = "Bắp", HinhAnh = "/assets/concessions/bap_phomai_l.png" },
            new DoAnVat { MaDoAnVat = 5, TenMon = "Coca Cola (L)", MoTa = "Nước ngọt Coca Cola mát lạnh size lớn", Gia = 35000, Loai = "Nước", HinhAnh = "/assets/concessions/coca_l.png" },
            new DoAnVat { MaDoAnVat = 6, TenMon = "Pepsi (L)", MoTa = "Nước ngọt Pepsi mát lạnh size lớn", Gia = 35000, Loai = "Nước", HinhAnh = "/assets/concessions/coca_l.png" },
            new DoAnVat { MaDoAnVat = 7, TenMon = "Nước Cam Tươi", MoTa = "Nước cam ép nguyên chất giàu vitamin", Gia = 45000, Loai = "Nước", HinhAnh = "/assets/concessions/cam_tuoi.png" },
            new DoAnVat { MaDoAnVat = 8, TenMon = "Milo Trà Xanh Đậu Đen", MoTa = "Thức uống sáng tạo kết hợp Milo và Trà xanh", Gia = 50000, Loai = "Nước", HinhAnh = "/assets/concessions/milo_trachanh.png" },
            new DoAnVat { MaDoAnVat = 9, TenMon = "Pizza Xúc Xích Mini", MoTa = "Pizza cỡ nhỏ phủ xúc xích và phô mai", Gia = 75000, Loai = "Thức ăn", HinhAnh = "/assets/concessions/pizza_mini.png" },
            new DoAnVat { MaDoAnVat = 10, TenMon = "Combo Single Alpha", MoTa = "1 Bắp (M) + 1 Nước (L) - Tiết kiệm hơn", Gia = 75000, Loai = "Combo", HinhAnh = "/assets/concessions/combo_single.png" },
            new DoAnVat { MaDoAnVat = 11, TenMon = "Combo Couple Alpha", MoTa = "1 Bắp (L) + 2 Nước (L) - Cho cặp đôi", Gia = 115000, Loai = "Combo", HinhAnh = "/assets/concessions/combo_couple.png" }
        );
    }
}
