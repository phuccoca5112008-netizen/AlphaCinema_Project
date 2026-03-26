using AlphaCinema.Core.DTOs.Phim;
using AlphaCinema.Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphaCinema.API.Controllers;

[ApiController]
[Route("api/phim")]
public class PhimController : ControllerBase
{
    private readonly IPhimService _phimService;
    private readonly IValidator<CreatePhimRequest> _validator;

    public PhimController(IPhimService phimService, IValidator<CreatePhimRequest> validator)
    {
        _phimService = phimService;
        _validator = validator;
    }

    /// <summary>Lấy danh sách phim (Public) - hỗ trợ filter</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? tuKhoa,
        [FromQuery] string? trangThai,
        [FromQuery] string? theLoai)
    {
        var result = await _phimService.GetAllAsync(tuKhoa, trangThai, theLoai);
        return Ok(new { success = true, data = result });
    }

    /// <summary>Lấy chi tiết phim theo ID (Public)</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _phimService.GetByIdAsync(id);
        if (result == null) return NotFound(new { success = false, message = "Phim không tồn tại." });
        return Ok(new { success = true, data = result });
    }

    /// <summary>Thêm phim mới (Admin only)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePhimRequest request)
    {
        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(new { success = false, errors = validation.Errors.Select(e => e.ErrorMessage) });

        var result = await _phimService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.MaPhim }, new { success = true, data = result });
    }

    /// <summary>Cập nhật phim (Admin only)</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePhimRequest request)
    {
        var result = await _phimService.UpdateAsync(id, request);
        return Ok(new { success = true, data = result });
    }

    /// <summary>Xóa phim (Admin only)</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _phimService.DeleteAsync(id);
        return Ok(new { success = true, message = "Đã xóa phim thành công." });
    }

    /// <summary>Khởi tạo dữ liệu phim, phòng, suất chiếu (Admin / System)</summary>
    [HttpPost("seed")]
    public async Task<IActionResult> SeedTmdbData()
    {
        await _phimService.SeedTmdbDataAsync();
        return Ok(new { success = true, message = "Đã bơm dữ liệu phim và suất chiếu thành công!" });
    }
}
