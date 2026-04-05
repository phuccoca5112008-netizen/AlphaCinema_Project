using AlphaCinema.Core.DTOs.Phim;
using AlphaCinema.Core.Interfaces;
using AlphaCinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AlphaCinema.Services.Services;
using System.Text.Json;
using AlphaCinema.Core.Entities;

namespace AlphaCinema.Services.Services;

public class PhimService : IPhimService
{
    private readonly AlphaCinemaDbContext _context;

    public PhimService(AlphaCinemaDbContext context) => _context = context;

    public async Task<IEnumerable<PhimResponse>> GetAllAsync(string? tuKhoa = null, string? trangThai = null, string? theLoai = null)
    {
        var query = _context.Phims
            .Include(p => p.DanhGias)
            .Include(p => p.SuatChieus).ThenInclude(s => s.Ves)
            .AsQueryable();

        if (!string.IsNullOrEmpty(tuKhoa))
            query = query.Where(p => p.TenPhim.Contains(tuKhoa) || p.TheLoai.Contains(tuKhoa));

        if (!string.IsNullOrEmpty(trangThai))
            query = query.Where(p => p.TrangThaiPhim == trangThai);

        if (!string.IsNullOrEmpty(theLoai))
            query = query.Where(p => p.TheLoai.Contains(theLoai));

        return await query.Select(p => new PhimResponse
        {
            MaPhim = p.MaPhim,
            TenPhim = p.TenPhim,
            TheLoai = p.TheLoai,
            ThoiLuong = p.ThoiLuong,
            Poster = p.Poster,
            TomTat = p.TomTat,
            TrangThaiPhim = p.TrangThaiPhim,
            DiemTrungBinh = p.DanhGias.Any() ? p.DanhGias.Average(d => d.DiemSo) : 0,
            SoLuongDanhGia = p.DanhGias.Count(),
            DoanhThu = p.SuatChieus.Sum(s => s.Ves.Where(v => v.TrangThai != "Đã hủy").Sum(v => v.GiaVe))
        }).ToListAsync();
    }

    public async Task<PhimResponse?> GetByIdAsync(int id)
    {
        var p = await _context.Phims
            .Include(x => x.DanhGias)
            .Include(x => x.SuatChieus).ThenInclude(s => s.Ves)
            .FirstOrDefaultAsync(x => x.MaPhim == id);
        if (p == null) return null;

        return new PhimResponse
        {
            MaPhim = p.MaPhim, TenPhim = p.TenPhim, TheLoai = p.TheLoai,
            ThoiLuong = p.ThoiLuong, Poster = p.Poster, TomTat = p.TomTat,
            TrangThaiPhim = p.TrangThaiPhim,
            DiemTrungBinh = p.DanhGias.Any() ? p.DanhGias.Average(d => d.DiemSo) : 0,
            SoLuongDanhGia = p.DanhGias.Count(),
            DoanhThu = p.SuatChieus.Sum(s => s.Ves.Where(v => v.TrangThai != "Đã hủy").Sum(v => v.GiaVe))
        };
    }

