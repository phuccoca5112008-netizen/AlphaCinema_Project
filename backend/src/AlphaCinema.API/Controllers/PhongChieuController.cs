using AlphaCinema.Core.DTOs.PhongChieu;
using AlphaCinema.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphaCinema.API.Controllers;

[ApiController]
[Route("api/phong-chieu")]
public class PhongChieuController : ControllerBase
{
    private readonly IPhongChieuService _service;

    public PhongChieuController(IPhongChieuService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(new { success = true, data = await _service.GetAllAsync() });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var data = await _service.GetByIdAsync(id);
        if (data == null) return NotFound(new { success = false, message = "Không tìm thấy phòng chiếu" });
        return Ok(new { success = true, data });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePhongChieuRequest request)
    {
        var result = await _service.CreateAsync(request);
        return Ok(new { success = true, data = result });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return Ok(new { success = true, message = "Xóa phòng chiếu thành công" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePhongChieuRequest request)
    {
        try
        {
            await _service.UpdateAsync(id, request);
            return Ok(new { success = true, message = "Cập nhật phòng chiếu thành công" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("{id}/ghe/generate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GenerateGhe(int id, [FromBody] GenerateGheRequest request)
    {
        try
        {
            await _service.GenerateGheAsync(id, request);
            return Ok(new { success = true, message = "Đã tạo sơ đồ ghế mới" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("ghe/{maGhe}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateGhe(int maGhe, [FromBody] UpdateGheRequest request)
    {
        try
        {
            await _service.UpdateGheAsync(maGhe, request);
            return Ok(new { success = true, message = "Đã cập nhật loại ghế" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
