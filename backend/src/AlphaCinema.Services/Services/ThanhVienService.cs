using AlphaCinema.Core.DTOs.ThanhVien;
using AlphaCinema.Core.Interfaces;
using AlphaCinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AlphaCinema.Services.Services;

public class ThanhVienService : IThanhVienService
{
    private readonly AlphaCinemaDbContext _context;

    public ThanhVienService(AlphaCinemaDbContext context)
    {
        _context = context;
    }

    private readonly List<RewardResponse> _mockRewards = new()
    {
        new RewardResponse { MaPhanThuong = 1, TenPhanThuong = "Bắp rang ngọt Caramel", MoTa = "Quy đổi 1 phần bắp ngọt Caramel thơm lừng tại quầy.", DiemYeuCau = 150, LoaiPhanThuong = "Combo", HinhAnh = "/assets/rewards/bap_caramel.png" },
        new RewardResponse { MaPhanThuong = 2, TenPhanThuong = "Nước ngọt Cola (Size L)", MoTa = "Quy đổi 1 ly Pepsi/Coca Cola mát lạnh size lớn.", DiemYeuCau = 100, LoaiPhanThuong = "Combo", HinhAnh = "/assets/rewards/cola_l.png" },
        new RewardResponse { MaPhanThuong = 5, TenPhanThuong = "Combo Đơn (Bắp Caramel + Nước L)", MoTa = "Quy đổi 1 combo gồm bắp ngọt Caramel và 1 ly nước ngọt size lớn.", DiemYeuCau = 250, LoaiPhanThuong = "Combo", HinhAnh = "/assets/rewards/combo_don.png" },
        new RewardResponse { MaPhanThuong = 6, TenPhanThuong = "Combo Đôi (Bắp L + 2 Nước L)", MoTa = "Quy đổi 1 combo lớn cho cặp đôi.", DiemYeuCau = 350, LoaiPhanThuong = "Combo", HinhAnh = "/assets/rewards/combo_doi.png" },
        new RewardResponse { MaPhanThuong = 3, TenPhanThuong = "Voucher Giảm 50% Vé", MoTa = "Giảm 50% cho 1 vé xem phim bất kỳ (Áp dụng 2D/3D).", DiemYeuCau = 300, LoaiPhanThuong = "Voucher", HinhAnh = "/assets/rewards/voucher_50.png" },
        new RewardResponse { MaPhanThuong = 4, TenPhanThuong = "1 Vé Xem Phim Miễn Phí", MoTa = "Quy đổi 1 vé máy tính miễn phí cho phim bất kỳ.", DiemYeuCau = 600, LoaiPhanThuong = "Ticket", HinhAnh = "/assets/rewards/ve_mien_phi.png" },
        new RewardResponse { MaPhanThuong = 7, TenPhanThuong = "Nâng cấp ghế VIP", MoTa = "Nâng cấp từ ghế thường lên ghế VIP miễn phí.", DiemYeuCau = 80, LoaiPhanThuong = "Service", HinhAnh = "/assets/rewards/ghe_vip.png" },
        new RewardResponse { MaPhanThuong = 8, TenPhanThuong = "Bình nước Alpha Refreshment Tube", MoTa = "Quy đổi 1 bình nước Alpha Cinema phiên bản giới hạn.", DiemYeuCau = 1200, LoaiPhanThuong = "Merchandise", HinhAnh = "/assets/rewards/binh_nuoc.png" }
    };

    public Task<IEnumerable<RewardResponse>> GetAvailableRewardsAsync()
    {
        return Task.FromResult<IEnumerable<RewardResponse>>(_mockRewards);
    }

