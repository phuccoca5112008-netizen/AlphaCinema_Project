namespace AlphaCinema.Core.DTOs.NguoiDung;

public class NguoiDungResponse
{
    public int MaNguoiDung { get; set; }
    public string Email { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string VaiTro { get; set; } = string.Empty;
    public int DiemTichLuy { get; set; }
    public string HangThanhVien { get; set; } = "Bạc";
}

public class UpdateNguoiDungRequest
{
    public string? HoTen { get; set; }
    public string? MatKhauMoi { get; set; }
    public string? MatKhauCu { get; set; }
    /// <summary>Cập nhật vai trò người dùng (chỉ Admin)</summary>
    public string? VaiTro { get; set; } // "Admin", "Staff", "Customer"
}
