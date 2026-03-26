namespace AlphaCinema.Core.Entities;

public class Ve
{
    public int MaVe { get; set; }
    public int MaHoaDon { get; set; }
    public int MaGhe { get; set; }
    public int MaSuatChieu { get; set; }
    public string? MaQR { get; set; }
    public string? MaVaoCong { get; set; }
    public string TrangThai { get; set; } = string.Empty; // "Đã thanh toán", "Đã sử dụng", "Đã hủy"
    public decimal GiaVe { get; set; }

    // Navigation
    public HoaDon HoaDon { get; set; } = null!;
    public Ghe Ghe { get; set; } = null!;
    public SuatChieu SuatChieu { get; set; } = null!;
}
