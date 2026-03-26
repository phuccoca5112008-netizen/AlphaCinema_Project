namespace AlphaCinema.Core.Entities;

public class VaiTro
{
    public int MaVaiTro { get; set; }
    public string TenVaiTro { get; set; } = string.Empty;

    // Navigation
    public ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();
}
