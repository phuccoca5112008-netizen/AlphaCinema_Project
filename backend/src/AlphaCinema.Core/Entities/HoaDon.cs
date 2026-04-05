namespace AlphaCinema.Core.Entities;

public class HoaDon
{
    public int MaHoaDon { get; set; }
    public int? MaKhuyenMai { get; set; }
    public int MaNguoiDung { get; set; }
    public decimal TongTien { get; set; }
    public string PhuongThucThanhToan { get; set; } = string.Empty;
    public DateTime NgayGiaoDich { get; set; } = DateTime.Now;
    public string? MaVaoCong { get; set; }
    public string TrangThai { get; set; } = "Đã thanh toán"; // "Đã thanh toán", "Đã sử dụng", "Đã hủy"

    // Navigation
    public KhuyenMai? KhuyenMai { get; set; }
    public NguoiDung NguoiDung { get; set; } = null!;
    public ICollection<Ve> Ves { get; set; } = new List<Ve>();
    public virtual ICollection<HoaDonDoAnVat> HoaDonDoAnVats { get; set; } = new List<HoaDonDoAnVat>();
}
