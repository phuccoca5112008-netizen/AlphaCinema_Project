using AlphaCinema.Core.DTOs.Ve;
using AlphaCinema.Core.Interfaces;
using AlphaCinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
// Removed QRCoder to avoid server-side runtime issues.

namespace AlphaCinema.Services.Services;

public class DatVeService : IDatVeService
{
    private readonly AlphaCinemaDbContext _context;
    private readonly IKhuyenMaiService _khuyenMaiService;

    public DatVeService(AlphaCinemaDbContext context, IKhuyenMaiService khuyenMaiService)
    {
        _context = context;
        _khuyenMaiService = khuyenMaiService;
    }

    public async Task<DatVeResponse> DatVeAsync(int maNguoiDung, DatVeRequest request)
    {
        Console.WriteLine($"[DEBUG] DatVe Started: User={maNguoiDung}, SuatChieu={request.MaSuatChieu}");
        
        // Force add columns if not exists (handling manual DB changes without migrations)
        try {
            await _context.Database.ExecuteSqlRawAsync(
                "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HoaDon') AND name = 'ma_vao_cong') ALTER TABLE HoaDon ADD ma_vao_cong NVARCHAR(50); " +
                "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HoaDon') AND name = 'trang_thai') ALTER TABLE HoaDon ADD trang_thai NVARCHAR(50); " +
                "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Ve') AND name = 'ma_vao_cong') ALTER TABLE Ve ADD ma_vao_cong NVARCHAR(50); " +
                "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Ve') AND name = 'trang_thai') ALTER TABLE Ve ADD trang_thai NVARCHAR(50);" 
            );
        } catch { /* Silent if already exists or other error */ }

        // 1. Validate suất chiếu
        var suatChieu = await _context.SuatChieus
            .Include(s => s.Phim).Include(s => s.PhongChieu)
            .FirstOrDefaultAsync(s => s.MaSuatChieu == request.MaSuatChieu)
            ?? throw new Exception("Suất chiếu không tồn tại.");

        // 2. Validate ghế - kiểm tra ghế thuộc phòng chiếu và chưa bị đặt
        var ghes = await _context.Ghes
            .Where(g => request.MaGheIds.Contains(g.MaGhe) && g.MaPhong == suatChieu.MaPhong)
            .ToListAsync();

        if (ghes.Count != request.MaGheIds.Count)
            throw new Exception("Một hoặc nhiều ghế không hợp lệ.");

        var gheDaDat = await _context.Ves
            .Where(v => v.MaSuatChieu == request.MaSuatChieu
                     && request.MaGheIds.Contains(v.MaGhe)
                     && v.TrangThai != "Đã hủy")
            .AnyAsync();

        if (gheDaDat) throw new Exception("Một hoặc nhiều ghế đã được đặt.");

        // 3. Tính tổng tiền gốc
        var tongTienGoc = ghes.Sum(g =>
            g.LoaiGhe == "VIP" ? suatChieu.GiaVeGoc * 1.5m : suatChieu.GiaVeGoc);
        decimal tienGiam = 0;

        // 4. Áp dụng khuyến mãi nếu có
        if (!string.IsNullOrEmpty(request.MaCodeGiamGia))
        {
            var km = await _context.KhuyenMais
                .FirstOrDefaultAsync(k => k.MaCodeGiamGia == request.MaCodeGiamGia
                    && k.NgayBatDau <= DateTime.Now && k.NgayKetThuc >= DateTime.Now);

            if (km != null)
            {
                if (km.DonHangToiThieu.HasValue && tongTienGoc < km.DonHangToiThieu)
                    throw new Exception($"Đơn hàng tối thiểu {km.DonHangToiThieu:N0}đ để áp dụng mã này.");

                tienGiam = km.LoaiGiamGia == "PhanTram"
                    ? tongTienGoc * km.GiaTriGiam / 100
                    : km.GiaTriGiam;

                if (km.GiamToiDa.HasValue && tienGiam > km.GiamToiDa)
                    tienGiam = km.GiamToiDa.Value;
            }
        }

        var tongTienSauGiam = tongTienGoc - tienGiam;

        // 5. Tạo HoaDon        
        var km2 = string.IsNullOrEmpty(request.MaCodeGiamGia) ? null :
            await _context.KhuyenMais.FirstOrDefaultAsync(k => k.MaCodeGiamGia == request.MaCodeGiamGia);

        var hoaDon = new Core.Entities.HoaDon
        {
            MaNguoiDung = maNguoiDung,
            MaKhuyenMai = km2?.MaKhuyenMai,
            TongTien = tongTienSauGiam,
            PhuongThucThanhToan = request.PhuongThucThanhToan,
            NgayGiaoDich = DateTime.Now,
            MaVaoCong = $"BL-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}",
            TrangThai = "Đã thanh toán"
        };
        var billCode = hoaDon.MaVaoCong; // Save local copy
        _context.HoaDons.Add(hoaDon);
        await _context.SaveChangesAsync();

        // 6. Tạo vé + QR cho mỗi ghế
        var vesCreated = new List<Core.Entities.Ve>();
        foreach (var ghe in ghes)
        {
            var giaVe = ghe.LoaiGhe == "VIP" ? suatChieu.GiaVeGoc * 1.5m : suatChieu.GiaVeGoc;
            var maQR = GenerateQRCode($"ALPHA-{hoaDon.MaHoaDon}-{ghe.MaGhe}-{request.MaSuatChieu}");

            var ve = new Core.Entities.Ve
            {
                MaHoaDon = hoaDon.MaHoaDon,
                MaGhe = ghe.MaGhe,
                MaSuatChieu = request.MaSuatChieu,
                MaQR = maQR,
                MaVaoCong = $"AL-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}",
                TrangThai = "Đã thanh toán",
                GiaVe = giaVe
            };
            vesCreated.Add(ve);
        }
        _context.Ves.AddRange(vesCreated);

        // 7. Cộng điểm tích lũy (1 điểm per 10,000đ)
        var nguoiDung = await _context.NguoiDungs.FindAsync(maNguoiDung);
        if (nguoiDung != null)
            nguoiDung.DiemTichLuy += (int)(tongTienSauGiam / 10000);

        await _context.SaveChangesAsync();

        return new DatVeResponse
        {
            MaHoaDon = hoaDon.MaHoaDon,
            TongTien = tongTienSauGiam,
            TienGiam = tienGiam,
            PhuongThucThanhToan = request.PhuongThucThanhToan,
            NgayGiaoDich = hoaDon.NgayGiaoDich,
            MaVaoCong = billCode,
            TrangThai = hoaDon.TrangThai,
            Ves = vesCreated.Select(v => {
                var g = ghes.FirstOrDefault(x => x.MaGhe == v.MaGhe);
                return new VeResponse
                {
                    MaVe = v.MaVe,
                    MaQR = v.MaQR,
                    MaVaoCong = v.MaVaoCong,
                    TrangThai = v.TrangThai,
                    GiaVe = v.GiaVe,
                    TenPhim = suatChieu.Phim.TenPhim,
                    ThoiGianChieu = suatChieu.ThoiGianBatDau,
                    TenPhong = suatChieu.PhongChieu.TenPhong,
                    ViTriGhe = g != null ? $"{g.Hang}{g.SoGhe}" : "??",
                    LoaiGhe = g?.LoaiGhe ?? "Thường"
                };
            }).ToList()
        };
    }

