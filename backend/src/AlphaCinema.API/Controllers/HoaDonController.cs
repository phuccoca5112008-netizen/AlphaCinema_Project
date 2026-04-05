using System.Security.Claims;
using AlphaCinema.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphaCinema.API.Controllers;

[ApiController]
[Route("api/hoa-don")]
[Authorize]
public class HoaDonController : ControllerBase
{
    private readonly IHoaDonService _service;

    public HoaDonController(IHoaDonService service) => _service = service;

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Lấy lịch sử hóa đơn của tôi</summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMyHoaDon()
    {
        var userId = GetUserId();
        Console.WriteLine($"[DEBUG] GetMyHoaDon for UserID={userId}");
        var result = await _service.GetByNguoiDungAsync(userId);
        Console.WriteLine($"[DEBUG] Found {result.Count()} invoices.");
        return Ok(new { success = true, data = result });
    }

    /// <summary>Lấy tất cả hóa đơn (Admin)</summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(new { success = true, data = result });
    }

    /// <summary>Chi tiết hóa đơn</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(new { success = false, message = "Hóa đơn không tồn tại." });
        return Ok(new { success = true, data = result });
    }

    /// <summary>Thống kê doanh thu (Admin)</summary>
    [HttpGet("doanh-thu")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetDoanhThu([FromQuery] DateTime? tuNgay, [FromQuery] DateTime? denNgay)
    {
        var result = await _service.GetDoanhThuAsync(tuNgay, denNgay);
        return Ok(new { success = true, data = result });
    }
}
