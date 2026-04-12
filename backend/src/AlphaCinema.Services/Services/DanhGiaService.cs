using AlphaCinema.Core.DTOs.DanhGia;
using AlphaCinema.Core.Interfaces;
using AlphaCinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AlphaCinema.Services.Services;

public class DanhGiaService : IDanhGiaService
{
    private readonly AlphaCinemaDbContext _context;

    public DanhGiaService(AlphaCinemaDbContext context) => _context = context;

    public async Task<IEnumerable<DanhGiaResponse>> GetByPhimAsync(int maPhim)
        => await _context.DanhGias.Include(d => d.NguoiDung).Include(d => d.Phim)
            .Where(d => d.MaPhim == maPhim)
            .OrderByDescending(d => d.NgayTao)
            .Select(d => new DanhGiaResponse
            {
                MaDanhGia = d.MaDanhGia, MaPhim = d.MaPhim,
                TenPhim = d.Phim.TenPhim,
                TenNguoiDung = d.NguoiDung.HoTen,
                BinhLuan = d.NoiDung, DiemSo = d.DiemSo, NgayDanhGia = d.NgayTao
            }).ToListAsync();

    public async Task<IEnumerable<DanhGiaResponse>> GetAllAsync()
        => await _context.DanhGias.Include(d => d.NguoiDung).Include(d => d.Phim)
            .OrderByDescending(d => d.NgayTao)
            .Select(d => new DanhGiaResponse
            {
                MaDanhGia = d.MaDanhGia, MaPhim = d.MaPhim,
                TenPhim = d.Phim.TenPhim,
                TenNguoiDung = d.NguoiDung.HoTen,
                BinhLuan = d.NoiDung, DiemSo = d.DiemSo, NgayDanhGia = d.NgayTao
            }).ToListAsync();
    public async Task<bool> IsUserEligibleToReview(int maNguoiDung, int maPhim)
    {
        // 1. Phải đã mua vé phim này (Trạng thái khác Đã hủy)
        var hasBought = await _context.Ves
            .Include(v => v.SuatChieu)
            .Include(v => v.HoaDon)
            .AnyAsync(v => v.HoaDon.MaNguoiDung == maNguoiDung 
                        && v.SuatChieu.MaPhim == maPhim 
                        && v.TrangThai != "Đã hủy");
        
        if (!hasBought) return false;

        // 2. Phải chưa bình luận phim này bao giờ (Mỗi người 1 lần)
        var alreadyReviewed = await _context.DanhGias
            .AnyAsync(d => d.MaNguoiDung == maNguoiDung && d.MaPhim == maPhim);

        return !alreadyReviewed;
    }
    public async Task<DanhGiaResponse> CreateAsync(int maNguoiDung, CreateDanhGiaRequest request)
    {
        if (request.DiemSo < 1 || request.DiemSo > 5)
            throw new Exception("Điểm số phải từ 1 đến 5.");

        if (!await _context.Phims.AnyAsync(p => p.MaPhim == request.MaPhim))
            throw new Exception("Phim không tồn tại.");

        if (!await IsUserEligibleToReview(maNguoiDung, request.MaPhim))
        {
            var alreadyReviewed = await _context.DanhGias.AnyAsync(d => d.MaNguoiDung == maNguoiDung && d.MaPhim == request.MaPhim);
            if (alreadyReviewed) throw new Exception("Bạn đã đánh giá phim này rồi.");
            throw new Exception("Bạn phải mua vé xem phim này mới được đánh giá.");
        }

        var dg = new Core.Entities.DanhGia
        {
            MaPhim = request.MaPhim, MaNguoiDung = maNguoiDung,
            NoiDung = request.NoiDung, DiemSo = request.DiemSo, NgayTao = DateTime.Now
        };
        _context.DanhGias.Add(dg);
        await _context.SaveChangesAsync();

        var nguoiDung = await _context.NguoiDungs.FindAsync(maNguoiDung);
        var phim = await _context.Phims.FindAsync(dg.MaPhim);
        return new DanhGiaResponse
        {
            MaDanhGia = dg.MaDanhGia, MaPhim = dg.MaPhim,
            TenPhim = phim!.TenPhim,
            TenNguoiDung = nguoiDung!.HoTen,
            BinhLuan = dg.NoiDung, DiemSo = dg.DiemSo, NgayDanhGia = dg.NgayTao
        };
    }

    public async Task DeleteAsync(int maDanhGia)
    {
        var dg = await _context.DanhGias.FindAsync(maDanhGia) ?? throw new Exception("Đánh giá không tồn tại.");
        _context.DanhGias.Remove(dg);
        await _context.SaveChangesAsync();
    }
}