    public async Task<PhimResponse> CreateAsync(CreatePhimRequest request)
    {
        var phim = new Core.Entities.Phim
        {
            TenPhim = request.TenPhim, TheLoai = request.TheLoai,
            ThoiLuong = request.ThoiLuong, Poster = request.Poster,
            TomTat = request.TomTat, TrangThaiPhim = request.TrangThaiPhim
        };
        _context.Phims.Add(phim);
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(phim.MaPhim))!;
    }

    public async Task<PhimResponse> UpdateAsync(int id, UpdatePhimRequest request)
    {
        var phim = await _context.Phims.FindAsync(id) ?? throw new Exception("Phim không tồn tại.");
        if (request.TenPhim != null) phim.TenPhim = request.TenPhim;
        if (request.TheLoai != null) phim.TheLoai = request.TheLoai;
        if (request.ThoiLuong.HasValue) phim.ThoiLuong = request.ThoiLuong.Value;
        if (request.Poster != null) phim.Poster = request.Poster;
        if (request.TomTat != null) phim.TomTat = request.TomTat;
        if (request.TrangThaiPhim != null) phim.TrangThaiPhim = request.TrangThaiPhim;
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(phim.MaPhim))!;
    }

    public async Task DeleteAsync(int id)
    {
        var phim = await _context.Phims
            .Include(p => p.SuatChieus)
            .FirstOrDefaultAsync(p => p.MaPhim == id) 
            ?? throw new Exception("Phim không tồn tại.");

        if (phim.SuatChieus.Any())
        {
            throw new Exception("Không thể xóa phim này vì đã có các suất chiếu liên quan. Vui lòng xóa các suất chiếu trước.");
        }

        _context.Phims.Remove(phim);
        await _context.SaveChangesAsync();
    }

    public async Task SeedTmdbDataAsync()
    {
        // 1. Phim 
        if (!await _context.Phims.AnyAsync())
        {
            var fallbackMovies = new List<Phim>
            {
                new Phim { TenPhim = "Dune: Hành Tinh Cát - Phần 2", TheLoai = "Hành Động, Viễn Tưởng", ThoiLuong = 166, TrangThaiPhim = "Đang chiếu", Poster = "https://image.tmdb.org/t/p/w500/1pdfLvkbY9ohJlCjQH2TGpiKKTe.jpg", TomTat = "Paul Atreides tiếp tục hành trình trả thù..." },
                new Phim { TenPhim = "Godzilla x Kong: Đế Chế Mới", TheLoai = "Hành Động, Giả Tưởng", ThoiLuong = 115, TrangThaiPhim = "Đang chiếu", Poster = "https://image.tmdb.org/t/p/w500/1pdfLvkbY9ohJlCjQH2TGpiKKTe.jpg", TomTat = "Godzilla và Kong phải hợp sức chống lại một mối đe dọa khổng lồ ẩn sâu trong lõi Trái Đất..." },
                new Phim { TenPhim = "Kung Fu Panda 4", TheLoai = "Hoạt Hình, Hài, Gia Đình", ThoiLuong = 94, TrangThaiPhim = "Đang chiếu", Poster = "https://image.tmdb.org/t/p/w500/kDp1vUBnMpe8ak4rjgl3cLELqjU.jpg", TomTat = "Po chuẩn bị trở thành Thủ lĩnh tinh thần của Thung lũng Bình Yên và cần tìm một Chiến binh Thần Long mới..." },
                new Phim { TenPhim = "Mai", TheLoai = "Tình Cảm, Tâm Lý", ThoiLuong = 131, TrangThaiPhim = "Đang chiếu", Poster = "https://image.tmdb.org/t/p/w500/3r2A6g1G0u4O9KxI4eS9NThgE.jpg", TomTat = "Câu chuyện về Mai, một phụ nữ làm nghề mát-xa gặp phải nhiều định kiến xã hội..." },
                new Phim { TenPhim = "Lật Mặt 7: Một Điều Ước", TheLoai = "Gia Đình, Tâm Lý", ThoiLuong = 125, TrangThaiPhim = "Sắp chiếu", Poster = "https://image.tmdb.org/t/p/w500/z1p34Vh7dEOnIhC0AebqR8Jdb1n.jpg", TomTat = "Bà Hai 73 tuổi bị tai nạn, năm người con đứng trước sự lựa chọn ai sẽ về chăm sóc mẹ..." },
                new Phim { TenPhim = "Deadpool & Wolverine", TheLoai = "Hành Động, Hài", ThoiLuong = 127, TrangThaiPhim = "Sắp chiếu", Poster = "https://image.tmdb.org/t/p/w500/8cdWjvZQUExUUTzyp4t6EDMubfO.jpg", TomTat = "Tội phạm thời thời gian buộc Deadpool phải hợp tác với Wolverine để cứu rỗi vũ trụ của anh..." },
                new Phim { TenPhim = "Inside Out 2", TheLoai = "Hoạt Hình, Phiêu Lưu", ThoiLuong = 96, TrangThaiPhim = "Đang chiếu", Poster = "https://image.tmdb.org/t/p/w500/vpnVM9B6NMmQpWeZvzRx5xL0P79.jpg", TomTat = "Những cảm xúc mới lạ xuất hiện khi cô bé Riley bước vào tuổi dậy thì..." },
                new Phim { TenPhim = "Oppenheimer", TheLoai = "Lịch sử, Tâm lý", ThoiLuong = 180, TrangThaiPhim = "Đang chiếu", Poster = "https://image.tmdb.org/t/p/w500/8Gxv8gSFCU0XGDykEGv7zR1n2ua.jpg", TomTat = "Tiểu sử về cha đẻ bom nguyên tử..." }
            };

            // Có thể dùng HTTP Client để gọi TMDB ở đây nhưng dùng fallback array để đảm bảo tốc độ và độ ổn định trong demo.

            _context.Phims.AddRange(fallbackMovies);
            await _context.SaveChangesAsync();
        }

        // 2. Phòng và Ghế
        if (!await _context.PhongChieus.AnyAsync())
        {
            var r1 = new PhongChieu { TenPhong = "1 (IMAX)" };
            var r2 = new PhongChieu { TenPhong = "2 (VIP)" };
            var r3 = new PhongChieu { TenPhong = "3 (3D)" };
            _context.PhongChieus.AddRange(r1, r2, r3);
            await _context.SaveChangesAsync();

            // Tạo ghế cho phòng 1
            char[] hangs = { 'A', 'B', 'C', 'D' };
            int pcId = r1.MaPhong;
            foreach (var h in hangs)
            {
                for (int i = 1; i <= 8; i++)
                {
                    _context.Ghes.Add(new Ghe { MaPhong = pcId, Hang = h, SoGhe = i, LoaiGhe = i > 2 && i < 7 ? "VIP" : "Thường" });
                }
            }
            // Tạo ghế phòng 2, 3 ... (demo ngắn gọn)
            await _context.SaveChangesAsync();
        }

        // 3. Suất Chiếu mồi
        if (!await _context.SuatChieus.AnyAsync())
        {
            var qPhim = await _context.Phims.Where(p => p.TrangThaiPhim == "Đang chiếu").Take(3).ToListAsync();
            var phong = await _context.PhongChieus.FirstAsync();
            var tomorrow = DateTime.Now.Date.AddDays(1).AddHours(19);

            foreach (var p in qPhim)
            {
                _context.SuatChieus.Add(new SuatChieu
                {
                    MaPhim = p.MaPhim,
                    MaPhong = phong.MaPhong,
                    ThoiGianBatDau = tomorrow,
                    DinhDang = "2D",
                    GiaVeGoc = 90000
                });
                tomorrow = tomorrow.AddHours(3); // Cách nhau 3 tiếng
            }
            await _context.SaveChangesAsync();
        }
    }
}
