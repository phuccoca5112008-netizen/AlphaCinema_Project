namespace AlphaCinema.Core.DTOs.PhongChieu;

public class GheResponse
{
    public int MaGhe { get; set; }
    public char Hang { get; set; }
    public int SoGhe { get; set; }
    public string LoaiGhe { get; set; } = string.Empty;
}

public class PhongChieuResponse
{
    public int MaPhong { get; set; }
    public string TenPhong { get; set; } = string.Empty;
    public string LoaiPhong { get; set; } = "Standard";
    public List<GheResponse> Ghes { get; set; } = new();
}

public class CreatePhongChieuRequest
{
    public string TenPhong { get; set; } = string.Empty;
    public string LoaiPhong { get; set; } = "Standard";
}

public class GenerateGheRequest
{
    public int SoHang { get; set; } // Số hàng (từ A, B, C...)
    public int SoGheMotHang { get; set; } // Số lượng ghế trên 1 hàng (VD: 10 ghế)
}

public class UpdateGheRequest
{
    public string LoaiGhe { get; set; } = string.Empty; // "Thuong", "VIP"
}
