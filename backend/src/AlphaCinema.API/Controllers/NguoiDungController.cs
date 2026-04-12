using System.Security.Claims;
using AlphaCinema.Core.DTOs.NguoiDung;
using AlphaCinema.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphaCinema.API.Controllers;

[ApiController]
[Route("api/nguoi-dung")]
[Authorize]
public class NguoiDungController : ControllerBase
{
    private readonly INguoiDungService _service;

    public NguoiDungController(INguoiDungService service) => _service = service;

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Lấy danh sách tất cả người dùng (Admin)</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
        => Ok(new { success = true, data = await _service.GetAllAsync() });

    /// <summary>Lấy profile của tôi</summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var result = await _service.GetByIdAsync(GetUserId());
        if (result == null) return NotFound();
        return Ok(new { success = true, data = result });
    }

    /// <summary>Lấy người dùng theo ID (Admin)</summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(new { success = false, message = "Người dùng không tồn tại." });
        return Ok(new { success = true, data = result });
    }

    /// <summary>Cập nhật thông tin cá nhân</summary>
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateNguoiDungRequest request)
    {
        var result = await _service.UpdateAsync(GetUserId(), request);
        return Ok(new { success = true, data = result });
    }

    /// <summary>Cập nhật thông tin người dùng (Admin)</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateNguoiDungRequest request)
    {
        // Admin không được phép đổi mật khẩu của người dùng khác qua endpoint này
        request.MatKhauMoi = null;
        request.MatKhauCu = null;

        var result = await _service.UpdateAsync(id, request);
        return Ok(new { success = true, data = result });
    }

    /// <summary>Xóa người dùng (Admin)</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok(new { success = true, message = "Đã xóa người dùng." });
    }
}
