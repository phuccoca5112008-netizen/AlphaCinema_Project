using AlphaCinema.Core.DTOs.KhuyenMai;
using AlphaCinema.Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphaCinema.API.Controllers;

[ApiController]
[Route("api/khuyen-mai")]
public class KhuyenMaiController : ControllerBase
{
    private readonly IKhuyenMaiService _service;
    private readonly IValidator<CreateKhuyenMaiRequest> _validator;

    public KhuyenMaiController(IKhuyenMaiService service, IValidator<CreateKhuyenMaiRequest> validator)
    {
        _service = service;
        _validator = validator;
    }

    /// <summary>Lấy danh sách khuyến mãi (Public)</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(new { success = true, data = await _service.GetAllAsync() });

    /// <summary>Chi tiết khuyến mãi (Public)</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(new { success = false, message = "Khuyến mãi không tồn tại." });
        return Ok(new { success = true, data = result });
    }

    /// <summary>Áp dụng mã giảm giá (Authenticated)</summary>
    [HttpPost("ap-dung")]
    [Authorize]
    public async Task<IActionResult> ApDung([FromBody] ApDungKhuyenMaiRequest request)
    {
        var result = await _service.ApDungMaGiamAsync(request);
        return Ok(new { success = true, data = result });
    }

    /// <summary>Tạo khuyến mãi (Admin)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateKhuyenMaiRequest request)
    {
        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(new { success = false, errors = validation.Errors.Select(e => e.ErrorMessage) });

        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.MaKhuyenMai }, new { success = true, data = result });
    }

    /// <summary>Cập nhật khuyến mãi (Admin)</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateKhuyenMaiRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        return Ok(new { success = true, data = result });
    }

    /// <summary>Xóa khuyến mãi (Admin)</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok(new { success = true, message = "Đã xóa khuyến mãi." });
    }
}
