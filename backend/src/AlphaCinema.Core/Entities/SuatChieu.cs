namespace AlphaCinema.Core.Entities;

public class SuatChieu
{
    public int MaSuatChieu { get; set; }
    public int MaPhong { get; set; }
    public int MaPhim { get; set; }
    public DateTime ThoiGianBatDau { get; set; }
    public string DinhDang { get; set; } = string.Empty; // "2D", "3D"
    public decimal GiaVeGoc { get; set; }

    // Navigation
    public PhongChieu PhongChieu { get; set; } = null!;
    public Phim Phim { get; set; } = null!;
    public ICollection<Ve> Ves { get; set; } = new List<Ve>();
}
