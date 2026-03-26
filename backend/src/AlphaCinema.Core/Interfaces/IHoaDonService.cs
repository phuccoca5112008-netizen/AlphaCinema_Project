using AlphaCinema.Core.DTOs.HoaDon;

namespace AlphaCinema.Core.Interfaces;

public interface IHoaDonService
{
    Task<IEnumerable<HoaDonResponse>> GetByNguoiDungAsync(int maNguoiDung);
    Task<IEnumerable<HoaDonResponse>> GetAllAsync();
    Task<HoaDonResponse?> GetByIdAsync(int id);
    Task<DoanhThuResponse> GetDoanhThuAsync(DateTime? tuNgay = null, DateTime? denNgay = null);
}
