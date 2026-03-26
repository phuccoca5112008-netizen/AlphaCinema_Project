using AlphaCinema.Core.DTOs.KhuyenMai;

namespace AlphaCinema.Core.Interfaces;

public interface IKhuyenMaiService
{
    Task<IEnumerable<KhuyenMaiResponse>> GetAllAsync();
    Task<KhuyenMaiResponse?> GetByIdAsync(int id);
    Task<ApDungKhuyenMaiResponse> ApDungMaGiamAsync(ApDungKhuyenMaiRequest request);
    Task<KhuyenMaiResponse> CreateAsync(CreateKhuyenMaiRequest request);
    Task<KhuyenMaiResponse> UpdateAsync(int id, CreateKhuyenMaiRequest request);
    Task DeleteAsync(int id);
}
