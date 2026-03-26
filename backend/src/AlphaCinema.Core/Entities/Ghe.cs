namespace AlphaCinema.Core.Entities;

public class Ghe
{
    public int MaGhe { get; set; }
    public int MaPhong { get; set; }
    public char Hang { get; set; }
    public int SoGhe { get; set; }
    public string LoaiGhe { get; set; } = string.Empty; // "Thuong", "VIP"

    // Navigation
    public PhongChieu PhongChieu { get; set; } = null!;
    public ICollection<Ve> Ves { get; set; } = new List<Ve>();
}
