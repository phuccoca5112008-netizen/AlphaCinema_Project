namespace AlphaCinema.Core.Entities;

public class NguoiDung
{
    public int MaNguoiDung { get; set; }
    public int MaVaiTro { get; set; }
    public string Email { get; set; } = string.Empty;
    public string MatKhau { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public int DiemTichLuy { get; set; } = 0;

    // Navigation
    public VaiTro VaiTro { get; set; } = null!;
    public ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
    public ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
}
