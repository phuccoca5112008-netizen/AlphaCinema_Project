using AlphaCinema.Core.DTOs.NguoiDung;

namespace AlphaCinema.Core.Interfaces;

public interface INguoiDungService
{
    Task<IEnumerable<NguoiDungResponse>> GetAllAsync();
    Task<NguoiDungResponse?> GetByIdAsync(int id);
    Task<NguoiDungResponse> UpdateAsync(int id, UpdateNguoiDungRequest request);
    Task DeleteAsync(int id);
}
