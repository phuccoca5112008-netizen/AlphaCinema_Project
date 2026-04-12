using AlphaCinema.Core.DTOs.ThanhVien;

namespace AlphaCinema.Core.Interfaces;

public interface IThanhVienService
{
    Task<IEnumerable<RewardResponse>> GetAvailableRewardsAsync();
    Task<RedeemResponse> RedeemRewardAsync(int maNguoiDung, int maPhanThuong);
    Task<IEnumerable<RewardHistoryResponse>> GetRewardHistoryAsync(int maNguoiDung);
}
