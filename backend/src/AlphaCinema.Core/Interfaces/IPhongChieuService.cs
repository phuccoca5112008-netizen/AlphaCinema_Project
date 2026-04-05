using AlphaCinema.Core.DTOs.PhongChieu;

namespace AlphaCinema.Core.Interfaces;

public interface IPhongChieuService
{
    Task<IEnumerable<PhongChieuResponse>> GetAllAsync();
    Task<PhongChieuResponse?> GetByIdAsync(int id);
    Task<PhongChieuResponse> CreateAsync(CreatePhongChieuRequest request);
    Task DeleteAsync(int id);
    Task UpdateAsync(int id, CreatePhongChieuRequest request);
    Task GenerateGheAsync(int maPhong, GenerateGheRequest request);
    Task UpdateGheAsync(int maGhe, UpdateGheRequest request);
}
