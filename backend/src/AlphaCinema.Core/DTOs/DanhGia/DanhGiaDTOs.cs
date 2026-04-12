namespace AlphaCinema.Core.DTOs.DanhGia;

public class DanhGiaResponse
{
    public int MaDanhGia { get; set; }
    public int MaPhim { get; set; }
    public string TenPhim { get; set; } = string.Empty;
    public string TenNguoiDung { get; set; } = string.Empty;
    public string? BinhLuan { get; set; }
    public int DiemSo { get; set; }
    public DateTime NgayDanhGia { get; set; }
}

public class CreateDanhGiaRequest
{
    public int MaPhim { get; set; }
    public string? NoiDung { get; set; }
    public int DiemSo { get; set; } // 1-5
}
