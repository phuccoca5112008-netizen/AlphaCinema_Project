namespace AlphaCinema.Core.DTOs.KhuyenMai;

public class KhuyenMaiResponse
{
    public int MaKhuyenMai { get; set; }
    public string TenKhuyenMai { get; set; } = string.Empty;
    public string MaCodeGiamGia { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
    public string LoaiGiamGia { get; set; } = string.Empty;
    public decimal GiaTriGiam { get; set; }
    public decimal? GiamToiDa { get; set; }
    public decimal? DonHangToiThieu { get; set; }
    public bool ConHieuLuc { get; set; }
    public string? HinhAnh { get; set; }
    public string? PhanLoai { get; set; }
}

public class CreateKhuyenMaiRequest
{
    public string TenKhuyenMai { get; set; } = string.Empty;
    public string MaCodeGiamGia { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
    public string LoaiGiamGia { get; set; } = string.Empty; // "PhanTram", "CoDinh"
    public decimal GiaTriGiam { get; set; }
    public decimal? GiamToiDa { get; set; }
    public decimal? DonHangToiThieu { get; set; }
    public string? HinhAnh { get; set; }
    public string? PhanLoai { get; set; }
}

public class ApDungKhuyenMaiRequest
{
    public string MaCode { get; set; } = string.Empty;
    public decimal TongTienGoc { get; set; }
}

public class ApDungKhuyenMaiResponse
{
    public decimal TienGiam { get; set; }
    public decimal TongTienSauGiam { get; set; }
    public string TenKhuyenMai { get; set; } = string.Empty;
    public int? MaDoAnVatTang { get; set; }
    public string? TenDoAnVatTang { get; set; }
}
