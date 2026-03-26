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

        var gheDaDat = await _context.Ves
            .Where(v => v.MaSuatChieu == maSuatChieu && v.TrangThai != "Đã hủy")
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

    public async Task<SuatChieuResponse> UpdateAsync(int id, UpdateSuatChieuRequest request)
    {
        var sc = await _context.SuatChieus.FindAsync(id) ?? throw new Exception("Suất chiếu không tồn tại.");
        if (request.ThoiGianBatDau.HasValue) sc.ThoiGianBatDau = request.ThoiGianBatDau.Value;
        if (request.DinhDang != null) sc.DinhDang = request.DinhDang;
        if (request.GiaVeGoc.HasValue) sc.GiaVeGoc = request.GiaVeGoc.Value;
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(sc.MaSuatChieu))!;
    }

    public async Task DeleteAsync(int id)
    {
        var sc = await _context.SuatChieus.FindAsync(id) ?? throw new Exception("Suất chiếu không tồn tại.");
        _context.SuatChieus.Remove(sc);
        await _context.SaveChangesAsync();
    }
}
