using System.Security.Claims;
using AlphaCinema.Core.DTOs.Ve;
using AlphaCinema.Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphaCinema.API.Controllers;

[ApiController]
[Route("api/dat-ve")]
[Authorize]
public class DatVeController : ControllerBase
{
    private readonly IDatVeService _service;
    private readonly IValidator<DatVeRequest> _validator;

    public DatVeController(IDatVeService service, IValidator<DatVeRequest> validator)
    {
        _service = service;
        _validator = validator;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Đặt vé (Customer)</summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DatVe([FromBody] DatVeRequest request)
    {
        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(new { success = false, errors = validation.Errors.Select(e => e.ErrorMessage) });

        try
        {
            var result = await _service.DatVeAsync(GetUserId(), request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CRITICAL] DatVe Error: {ex}");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>Lấy vé của người dùng hiện tại</summary>
    [HttpGet("my-tickets")]
    public async Task<IActionResult> GetMyTickets()
    {
        var result = await _service.GetVeByNguoiDungAsync(GetUserId());
        return Ok(new { success = true, data = result });
    }

    /// <summary>Lấy chi tiết vé</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetVeByIdAsync(id);
        if (result == null) return NotFound(new { success = false, message = "Vé không tồn tại." });
        return Ok(new { success = true, data = result });
    }

    /// <summary>Hủy vé</summary>
    [HttpPatch("{id}/huy")]
    [Authorize(Roles = "Customer,Admin")]
    public async Task<IActionResult> HuyVe(int id)
    {
        await _service.HuyVeAsync(id, GetUserId());
        return Ok(new { success = true, message = "Hủy vé thành công." });
    }

    /// <summary>Kiểm tra vé của khách (Admin/Staff)</summary>
    [HttpGet("kiem-tra/{id}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> KiemTraVe(int id)
    {
        var result = await _service.GetVeByIdAsync(id);
        if (result == null) return NotFound(new { success = false, message = "Vé không tồn tại." });
        return Ok(new { success = true, data = result });
    }

    /// <summary>Kiểm tra và check-in vé (Admin/Staff)</summary>
    [HttpPost("check-in")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> CheckIn([FromBody] CheckInTicketRequest request)
    {
        try
        {
            var result = await _service.CheckInTicketAsync(request.Code);
            return Ok(new { success = true, data = result, message = "Vé hợp lệ. Đã thực hiện check-in thành công." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
