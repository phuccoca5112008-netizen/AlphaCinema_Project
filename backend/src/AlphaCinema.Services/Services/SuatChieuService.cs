using AlphaCinema.Core.DTOs.SuatChieu;
using AlphaCinema.Core.Interfaces;
using AlphaCinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AlphaCinema.Services.Services;

public class SuatChieuService : ISuatChieuService
{
    private readonly AlphaCinemaDbContext _context;

    public SuatChieuService(AlphaCinemaDbContext context) => _context = context;

    public async Task<IEnumerable<SuatChieuResponse>> GetAllAsync(int? maPhim = null, DateTime? ngay = null)
    {
        var query = _context.SuatChieus
            .Include(s => s.Phim).Include(s => s.PhongChieu).Include(s => s.Ves)
            .AsQueryable();

        if (maPhim.HasValue) query = query.Where(s => s.MaPhim == maPhim.Value);
        if (ngay.HasValue) query = query.Where(s => s.ThoiGianBatDau.Date == ngay.Value.Date);

        var totalGhes = await _context.Ghes.GroupBy(g => g.MaPhong)
            .ToDictionaryAsync(g => g.Key, g => g.Count());

        return await query.Select(s => new SuatChieuResponse
        {
            MaSuatChieu = s.MaSuatChieu,
            MaPhim = s.MaPhim,
            TenPhim = s.Phim.TenPhim,
            MaPhong = s.MaPhong,
            TenPhong = s.PhongChieu.TenPhong,
            ThoiGianBatDau = s.ThoiGianBatDau,
            DinhDang = s.DinhDang,
            GiaVeGoc = s.GiaVeGoc,
            SoGheTrong = _context.Ghes.Count(g => g.MaPhong == s.MaPhong) -
                         s.Ves.Count(v => v.TrangThai != "Đã hủy")
        }).ToListAsync();
    }

    public async Task<SuatChieuResponse?> GetByIdAsync(int id)
    {
        var s = await _context.SuatChieus
            .Include(x => x.Phim).Include(x => x.PhongChieu).Include(x => x.Ves)
            .FirstOrDefaultAsync(x => x.MaSuatChieu == id);
        if (s == null) return null;

        var totalGhe = await _context.Ghes.CountAsync(g => g.MaPhong == s.MaPhong);
        return new SuatChieuResponse
        {
            MaSuatChieu = s.MaSuatChieu, MaPhim = s.MaPhim, TenPhim = s.Phim.TenPhim,
            MaPhong = s.MaPhong, TenPhong = s.PhongChieu.TenPhong,
            ThoiGianBatDau = s.ThoiGianBatDau, DinhDang = s.DinhDang, GiaVeGoc = s.GiaVeGoc,
            SoGheTrong = totalGhe - s.Ves.Count(v => v.TrangThai != "Đã hủy")
        };
    }

    public async Task<IEnumerable<GheResponse>> GetGheBySuatChieuAsync(int maSuatChieu)
    {
        var suatChieu = await _context.SuatChieus.FindAsync(maSuatChieu)
            ?? throw new Exception("Suất chiếu không tồn tại.");

        var ghes = await _context.Ghes
            .Where(g => g.MaPhong == suatChieu.MaPhong)
            .ToListAsync();

        var limitTime = DateTime.Now.AddMinutes(-10);
        var gheDaDat = await _context.Ves
            .Include(v => v.HoaDon)
            .Where(v => v.MaSuatChieu == maSuatChieu 
                     && v.TrangThai != "Đã hủy"
                     && (v.TrangThai != "Đang chờ" || v.HoaDon.NgayGiaoDich > limitTime))
            .Select(v => v.MaGhe)
            .ToListAsync();

        return ghes.Select(g => new GheResponse
        {
            MaGhe = g.MaGhe,
            Hang = g.Hang,
            SoGhe = g.SoGhe,
            LoaiGhe = g.LoaiGhe,
            DaDat = gheDaDat.Contains(g.MaGhe)
        });
    }

