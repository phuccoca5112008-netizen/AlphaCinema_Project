using System.Security.Claims;
using AlphaCinema.Core.DTOs.ThanhVien;
using AlphaCinema.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphaCinema.API.Controllers;

[ApiController]
[Route("api/thanh-vien")]
[Authorize]
public class ThanhVienController : ControllerBase
{
    private readonly IThanhVienService _service;

    public ThanhVienController(IThanhVienService service)
    {
        _service = service;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("rewards")]
    public async Task<IActionResult> GetRewards()
    {
        var result = await _service.GetAvailableRewardsAsync();
        return Ok(new { success = true, data = result });
    }

    [HttpPost("redeem")]
    public async Task<IActionResult> Redeem([FromBody] RedeemRequest request)
    {
        try
        {
            var result = await _service.RedeemRewardAsync(GetUserId(), request.MaPhanThuong);
            return Ok(new { success = result.Success, data = result, message = result.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("reward-history")]
    public async Task<IActionResult> GetHistory()
    {
        var result = await _service.GetRewardHistoryAsync(GetUserId());
        return Ok(new { success = true, data = result });
    }
}
