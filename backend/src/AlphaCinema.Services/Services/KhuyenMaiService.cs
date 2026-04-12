using AlphaCinema.Core.DTOs.KhuyenMai;
using AlphaCinema.Core.Interfaces;
using AlphaCinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AlphaCinema.Services.Services;

public class KhuyenMaiService : IKhuyenMaiService
{
    private readonly AlphaCinemaDbContext _context;

    public KhuyenMaiService(AlphaCinemaDbContext context) => _context = context;

    private KhuyenMaiResponse ToResponse(Core.Entities.KhuyenMai k) => new()
    {
        MaKhuyenMai = k.MaKhuyenMai, TenKhuyenMai = k.TenKhuyenMai,
        MaCodeGiamGia = k.MaCodeGiamGia, MoTa = k.MoTa,
        NgayBatDau = k.NgayBatDau, NgayKetThuc = k.NgayKetThuc,
        LoaiGiamGia = k.LoaiGiamGia, GiaTriGiam = k.GiaTriGiam,
        GiamToiDa = k.GiamToiDa, DonHangToiThieu = k.DonHangToiThieu,
        HinhAnh = k.HinhAnh, PhanLoai = k.PhanLoai,
        ConHieuLuc = k.NgayBatDau <= DateTime.Now && k.NgayKetThuc >= DateTime.Now
    };

    public async Task<IEnumerable<KhuyenMaiResponse>> GetAllAsync()
        => (await _context.KhuyenMais.Where(k => k.MaNguoiDung == null).ToListAsync()).Select(ToResponse);

    public async Task<KhuyenMaiResponse?> GetByIdAsync(int id)
    {
        var k = await _context.KhuyenMais.FindAsync(id);
        return k == null ? null : ToResponse(k);
    }

    public async Task<ApDungKhuyenMaiResponse> ApDungMaGiamAsync(ApDungKhuyenMaiRequest request)
    {
        var km = await _context.KhuyenMais.FirstOrDefaultAsync(k =>
            k.MaCodeGiamGia == request.MaCode &&
            k.NgayBatDau <= DateTime.Now && k.NgayKetThuc >= DateTime.Now)
            ?? throw new Exception("Mã giảm giá không hợp lệ hoặc đã hết hạn.");

        if (km.DonHangToiThieu.HasValue && request.TongTienGoc < km.DonHangToiThieu)
            throw new Exception($"Đơn hàng tối thiểu {km.DonHangToiThieu:N0}đ để áp dụng mã này.");

        var tienGiam = km.LoaiGiamGia == "PhanTram"
            ? request.TongTienGoc * km.GiaTriGiam / 100
            : km.GiaTriGiam;

        decimal tongTienSauGiam = request.TongTienGoc - tienGiam;
        if (tongTienSauGiam < 0) tongTienSauGiam = 0;

        // Trả về tên quà tặng kèm (nếu có)
        string? tenDoAnVatTang = null;
        if (km.MaDoAnVatTang.HasValue)
        {
            var doAn = await _context.DoAnVats.FirstOrDefaultAsync(d => d.MaDoAnVat == km.MaDoAnVatTang.Value);
            tenDoAnVatTang = doAn?.TenMon;
        }

        return new ApDungKhuyenMaiResponse
        {
            TienGiam = tienGiam,
            TongTienSauGiam = tongTienSauGiam,
            TenKhuyenMai = km.TenKhuyenMai,
            MaDoAnVatTang = km.MaDoAnVatTang,
            TenDoAnVatTang = tenDoAnVatTang
        };
    }

    public async Task<KhuyenMaiResponse> CreateAsync(CreateKhuyenMaiRequest request)
    {
        if (await _context.KhuyenMais.AnyAsync(k => k.MaCodeGiamGia == request.MaCodeGiamGia))
            throw new Exception("Mã code đã tồn tại.");

        var km = new Core.Entities.KhuyenMai
        {
            TenKhuyenMai = request.TenKhuyenMai, MaCodeGiamGia = request.MaCodeGiamGia,
            MoTa = request.MoTa, NgayBatDau = request.NgayBatDau, NgayKetThuc = request.NgayKetThuc,
            LoaiGiamGia = request.LoaiGiamGia, GiaTriGiam = request.GiaTriGiam,
            GiamToiDa = request.GiamToiDa, DonHangToiThieu = request.DonHangToiThieu,
            HinhAnh = request.HinhAnh, PhanLoai = request.PhanLoai
        };
        _context.KhuyenMais.Add(km);
        await _context.SaveChangesAsync();
        return ToResponse(km);
    }

    public async Task<KhuyenMaiResponse> UpdateAsync(int id, CreateKhuyenMaiRequest request)
    {
        var km = await _context.KhuyenMais.FindAsync(id) ?? throw new Exception("Khuyến mãi không tồn tại.");
        km.TenKhuyenMai = request.TenKhuyenMai; km.MoTa = request.MoTa;
        km.NgayBatDau = request.NgayBatDau; km.NgayKetThuc = request.NgayKetThuc;
        km.LoaiGiamGia = request.LoaiGiamGia; km.GiaTriGiam = request.GiaTriGiam;
        km.GiamToiDa = request.GiamToiDa; km.DonHangToiThieu = request.DonHangToiThieu;
        km.HinhAnh = request.HinhAnh; km.PhanLoai = request.PhanLoai;
        await _context.SaveChangesAsync();
        return ToResponse(km);
    }

    public async Task DeleteAsync(int id)
    {
        var km = await _context.KhuyenMais.FindAsync(id) ?? throw new Exception("Khuyến mãi không tồn tại.");
        _context.KhuyenMais.Remove(km);
        await _context.SaveChangesAsync();
    }
}