    public async Task<IEnumerable<VeResponse>> GetVeByNguoiDungAsync(int maNguoiDung)
    {
        return await _context.Ves
            .Include(v => v.HoaDon).Include(v => v.Ghe)
            .Include(v => v.SuatChieu).ThenInclude(s => s.Phim)
            .Include(v => v.SuatChieu).ThenInclude(s => s.PhongChieu)
            .Where(v => v.HoaDon.MaNguoiDung == maNguoiDung)
            .Select(v => new VeResponse
            {
                MaVe = v.MaVe, MaQR = v.MaQR, MaVaoCong = v.MaVaoCong, TrangThai = v.TrangThai, GiaVe = v.GiaVe,
                TenPhim = v.SuatChieu.Phim.TenPhim,
                ThoiGianChieu = v.SuatChieu.ThoiGianBatDau,
                TenPhong = v.SuatChieu.PhongChieu.TenPhong,
                ViTriGhe = v.Ghe.Hang.ToString() + v.Ghe.SoGhe.ToString(),
                LoaiGhe = v.Ghe.LoaiGhe
            }).ToListAsync();
    }

    public async Task<VeResponse?> GetVeByIdAsync(int maVe)
    {
        return await _context.Ves
            .Include(v => v.HoaDon).Include(v => v.Ghe)
            .Include(v => v.SuatChieu).ThenInclude(s => s.Phim)
            .Include(v => v.SuatChieu).ThenInclude(s => s.PhongChieu)
            .Where(v => v.MaVe == maVe)
            .Select(v => new VeResponse
            {
                MaVe = v.MaVe, MaQR = v.MaQR, TrangThai = v.TrangThai, GiaVe = v.GiaVe,
                TenPhim = v.SuatChieu.Phim.TenPhim,
                ThoiGianChieu = v.SuatChieu.ThoiGianBatDau,
                TenPhong = v.SuatChieu.PhongChieu.TenPhong,
                ViTriGhe = v.Ghe.Hang.ToString() + v.Ghe.SoGhe.ToString(),
                LoaiGhe = v.Ghe.LoaiGhe
            }).FirstOrDefaultAsync();
    }

