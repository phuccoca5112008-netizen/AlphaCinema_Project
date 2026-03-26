using AlphaCinema.Core.DTOs.Phim;

namespace AlphaCinema.Core.Interfaces;

public interface IPhimService
{
    Task<IEnumerable<PhimResponse>> GetAllAsync(string? tuKhoa = null, string? trangThai = null, string? theLoai = null);
    Task<PhimResponse?> GetByIdAsync(int id);
    Task<PhimResponse> CreateAsync(CreatePhimRequest request);
    Task<PhimResponse> UpdateAsync(int id, UpdatePhimRequest request);
    Task DeleteAsync(int id);
    Task SeedTmdbDataAsync();
}
