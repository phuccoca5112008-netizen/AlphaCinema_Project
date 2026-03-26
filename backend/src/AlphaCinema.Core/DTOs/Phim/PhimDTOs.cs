namespace AlphaCinema.Core.DTOs.Phim;

public class PhimResponse
{
    public int MaPhim { get; set; }
    public string TenPhim { get; set; } = string.Empty;
    public string TheLoai { get; set; } = string.Empty;
    public int ThoiLuong { get; set; }
    public string? Poster { get; set; }
    public string? TomTat { get; set; }
    public string TrangThaiPhim { get; set; } = string.Empty;
    public double DiemTrungBinh { get; set; }
    public int SoLuongDanhGia { get; set; }
    public decimal DoanhThu { get; set; }
}

public class CreatePhimRequest
{
    public string TenPhim { get; set; } = string.Empty;
    public string TheLoai { get; set; } = string.Empty;
    public int ThoiLuong { get; set; }
    public string? Poster { get; set; }
    public string? TomTat { get; set; }
    public string TrangThaiPhim { get; set; } = "Sắp chiếu";
}

public class UpdatePhimRequest
{
    public string? TenPhim { get; set; }
    public string? TheLoai { get; set; }
    public int? ThoiLuong { get; set; }
    public string? Poster { get; set; }
    public string? TomTat { get; set; }
    public string? TrangThaiPhim { get; set; }
}
