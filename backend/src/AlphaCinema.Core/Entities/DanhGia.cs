namespace AlphaCinema.Core.Entities;

public class DanhGia
{
    public int MaDanhGia { get; set; }
    public int MaPhim { get; set; }
    public int MaNguoiDung { get; set; }
    public string? NoiDung { get; set; }
    public int DiemSo { get; set; }  // 1-5
    public DateTime NgayTao { get; set; } = DateTime.Now;

    // Navigation
    public Phim Phim { get; set; } = null!;
    public NguoiDung NguoiDung { get; set; } = null!;
}
