namespace AlphaCinema.Core.Entities;

public class Phim
{
    public int MaPhim { get; set; }
    public string TenPhim { get; set; } = string.Empty;
    public string TheLoai { get; set; } = string.Empty;
    public int ThoiLuong { get; set; }      // phút
    public string? Poster { get; set; }
    public string? TomTat { get; set; }
    public string TrangThaiPhim { get; set; } = "Sắp chiếu"; // "Đang chiếu", "Sắp chiếu", "Ngừng chiếu"

    // Navigation
    public ICollection<SuatChieu> SuatChieus { get; set; } = new List<SuatChieu>();
    public ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
}
