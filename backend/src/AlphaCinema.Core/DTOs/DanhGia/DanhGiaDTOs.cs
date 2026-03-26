namespace AlphaCinema.Core.DTOs.DanhGia;

public class DanhGiaResponse
{
    public int MaDanhGia { get; set; }
    public int MaPhim { get; set; }
    public string HoTenNguoiDung { get; set; } = string.Empty;
    public string? NoiDung { get; set; }
    public int DiemSo { get; set; }
    public DateTime NgayTao { get; set; }
}

public class CreateDanhGiaRequest
{
    public int MaPhim { get; set; }
    public string? NoiDung { get; set; }
    public int DiemSo { get; set; } // 1-5
}