    public async Task HuyVeAsync(int maVe, int maNguoiDung)
    {
        var ve = await _context.Ves.Include(v => v.HoaDon)
            .FirstOrDefaultAsync(v => v.MaVe == maVe)
            ?? throw new Exception("Vé không tồn tại.");

        if (ve.HoaDon.MaNguoiDung != maNguoiDung)
            throw new Exception("Bạn không có quyền hủy vé này.");

        if (ve.TrangThai == "Đã hủy")
            throw new Exception("Vé đã được hủy trước đó.");

        ve.TrangThai = "Đã hủy";
        await _context.SaveChangesAsync();
    }

    public async Task<VeResponse> CheckInTicketAsync(string code)
    {
        // 1. Try Ticket Code
        var ve = await _context.Ves
            .Include(v => v.Ghe)
            .Include(v => v.SuatChieu).ThenInclude(s => s.Phim)
            .Include(v => v.SuatChieu).ThenInclude(s => s.PhongChieu)
            .FirstOrDefaultAsync(v => v.MaVaoCong == code || v.MaVe.ToString() == code);

        if (ve != null)
        {
            if (ve.TrangThai == "Đã sử dụng") throw new ArgumentException("Vé này đã được sử dụng trước đó.");
            if (ve.TrangThai == "Đã hủy") throw new ArgumentException("Vé này đã bị hủy.");

            ve.TrangThai = "Đã sử dụng";
            await _context.SaveChangesAsync();

            return new VeResponse
            {
                MaVe = ve.MaVe, MaQR = ve.MaQR, MaVaoCong = ve.MaVaoCong,
                TrangThai = ve.TrangThai, GiaVe = ve.GiaVe,
                TenPhim = ve.SuatChieu.Phim.TenPhim,
                ThoiGianChieu = ve.SuatChieu.ThoiGianBatDau,
                TenPhong = ve.SuatChieu.PhongChieu.TenPhong,
                ViTriGhe = $"{ve.Ghe.Hang}{ve.Ghe.SoGhe}",
                LoaiGhe = ve.Ghe.LoaiGhe
            };
        }

        // 2. Try Bill Code
        var hd = await _context.HoaDons
            .Include(h => h.Ves).ThenInclude(v => v.Ghe)
            .Include(h => h.Ves).ThenInclude(v => v.SuatChieu).ThenInclude(s => s.Phim)
            .Include(h => h.Ves).ThenInclude(v => v.SuatChieu).ThenInclude(s => s.PhongChieu)
            .FirstOrDefaultAsync(h => h.MaVaoCong == code || h.MaHoaDon.ToString() == code);

        if (hd != null)
        {
            if (hd.TrangThai == "Đã sử dụng") throw new ArgumentException("Hóa đơn này đã được sử dụng trước đó.");
            if (hd.TrangThai == "Đã hủy") throw new ArgumentException("Hóa đơn này đã bị hủy.");

            hd.TrangThai = "Đã sử dụng";
            foreach (var v in hd.Ves) v.TrangThai = "Đã sử dụng";
            await _context.SaveChangesAsync();

            var firstVe = hd.Ves.First();
            return new VeResponse
            {
                MaVe = firstVe.MaVe, MaQR = firstVe.MaQR, MaVaoCong = hd.MaVaoCong,
                TrangThai = hd.TrangThai, GiaVe = hd.TongTien, // Show total bill amount
                TenPhim = firstVe.SuatChieu.Phim.TenPhim,
                ThoiGianChieu = firstVe.SuatChieu.ThoiGianBatDau,
                TenPhong = firstVe.SuatChieu.PhongChieu.TenPhong,
                ViTriGhe = string.Join(", ", hd.Ves.Select(v => $"{v.Ghe.Hang}{v.Ghe.SoGhe}")),
                LoaiGhe = "HÓA ĐƠN"
            };
        }

        throw new ArgumentException("Mã vé/hóa đơn không hợp lệ hoặc không tồn tại.");
    }

    private string GenerateQRCode(string content)
    {
        // Don't use QRCoder server-side to avoid library-specific runtime errors.
        // Returning the content string instead, so frontend can generate QR image.
        return content;
    }
}
