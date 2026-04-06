using AlphaCinema.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlphaCinema.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AlphaCinemaDbContext context)
    {
        // 1. Tạo Database nếu chưa có
        await context.Database.EnsureCreatedAsync();

        // 2. Kiểm tra và nạp Phòng Chiếu
        if (!await context.PhongChieus.AnyAsync())
        {
            var rooms = new List<PhongChieu>
            {
                new PhongChieu { TenPhong = "P1 - IMAX", LoaiPhong = "IMAX" },
                new PhongChieu { TenPhong = "P2 - PREMIUM", LoaiPhong = "VIP" },
                new PhongChieu { TenPhong = "P3 - STANDARD", LoaiPhong = "2D" }
            };
            await context.PhongChieus.AddRangeAsync(rooms);
            await context.SaveChangesAsync();
        }

        // 3. Kiểm tra và nạp Phim
        if (!await context.Phims.AnyAsync())
        {
            var movies = new List<Phim>
            {
                new Phim { 
                    TenPhim = "Dune: Hành Tinh Cát - Phần 2", 
                    TheLoai = "Hành Động, Viễn Tưởng", 
                    ThoiLuong = 166, 
                    Poster = "https://image.tmdb.org/t/p/w500/1pdfLvkbY9ohJlCjQH2TGpiKKTe.jpg",
                    TomTat = "Paul Atreides tiếp tục hành trình trả thù những kẻ đã hủy diệt gia đình mình...",
                    TrangThaiPhim = "Đang chiếu"
                },
                new Phim { 
                    TenPhim = "Deadpool & Wolverine", 
                    TheLoai = "Hành Động, Hài", 
                    ThoiLuong = 127, 
                    Poster = "https://image.tmdb.org/t/p/w500/8cdWjvZQUExUUTzyp4t6EDMubfO.jpg",
                    TomTat = "Tội phạm thời thời gian buộc Deadpool phải hợp tác với người bạn cũ Wolverine...",
                    TrangThaiPhim = "Đang chiếu"
                },
                new Phim { 
                    TenPhim = "Moana 2", 
                    TheLoai = "Hoạt Hình, Phiêu Lưu", 
                    ThoiLuong = 100, 
                    Poster = "https://image.tmdb.org/t/p/w500/m0S9799S9mq69p968T9Vv477Yov.jpg",
                    TomTat = "Sau hành trình đầu tiên, Moana tiếp tục lời gọi của tổ tiên xa xôi...",
                    TrangThaiPhim = "Đang chiếu"
                }
            };
            await context.Phims.AddRangeAsync(movies);
            await context.SaveChangesAsync();
        }

        // 4. Kiểm tra và nạp Ghế (Tự động tạo dựa trên phòng chiếu)
        if (!await context.Ghes.AnyAsync())
        {
            var p1 = await context.PhongChieus.FirstOrDefaultAsync(r => r.TenPhong == "P1 - IMAX");
            if (p1 != null)
            {
                var seats = new List<Ghe>();
                char[] rows = { 'A', 'B', 'C', 'D' };
                foreach (var row in rows)
                {
                    for (int i = 1; i <= 8; i++)
                    {
                        seats.Add(new Ghe { 
                            MaPhong = p1.MaPhong, 
                            Hang = row, 
                            SoGhe = i, 
                            LoaiGhe = (i >= 3 && i <= 6) ? "VIP" : "Thường" 
                        });
                    }
                }
                await context.Ghes.AddRangeAsync(seats);
                await context.SaveChangesAsync();
            }
        }

        // 5. Kiểm tra và nạp Vai Trò (Roles)
        if (!await context.VaiTros.AnyAsync())
        {
            var roles = new List<VaiTro>
            {
                new VaiTro { MaVaiTro = 1, TenVaiTro = "Admin" },
                new VaiTro { MaVaiTro = 2, TenVaiTro = "Staff" },
                new VaiTro { MaVaiTro = 3, TenVaiTro = "Customer" }
            };
            await context.VaiTros.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }

        // 6. Kiểm tra và nạp Người Dùng (Users)
        if (!await context.NguoiDungs.AnyAsync())
        {
            var users = new List<NguoiDung>
            {
                new NguoiDung { 
                    MaVaiTro = 1, Email = "admin@alpha.com", 
                    MatKhau = BCrypt.Net.BCrypt.HashPassword("admin123"), 
                    HoTen = "Quản Trị Viên Alpha", DiemTichLuy = 1000 
                },
                new NguoiDung { 
                    MaVaiTro = 3, Email = "customer@alpha.com", 
                    MatKhau = BCrypt.Net.BCrypt.HashPassword("password123"), 
                    HoTen = "Khách Hàng Mẫu", DiemTichLuy = 0 
                }
            };
            await context.NguoiDungs.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }

        // 7. Kiểm tra và nạp Khuyến Mãi
        if (!await context.KhuyenMais.AnyAsync())
        {
            var promos = new List<KhuyenMai>
            {
                new KhuyenMai { 
                    TenKhuyenMai = "Giảm giá Tri Ân", MaCodeGiamGia = "GIAMGIA1", 
                    MoTa = "Ưu đãi tri ân khách hàng thân thiết của Alpha Cinema.",
                    NgayBatDau = DateTime.Now.AddDays(-10), NgayKetThuc = DateTime.Now.AddDays(20),
                    LoaiGiamGia = "PhanTram", GiaTriGiam = 10 
                },
                new KhuyenMai { 
                    TenKhuyenMai = "Mừng Khai Trương", MaCodeGiamGia = "HELLO2026", 
                    MoTa = "Mừng năm mới 2026 rực rỡ.",
                    NgayBatDau = DateTime.Now, NgayKetThuc = DateTime.Now.AddDays(30),
                    LoaiGiamGia = "CoDinh", GiaTriGiam = 20000, DonHangToiThieu = 100000 
                }
            };
            await context.KhuyenMais.AddRangeAsync(promos);
            await context.SaveChangesAsync();
        }
    }
}
