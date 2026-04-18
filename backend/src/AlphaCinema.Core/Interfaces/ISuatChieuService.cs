using AlphaCinema.Core.DTOs.SuatChieu;

namespace AlphaCinema.Core.Interfaces;

public interface ISuatChieuService
{
    Task<IEnumerable<SuatChieuResponse>> GetAllAsync(int? maPhim = null, DateTime? ngay = null);
    Task<SuatChieuResponse?> GetByIdAsync(int id);
    Task<IEnumerable<GheResponse>> GetGheBySuatChieuAsync(int maSuatChieu);
    Task<SuatChieuResponse> CreateAsync(CreateSuatChieuRequest request);
    Task<SuatChieuResponse> UpdateAsync(int id, UpdateSuatChieuRequest request);
    Task DeleteAsync(int id);
    Task CleanupOldSuatChieuAsync();
}
