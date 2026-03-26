using System.Security.Claims;
using AlphaCinema.Core.DTOs.DanhGia;
using AlphaCinema.Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphaCinema.API.Controllers;

[ApiController]
[Route("api/danh-gia")]
public class DanhGiaController : ControllerBase
{
    private readonly IDanhGiaService _service;
    private readonly IValidator<CreateDanhGiaRequest> _validator;

    public DanhGiaController(IDanhGiaService service, IValidator<CreateDanhGiaRequest> validator)
    {
        _service = service;
        _validator = validator;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Lấy đánh giá theo phim (Public)</summary>
    [HttpGet("phim/{maPhim}")]
    public async Task<IActionResult> GetByPhim(int maPhim)
        => Ok(new { success = true, data = await _service.GetByPhimAsync(maPhim) });

    /// <summary>Lấy tất cả đánh giá (Admin)</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
        => Ok(new { success = true, data = await _service.GetAllAsync() });

    /// <summary>Thêm đánh giá (Cần mua vé trước)</summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateDanhGiaRequest request)
    {
        try {
            var validation = await _validator.ValidateAsync(request);
            if (!validation.IsValid)
                return BadRequest(new { success = false, errors = validation.Errors.Select(e => e.ErrorMessage) });

            var result = await _service.CreateAsync(GetUserId(), request);
            return Ok(new { success = true, data = result, message = "Đánh giá thành công!" });
        } catch (Exception ex) {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>Kiểm tra quyền bình luận</summary>
    [HttpGet("check-eligibility/{maPhim}")]
    [Authorize]
    public async Task<IActionResult> CheckEligibility(int maPhim)
    {
        var eligible = await _service.IsUserEligibleToReview(GetUserId(), maPhim);
        return Ok(new { success = true, data = eligible });
    }

    /// <summary>Xóa đánh giá (Admin)</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok(new { success = true, message = "Đã xóa đánh giá." });
    }
}
