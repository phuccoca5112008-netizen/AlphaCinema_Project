namespace AlphaCinema.Core.DTOs.HoaDon;

public class HoaDonResponse
{
    public int MaHoaDon { get; set; }
    public decimal TongTien { get; set; }
    public string PhuongThucThanhToan { get; set; } = string.Empty;
    public DateTime NgayGiaoDich { get; set; }
    public DateTime NgayLap { get; set; }
    public string? TenKhuyenMai { get; set; }
    public string? MaCodeGiamGia { get; set; }
    public string HoTenKhachHang { get; set; } = string.Empty;
    public string TenNguoiDung { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int SoLuongVe { get; set; }
    public string? MaVaoCong { get; set; }
    public string? TrangThai { get; set; }
    public string? TenPhim { get; set; }
    public string? DanhSachGhe { get; set; }
    public string? DanhSachDoAn { get; set; }
}

public class DoanhThuResponse
{
    public decimal TongDoanhThu { get; set; }
    public int TongVeBan { get; set; }
    public int TongHoaDon { get; set; }
    public List<DoanhThuTheoPhim> DoanhThuTheoPhims { get; set; } = new();
}

public class DoanhThuTheoPhim
{
    public string TenPhim { get; set; } = string.Empty;
    public decimal DoanhThu { get; set; }
    public int SoVe { get; set; }
}
