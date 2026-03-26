using AlphaCinema.Core.DTOs.DanhGia;

namespace AlphaCinema.Core.Interfaces;

public interface IDanhGiaService
{
    Task<IEnumerable<DanhGiaResponse>> GetByPhimAsync(int maPhim);
    Task<IEnumerable<DanhGiaResponse>> GetAllAsync();
    Task<DanhGiaResponse> CreateAsync(int maNguoiDung, CreateDanhGiaRequest request);
    Task<bool> IsUserEligibleToReview(int maNguoiDung, int maPhim);
    Task DeleteAsync(int maDanhGia);
}
