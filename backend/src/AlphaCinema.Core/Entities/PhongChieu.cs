namespace AlphaCinema.Core.Entities;

public class PhongChieu
{
    public int MaPhong { get; set; }
    public string TenPhong { get; set; } = string.Empty;
    public string LoaiPhong { get; set; } = "Standard"; // Standard, VIP, IMAX, 4DX

    // Navigation
    public ICollection<Ghe> Ghes { get; set; } = new List<Ghe>();
    public ICollection<SuatChieu> SuatChieus { get; set; } = new List<SuatChieu>();
}
