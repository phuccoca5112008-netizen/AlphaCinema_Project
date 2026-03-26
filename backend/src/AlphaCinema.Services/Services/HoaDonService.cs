using AlphaCinema.Core.DTOs.HoaDon;
using AlphaCinema.Core.Interfaces;
using AlphaCinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AlphaCinema.Services.Services;

public class HoaDonService : IHoaDonService
{
    private readonly AlphaCinemaDbContext _context;

    public HoaDonService(AlphaCinemaDbContext context) => _context = context;

    public async Task<IEnumerable<HoaDonResponse>> GetByNguoiDungAsync(int maNguoiDung)
    {
        var list = await _context.HoaDons
            .Include(h => h.KhuyenMai)
            .Include(h => h.NguoiDung)
            .Include(h => h.Ves).ThenInclude(v => v.Ghe)
            .Include(h => h.Ves).ThenInclude(v => v.SuatChieu).ThenInclude(s => s.Phim)
            .Where(h => h.MaNguoiDung == maNguoiDung)
            .OrderByDescending(h => h.NgayGiaoDich)
            .ToListAsync();

        return list.Select(h => new HoaDonResponse
        {
            MaHoaDon = h.MaHoaDon, TongTien = h.TongTien,
            PhuongThucThanhToan = h.PhuongThucThanhToan,
            NgayGiaoDich = h.NgayGiaoDich,
            TenKhuyenMai = h.KhuyenMai?.TenKhuyenMai,
            HoTenKhachHang = h.NguoiDung.HoTen,
            SoLuongVe = h.Ves.Count,
            MaVaoCong = h.MaVaoCong,
            TrangThai = h.TrangThai,
            TenPhim = h.Ves.FirstOrDefault()?.SuatChieu?.Phim?.TenPhim ?? "N/A",
            DanhSachGhe = string.Join(", ", h.Ves.Select(v => $"{v.Ghe.Hang}{v.Ghe.SoGhe}"))
        });
    }

    public async Task<IEnumerable<HoaDonResponse>> GetAllAsync()
    {
        var list = await _context.HoaDons
            .Include(h => h.KhuyenMai)
            .Include(h => h.NguoiDung)
            .Include(h => h.Ves).ThenInclude(v => v.Ghe)
            .Include(h => h.Ves).ThenInclude(v => v.SuatChieu).ThenInclude(s => s.Phim)
            .OrderByDescending(h => h.NgayGiaoDich)
            .ToListAsync();

        return list.Select(h => new HoaDonResponse
        {
            MaHoaDon = h.MaHoaDon, TongTien = h.TongTien,
            PhuongThucThanhToan = h.PhuongThucThanhToan,
            NgayGiaoDich = h.NgayGiaoDich,
            TenKhuyenMai = h.KhuyenMai?.TenKhuyenMai,
            HoTenKhachHang = h.NguoiDung.HoTen,
            SoLuongVe = h.Ves.Count,
            MaVaoCong = h.MaVaoCong,
            TrangThai = h.TrangThai,
            TenPhim = h.Ves.FirstOrDefault()?.SuatChieu?.Phim?.TenPhim ?? "N/A",
            DanhSachGhe = string.Join(", ", h.Ves.Select(v => $"{v.Ghe.Hang}{v.Ghe.SoGhe}"))
        });
    }

    public async Task<HoaDonResponse?> GetByIdAsync(int id)
    {
        var h = await _context.HoaDons
            .Include(x => x.KhuyenMai).Include(x => x.NguoiDung)
            .Include(x => x.Ves).ThenInclude(v => v.Ghe)
            .Include(x => x.Ves).ThenInclude(v => v.SuatChieu).ThenInclude(s => s.Phim)
            .FirstOrDefaultAsync(x => x.MaHoaDon == id);
        if (h == null) return null;

        return new HoaDonResponse
        {
            MaHoaDon = h.MaHoaDon, TongTien = h.TongTien,
            PhuongThucThanhToan = h.PhuongThucThanhToan, NgayGiaoDich = h.NgayGiaoDich,
            TenKhuyenMai = h.KhuyenMai?.TenKhuyenMai, HoTenKhachHang = h.NguoiDung.HoTen,
            SoLuongVe = h.Ves.Count,
            MaVaoCong = h.MaVaoCong,
            TrangThai = h.TrangThai,
            TenPhim = h.Ves.FirstOrDefault()?.SuatChieu?.Phim?.TenPhim ?? "N/A",
            DanhSachGhe = string.Join(", ", h.Ves.Select(v => $"{v.Ghe.Hang}{v.Ghe.SoGhe}"))
        };
    }

    public async Task<DoanhThuResponse> GetDoanhThuAsync(DateTime? tuNgay = null, DateTime? denNgay = null)
    {
        var query = _context.HoaDons.Include(h => h.Ves)
            .ThenInclude(v => v.SuatChieu).ThenInclude(s => s.Phim).AsQueryable();

        if (tuNgay.HasValue) query = query.Where(h => h.NgayGiaoDich >= tuNgay.Value);
        if (denNgay.HasValue) query = query.Where(h => h.NgayGiaoDich <= denNgay.Value);

        var hoaDons = await query.ToListAsync();

        var doanhThuTheoPhim = hoaDons
            .SelectMany(h => h.Ves)
            .GroupBy(v => v.SuatChieu.Phim.TenPhim)
            .Select(g => new DoanhThuTheoPhim
            {
                TenPhim = g.Key,
                DoanhThu = g.Sum(v => v.GiaVe),
                SoVe = g.Count()
            }).ToList();

        return new DoanhThuResponse
        {
            TongDoanhThu = hoaDons.Sum(h => h.TongTien),
            TongVeBan = hoaDons.Sum(h => h.Ves.Count),
            TongHoaDon = hoaDons.Count,
            DoanhThuTheoPhims = doanhThuTheoPhim
        };
    }
}
