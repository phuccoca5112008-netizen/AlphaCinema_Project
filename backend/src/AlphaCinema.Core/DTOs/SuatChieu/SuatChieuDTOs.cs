namespace AlphaCinema.Core.DTOs.SuatChieu;

public class SuatChieuResponse
{
    public int MaSuatChieu { get; set; }
    public int MaPhim { get; set; }
    public string TenPhim { get; set; } = string.Empty;
    public int MaPhong { get; set; }
    public string TenPhong { get; set; } = string.Empty;
    public DateTime ThoiGianBatDau { get; set; }
    public string DinhDang { get; set; } = string.Empty;
    public decimal GiaVeGoc { get; set; }
    public int SoGheTrong { get; set; }
}

public class CreateSuatChieuRequest
{
    public int MaPhong { get; set; }
    public int MaPhim { get; set; }
    public DateTime ThoiGianBatDau { get; set; }
    public string DinhDang { get; set; } = string.Empty;
    public decimal GiaVeGoc { get; set; }
}

public class UpdateSuatChieuRequest
{
    public int? MaPhim { get; set; }
    public int? MaPhong { get; set; }
    public DateTime? ThoiGianBatDau { get; set; }
    public string? DinhDang { get; set; }
    public decimal? GiaVeGoc { get; set; }
}

public class GheResponse
{
    public int MaGhe { get; set; }
    public char Hang { get; set; }
    public int SoGhe { get; set; }
    public string LoaiGhe { get; set; } = string.Empty;
    public bool DaDat { get; set; }
}
