using AlphaCinema.Core.DTOs.Ve;

namespace AlphaCinema.Core.Interfaces;

public interface IDatVeService
{
    Task<DatVeResponse> DatVeAsync(int maNguoiDung, DatVeRequest request);
    Task<DatVeResponse> LockSeatsAsync(int maNguoiDung, LockSeatsRequest request);
    Task<IEnumerable<VeResponse>> GetVeByNguoiDungAsync(int maNguoiDung);
    Task<VeResponse?> GetVeByIdAsync(int maVe);
    Task HuyVeAsync(int maVe, int maNguoiDung);
    Task<VeResponse> CheckInTicketAsync(string code);
    Task<IEnumerable<ConcessionResponse>> GetConcessionsAsync();
}
