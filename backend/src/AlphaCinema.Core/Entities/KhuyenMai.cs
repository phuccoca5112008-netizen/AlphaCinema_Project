namespace AlphaCinema.Core.Entities;

public class KhuyenMai
{
    public int MaKhuyenMai { get; set; }
    public string TenKhuyenMai { get; set; } = string.Empty;
    public string MaCodeGiamGia { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
    public string LoaiGiamGia { get; set; } = string.Empty; // "PhanTram", "CoDinh"
    public decimal GiaTriGiam { get; set; }          // % hoặc VNĐ cố định
    public decimal? GiamToiDa { get; set; }          // Mức giảm tối đa (VNĐ)
    public decimal? DonHangToiThieu { get; set; }    // Đơn hàng tối thiểu

    // Navigation
    public ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}