    public async Task<SuatChieuResponse> CreateAsync(CreateSuatChieuRequest request)
    {
        // 1. Kiểm tra thời gian không được trong quá khứ
        if (request.ThoiGianBatDau < DateTime.Now)
        {
            throw new Exception("Thời gian bắt đầu không thể ở trong quá khứ.");
        }

        // 2. Lấy thông tin phim để biết thời lượng thực tế
        var movie = await _context.Phims.FindAsync(request.MaPhim) 
                    ?? throw new Exception("Không tìm thấy phim để tạo suất chiếu.");
        
        var bufferTime = 15; // 15 phút dọn phòng
        var movieDuration = movie.ThoiLuong + bufferTime;
        var startTime = request.ThoiGianBatDau;
        var endTime = startTime.AddMinutes(movieDuration);

        // 3. Chống chồng chéo lịch (Thông minh hơn: Lấy tất cả suất chiếu cùng phòng trong cùng ngày đó)
        var dateDate = request.ThoiGianBatDau.Date;
        var existingShows = await _context.SuatChieus
            .Include(s => s.Phim)
            .Where(s => s.MaPhong == request.MaPhong && s.ThoiGianBatDau.Date == dateDate)
            .ToListAsync();

        foreach (var s in existingShows)
        {
            var sStart = s.ThoiGianBatDau;
            var sEnd = sStart.AddMinutes(s.Phim.ThoiLuong + 15);

            // Kiểm tra giao thoa khoảng thời gian: [startTime, endTime] giao với [sStart, sEnd]
            if ((startTime >= sStart && startTime < sEnd) || 
                (endTime > sStart && endTime <= sEnd) ||
                (startTime <= sStart && endTime >= sEnd))
            {
                throw new Exception($"Trùng lịch chiếu! Phòng này đang có phim '{s.Phim.TenPhim}' chiếu từ {sStart:HH:mm} đến {sEnd:HH:mm} (Bao gồm dọn phòng).");
            }
        }

        try 
        {
            var sc = new Core.Entities.SuatChieu
            {
                MaPhong = request.MaPhong, MaPhim = request.MaPhim,
                ThoiGianBatDau = request.ThoiGianBatDau,
                DinhDang = request.DinhDang, GiaVeGoc = request.GiaVeGoc
            };
            _context.SuatChieus.Add(sc);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(sc.MaSuatChieu))!;
        }
        catch (Exception ex)
        {
            throw new Exception("Lỗi cơ sở dữ liệu khi tạo suất chiếu: " + ex.InnerException?.Message ?? ex.Message);
        }
    }

    public async Task<SuatChieuResponse> UpdateAsync(int id, UpdateSuatChieuRequest request)
    {
        var sc = await _context.SuatChieus.FindAsync(id) 
                 ?? throw new Exception("Suất chiếu không tồn tại.");

        // 1. Kiểm tra thời gian
        if (request.ThoiGianBatDau < DateTime.Now)
            throw new Exception("Thời gian bắt đầu không thể ở trong quá khứ.");

        // 2. Lấy thông tin phim để biết thời lượng thực tế
        var movie = await _context.Phims.FindAsync(request.MaPhim) 
                    ?? throw new Exception("Không tìm thấy phim.");
        
        var bufferTime = 15;
        var movieDuration = movie.ThoiLuong + bufferTime;
        var startTime = request.ThoiGianBatDau ?? sc.ThoiGianBatDau;
        var endTime = startTime.AddMinutes(movieDuration);

        // 3. Kiểm tra chồng chéo (Loại trừ chính nó)
        var dateDate = startTime.Date;
        var existingShows = await _context.SuatChieus
            .Include(s => s.Phim)
            .Where(s => s.MaPhong == (request.MaPhong ?? sc.MaPhong) && s.ThoiGianBatDau.Date == dateDate && s.MaSuatChieu != id)
            .ToListAsync();

        foreach (var s in existingShows)
        {
            var sStart = s.ThoiGianBatDau;
            var sEnd = sStart.AddMinutes(s.Phim.ThoiLuong + 15);

            if ((startTime >= sStart && startTime < sEnd) || 
                (endTime > sStart && endTime <= sEnd) ||
                (startTime <= sStart && endTime >= sEnd))
            {
                throw new Exception($"Trùng lịch! Phòng này đang bận từ {sStart:HH:mm} đến {sEnd:HH:mm} (Phim {s.Phim.TenPhim}).");
            }
        }

        try
        {
            sc.MaPhong = request.MaPhong ?? sc.MaPhong;
            sc.MaPhim = request.MaPhim ?? sc.MaPhim;
            sc.ThoiGianBatDau = request.ThoiGianBatDau ?? sc.ThoiGianBatDau;
            sc.DinhDang = request.DinhDang ?? sc.DinhDang;
            sc.GiaVeGoc = (decimal)(request.GiaVeGoc ?? sc.GiaVeGoc);

            await _context.SaveChangesAsync();
            return (await GetByIdAsync(id))!;
        }
        catch (Exception ex)
        {
            throw new Exception("Lỗi cập nhật suất chiếu: " + ex.InnerException?.Message ?? ex.Message);
        }
    }

    public async Task DeleteAsync(int id)
    {
        var sc = await _context.SuatChieus
            .Include(s => s.Ves)
            .FirstOrDefaultAsync(s => s.MaSuatChieu == id) 
            ?? throw new Exception("Suất chiếu không tồn tại.");

        // Kiểm tra xem đã có vé nào được đặt (không phải Đã hủy) chưa
        var hasBookedTickets = sc.Ves.Any(v => v.TrangThai != "Đã hủy");
        if (hasBookedTickets)
        {
            throw new Exception("Không thể xóa suất chiếu này vì đã có vé được đặt. Vui lòng hủy các vé liên quan trước.");
        }

        _context.SuatChieus.Remove(sc);
        await _context.SaveChangesAsync();
    }
}
