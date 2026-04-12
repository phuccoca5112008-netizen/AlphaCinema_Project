using AlphaCinema.Core.DTOs.HoaDon;
using AlphaCinema.Core.Entities;
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
            .Include(h => h.HoaDonDoAnVats).ThenInclude(ha => ha.DoAnVat)
            .Where(h => h.MaNguoiDung == maNguoiDung)
            .OrderByDescending(h => h.NgayGiaoDich)
            .ToListAsync();

        return list.Select(h => new HoaDonResponse
        {
            MaHoaDon = h.MaHoaDon, TongTien = h.TongTien,
            PhuongThucThanhToan = h.PhuongThucThanhToan,
            NgayGiaoDich = h.NgayGiaoDich,
            NgayLap = h.NgayGiaoDich,
            TenKhuyenMai = h.KhuyenMai?.TenKhuyenMai,
            MaCodeGiamGia = h.KhuyenMai?.MaCodeGiamGia,
            HoTenKhachHang = h.NguoiDung.HoTen,
            TenNguoiDung = h.NguoiDung.HoTen,
            Email = h.NguoiDung.Email,
            SoLuongVe = h.Ves.Count,
            MaVaoCong = h.MaVaoCong,
            TrangThai = h.TrangThai,
            TenPhim = h.Ves.FirstOrDefault()?.SuatChieu?.Phim?.TenPhim ?? "N/A",
            DanhSachGhe = string.Join(", ", h.Ves.Select(v => $"{v.Ghe.Hang}{v.Ghe.SoGhe}")),
            DanhSachDoAn = string.Join(", ", h.HoaDonDoAnVats.Select(ha => $"{ha.DoAnVat.TenMon} (x{ha.SoLuong})"))
        });
    }

    public async Task<IEnumerable<HoaDonResponse>> GetAllAsync()
    {
        var list = await _context.HoaDons
            .Include(h => h.KhuyenMai)
            .Include(h => h.NguoiDung)
            .Include(h => h.Ves).ThenInclude(v => v.Ghe)
            .Include(h => h.Ves).ThenInclude(v => v.SuatChieu).ThenInclude(s => s.Phim)
            .Include(h => h.HoaDonDoAnVats).ThenInclude(ha => ha.DoAnVat)
            .OrderByDescending(h => h.NgayGiaoDich)
            .ToListAsync();

        return list.Select(h => new HoaDonResponse
        {
            MaHoaDon = h.MaHoaDon, TongTien = h.TongTien,
            PhuongThucThanhToan = h.PhuongThucThanhToan,
            NgayGiaoDich = h.NgayGiaoDich,
            NgayLap = h.NgayGiaoDich,
            TenKhuyenMai = h.KhuyenMai?.TenKhuyenMai,
            MaCodeGiamGia = h.KhuyenMai?.MaCodeGiamGia,
            HoTenKhachHang = h.NguoiDung.HoTen,
            TenNguoiDung = h.NguoiDung.HoTen,
            Email = h.NguoiDung.Email,
            SoLuongVe = h.Ves.Count,
            MaVaoCong = h.MaVaoCong,
            TrangThai = h.TrangThai,
            TenPhim = h.Ves.FirstOrDefault()?.SuatChieu?.Phim?.TenPhim ?? "N/A",
            DanhSachGhe = string.Join(", ", h.Ves.Select(v => $"{v.Ghe.Hang}{v.Ghe.SoGhe}")),
            DanhSachDoAn = string.Join(", ", h.HoaDonDoAnVats.Select(ha => $"{ha.DoAnVat.TenMon} (x{ha.SoLuong})"))
        });
    }

    public async Task<HoaDonResponse?> GetByIdAsync(int id)
    {
        var h = await _context.HoaDons
            .Include(x => x.KhuyenMai).Include(x => x.NguoiDung)
            .Include(x => x.Ves).ThenInclude(v => v.Ghe)
            .Include(x => x.Ves).ThenInclude(v => v.SuatChieu).ThenInclude(s => s.Phim)
            .Include(h => h.HoaDonDoAnVats).ThenInclude(ha => ha.DoAnVat)
            .FirstOrDefaultAsync(x => x.MaHoaDon == id);
        if (h == null) return null;

        return new HoaDonResponse
        {
            MaHoaDon = h.MaHoaDon, TongTien = h.TongTien,
            PhuongThucThanhToan = h.PhuongThucThanhToan, NgayGiaoDich = h.NgayGiaoDich,
            NgayLap = h.NgayGiaoDich,
            TenKhuyenMai = h.KhuyenMai?.TenKhuyenMai, 
            MaCodeGiamGia = h.KhuyenMai?.MaCodeGiamGia,
            HoTenKhachHang = h.NguoiDung.HoTen,
            TenNguoiDung = h.NguoiDung.HoTen,
            Email = h.NguoiDung.Email,
            SoLuongVe = h.Ves.Count,
            MaVaoCong = h.MaVaoCong,
            TrangThai = h.TrangThai,
            TenPhim = h.Ves.FirstOrDefault()?.SuatChieu?.Phim?.TenPhim ?? "N/A",
            DanhSachGhe = string.Join(", ", h.Ves.Select(v => $"{v.Ghe.Hang}{v.Ghe.SoGhe}")),
            DanhSachDoAn = string.Join(", ", h.HoaDonDoAnVats.Select(ha => $"{ha.DoAnVat.TenMon} (x{ha.SoLuong})"))
        };
    }

    public async Task<DoanhThuResponse> GetDoanhThuAsync(DateTime? tuNgay = null, DateTime? denNgay = null)
    {
        var validStatuses = new[] { "Đã thanh toán", "Đã sử dụng" };
        var query = _context.HoaDons
            .Include(h => h.Ves).ThenInclude(v => v.SuatChieu).ThenInclude(s => s.Phim)
            .Include(h => h.HoaDonDoAnVats)
            .Where(h => validStatuses.Contains(h.TrangThai))
            .AsQueryable();

        if (tuNgay.HasValue) 
            query = query.Where(h => h.NgayGiaoDich >= tuNgay.Value.Date);
            
        if (denNgay.HasValue) 
        {
            var endOfDay = denNgay.Value.Date.AddDays(1).AddSeconds(-1);
            query = query.Where(h => h.NgayGiaoDich <= endOfDay);
        }

        var hoaDons = await query.ToListAsync();

        // 1. Tính doanh thu chi tiết theo từng phim (Chỉ tính vé KHÔNG bị hủy)
        var doanhThuTheoPhim = hoaDons
            .SelectMany(h => h.Ves ?? new List<Ve>())
            .Where(v => v.TrangThai != "Đã hủy" && v.SuatChieu?.Phim != null)
            .GroupBy(v => v.SuatChieu.Phim.TenPhim)
            .Select(g => new DoanhThuTheoPhim
            {
                TenPhim = g.Key,
                DoanhThu = g.Sum(v => v.GiaVe),
                SoVe = g.Count()
            }).ToList();

        // 2. Tính toán tổng doanh thu thực tế (Vé không hủy + Đồ ăn vặt)
        decimal tongTienVeThuc = hoaDons.SelectMany(h => h.Ves).Where(v => v.TrangThai != "Đã hủy").Sum(v => v.GiaVe);
        decimal tongTienDoAn = hoaDons.SelectMany(h => h.HoaDonDoAnVats).Sum(hd => hd.SoLuong * hd.DonGia);

        return new DoanhThuResponse
        {
            TongDoanhThu = tongTienVeThuc + tongTienDoAn,
            TongVeBan = hoaDons.SelectMany(h => h.Ves).Count(v => v.TrangThai != "Đã hủy"),
            TongHoaDon = hoaDons.Count,
            DoanhThuTheoPhims = doanhThuTheoPhim
        };
    }
}
