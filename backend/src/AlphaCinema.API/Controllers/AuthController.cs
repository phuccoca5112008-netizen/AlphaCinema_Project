using AlphaCinema.Core.DTOs.Auth;
using AlphaCinema.Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AlphaCinema.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthController(IAuthService authService,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator)
    {
        _authService = authService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    /// <summary>Đăng ký tài khoản mới</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var validation = await _registerValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(new { success = false, errors = validation.Errors.Select(e => e.ErrorMessage) });

        var result = await _authService.RegisterAsync(request);
        return Ok(new { success = true, data = result });
    }

    /// <summary>Đăng nhập</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var validation = await _loginValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(new { success = false, errors = validation.Errors.Select(e => e.ErrorMessage) });

        var result = await _authService.LoginAsync(request);
        return Ok(new { success = true, data = result });
    }
}
