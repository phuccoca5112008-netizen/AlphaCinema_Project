using AlphaCinema.Core.DTOs.SuatChieu;
using AlphaCinema.Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphaCinema.API.Controllers;

[ApiController]
[Route("api/suat-chieu")]
public class SuatChieuController : ControllerBase
{
    private readonly ISuatChieuService _service;
    private readonly IValidator<CreateSuatChieuRequest> _validator;

    public SuatChieuController(ISuatChieuService service, IValidator<CreateSuatChieuRequest> validator)
    {
        _service = service;
        _validator = validator;
    }

    /// <summary>Lấy danh sách suất chiếu (Public)</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? maPhim, [FromQuery] DateTime? ngay)
    {
        var result = await _service.GetAllAsync(maPhim, ngay);
        return Ok(new { success = true, data = result });
    }

    /// <summary>Lấy chi tiết suất chiếu (Public)</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(new { success = false, message = "Suất chiếu không tồn tại." });
        return Ok(new { success = true, data = result });
    }

    /// <summary>Lấy danh sách ghế của suất chiếu (Public)</summary>
    [HttpGet("{id}/ghe")]
    public async Task<IActionResult> GetGhe(int id)
    {
        var result = await _service.GetGheBySuatChieuAsync(id);
        return Ok(new { success = true, data = result });
    }

    /// <summary>Thêm suất chiếu (Admin only)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Create([FromBody] CreateSuatChieuRequest request)
    {
        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(new { success = false, errors = validation.Errors.Select(e => e.ErrorMessage) });

        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.MaSuatChieu }, new { success = true, data = result });
    }

    /// <summary>Cập nhật suất chiếu (Admin only)</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSuatChieuRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        return Ok(new { success = true, data = result });
    }

    /// <summary>Xóa suất chiếu (Admin only)</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok(new { success = true, message = "Đã xóa suất chiếu thành công." });
    }
}
