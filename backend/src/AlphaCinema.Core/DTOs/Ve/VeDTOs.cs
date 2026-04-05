namespace AlphaCinema.Core.DTOs.Ve;

public class DatVeRequest
{
    public int? MaHoaDon { get; set; } // Update an existing pending booking
    public int MaSuatChieu { get; set; }
    public List<int> MaGheIds { get; set; } = new();
    public string? MaCodeGiamGia { get; set; }
    public string PhuongThucThanhToan { get; set; } = string.Empty;
    public List<ConcessionRequest>? Concessions { get; set; } = new();
}

public class ConcessionRequest
{
    public int MaDoAnVat { get; set; }
    public int SoLuong { get; set; }
}

public class LockSeatsRequest
{
    public int MaSuatChieu { get; set; }
    public List<int> MaGheIds { get; set; } = new();
}

public class VeResponse
{
    public int MaVe { get; set; }
    public string? MaQR { get; set; }
    public string? MaVaoCong { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public decimal GiaVe { get; set; }
    public string TenPhim { get; set; } = string.Empty;
    public DateTime ThoiGianChieu { get; set; }
    public string TenPhong { get; set; } = string.Empty;
    public string ViTriGhe { get; set; } = string.Empty; // VD: "A5"
    public string LoaiGhe { get; set; } = string.Empty;
    public string? TenNguoiDung { get; set; }
    public string? DanhSachDoAn { get; set; }
    public bool DaCheckIn { get; set; } = false;
}

public class CheckInTicketRequest
{
    public string Code { get; set; } = string.Empty;
}

public class DatVeResponse
{
    public int MaHoaDon { get; set; }
    public decimal TongTien { get; set; }
    public decimal TienGiam { get; set; }
    public string PhuongThucThanhToan { get; set; } = string.Empty;
    public DateTime NgayGiaoDich { get; set; }
    public string? MaVaoCong { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public List<VeResponse> Ves { get; set; } = new();
}

public class ConcessionResponse
{
    public int MaDoAnVat { get; set; }
    public string TenMon { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public decimal Gia { get; set; }
    public string? HinhAnh { get; set; }
    public string? Loai { get; set; }
}