    public async Task<RedeemResponse> RedeemRewardAsync(int maNguoiDung, int maPhanThuong)
    {
        var user = await _context.NguoiDungs.FindAsync(maNguoiDung)
            ?? throw new Exception("Người dùng không tồn tại.");

        var reward = _mockRewards.FirstOrDefault(r => r.MaPhanThuong == maPhanThuong)
            ?? throw new Exception("Phần thưởng không tồn tại.");

        if (user.DiemTichLuy < reward.DiemYeuCau)
        {
            return new RedeemResponse { Success = false, Message = $"Bạn không đủ điểm. Cần {reward.DiemYeuCau} điểm." };
        }

        // Subtract points
        user.DiemTichLuy -= reward.DiemYeuCau;

        // Luôn tạo mã KhuyenMai để áp dụng vào hóa đơn
        var voucherCode = $"REDEEM-{Guid.NewGuid().ToString("N")[0..8].ToUpper()}";
        
        // Xác định giá trị giảm và món được tặng
        string loaiGiamGia = "SoTien";
        decimal giaTriGiam = 0; 
        int? maDoAnVatTang = null;

        if (reward.MaPhanThuong == 3) // Voucher 50%
        {
            loaiGiamGia = "PhanTram";
            giaTriGiam = 50;
        }
        else if (reward.MaPhanThuong == 4) giaTriGiam = 120000; // Vé miễn phí
        else if (reward.MaPhanThuong == 1) // Bắp Ngọt (M)
        {
            var gift = await _context.DoAnVats.FirstOrDefaultAsync(d => d.TenMon.Contains("Bắp") && d.TenMon.Contains("Ngọt"));
            maDoAnVatTang = gift?.MaDoAnVat;
            Console.WriteLine($"[REDEEM] Finding gift for Bắp Ngọt (M): Found ID {(maDoAnVatTang?.ToString() ?? "NULL")}");
        }
        else if (reward.MaPhanThuong == 2) // Nước L (Coca)
        {
            var gift = await _context.DoAnVats.FirstOrDefaultAsync(d => d.TenMon.Contains("Coca") || d.TenMon.Contains("Nước ngọt"));
            maDoAnVatTang = gift?.MaDoAnVat;
            Console.WriteLine($"[REDEEM] Finding gift for Coca L: Found ID {(maDoAnVatTang?.ToString() ?? "NULL")}");
        }
        else if (reward.MaPhanThuong == 5) // Combo Single
        {
            var gift = await _context.DoAnVats.FirstOrDefaultAsync(d => d.TenMon.Contains("Combo") && d.TenMon.Contains("Single"));
            maDoAnVatTang = gift?.MaDoAnVat;
            Console.WriteLine($"[REDEEM] Finding gift for Combo Single: Found ID {(maDoAnVatTang?.ToString() ?? "NULL")}");
        }
        else if (reward.MaPhanThuong == 6) // Combo Double
        {
            var gift = await _context.DoAnVats.FirstOrDefaultAsync(d => d.TenMon.Contains("Combo") && (d.TenMon.Contains("Double") || d.TenMon.Contains("Couple")));
            maDoAnVatTang = gift?.MaDoAnVat;
            Console.WriteLine($"[REDEEM] Finding gift for Combo Double: Found ID {(maDoAnVatTang?.ToString() ?? "NULL")}");
        }
        else if (reward.MaPhanThuong == 7) giaTriGiam = 30000;  // Upgrade
        else if (reward.MaPhanThuong == 8) giaTriGiam = 250000; // Bình nước

        var km = new Core.Entities.KhuyenMai
        {
            MaCodeGiamGia = voucherCode,
            TenKhuyenMai = $"Đổi thưởng: {reward.TenPhanThuong}",
            MoTa = $"Phần thưởng quy đổi từ điểm tích lũy: {reward.MoTa}",
            LoaiGiamGia = loaiGiamGia,
            GiaTriGiam = giaTriGiam,
            NgayBatDau = DateTime.Now.AddMinutes(-5),
            NgayKetThuc = DateTime.Now.AddDays(30),
            DonHangToiThieu = 0,
            MaNguoiDung = maNguoiDung,
            MaDoAnVatTang = maDoAnVatTang
        };
        _context.KhuyenMais.Add(km);
        await _context.SaveChangesAsync();
        
        // Trả về tên quà tặng để kiểm tra
        var finalGiftName = maDoAnVatTang.HasValue ? (await _context.DoAnVats.FindAsync(maDoAnVatTang.Value))?.TenMon : null;
        Console.WriteLine($"[REDEEM] Created Promo {voucherCode} with Gift: {finalGiftName ?? "NONE"}");

        return new RedeemResponse 
        { 
            Success = true, 
            Message = $"Quy đổi thành công {reward.TenPhanThuong} {(finalGiftName != null ? $"(Nhận ngay: {finalGiftName})" : "")}!", 
            GiftCode = voucherCode
        };
    }

    public async Task<IEnumerable<RewardHistoryResponse>> GetRewardHistoryAsync(int maNguoiDung)
    {
        return await _context.KhuyenMais
            .Where(k => k.MaNguoiDung == maNguoiDung)
            .OrderByDescending(k => k.NgayBatDau)
            .Select(k => new RewardHistoryResponse
            {
                MaKhuyenMai = k.MaKhuyenMai,
                TenKhuyenMai = k.TenKhuyenMai,
                MaCodeGiamGia = k.MaCodeGiamGia,
                MoTa = k.MoTa,
                NgayBatDau = k.NgayBatDau,
                NgayKetThuc = k.NgayKetThuc,
                LoaiGiamGia = k.LoaiGiamGia,
                GiaTriGiam = k.GiaTriGiam
            })
            .ToListAsync();
    }
}
