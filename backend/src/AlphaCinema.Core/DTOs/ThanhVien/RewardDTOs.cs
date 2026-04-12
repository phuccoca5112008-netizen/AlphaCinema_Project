namespace AlphaCinema.Core.DTOs.ThanhVien;

public class RewardResponse
{
    public int MaPhanThuong { get; set; }
    public string TenPhanThuong { get; set; } = string.Empty;
    public string MoTa { get; set; } = string.Empty;
    public int DiemYeuCau { get; set; }
    public string LoaiPhanThuong { get; set; } = string.Empty; // "Voucher", "Combo", "Ticket"
    public string? HinhAnh { get; set; }
}

public class RedeemRequest
{
    public int MaPhanThuong { get; set; }
}

public class RedeemResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? GiftCode { get; set; }
}

public class RewardHistoryResponse
{
    public int MaKhuyenMai { get; set; }
    public string TenKhuyenMai { get; set; } = string.Empty;
    public string MaCodeGiamGia { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
    public string LoaiGiamGia { get; set; } = string.Empty;
    public decimal GiaTriGiam { get; set; }
}
