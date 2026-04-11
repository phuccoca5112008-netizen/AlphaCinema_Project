using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AlphaCinema.Core.DTOs.Auth;
using AlphaCinema.Core.Interfaces;
using AlphaCinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AlphaCinema.Services.Services;

public class AuthService : IAuthService
{
    private readonly AlphaCinemaDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AlphaCinemaDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _context.NguoiDungs.AnyAsync(u => u.Email == request.Email))
            throw new ArgumentException("Tài khoản hoặc Email này đã được sử dụng.");

        // Lấy vai trò Customer từ DB để tránh lỗi Hardcode ID
        var vaiTro = await _context.VaiTros.FirstOrDefaultAsync(v => v.TenVaiTro == "Customer")
            ?? throw new Exception("Hệ thống chưa thiết lập vai trò 'Customer'. Vui lòng liên hệ Admin.");

        var nguoiDung = new Core.Entities.NguoiDung
        {
            Email = request.Email,
            MatKhau = BCrypt.Net.BCrypt.HashPassword(request.MatKhau),
            HoTen = request.HoTen,
            MaVaiTro = vaiTro.MaVaiTro,
            DiemTichLuy = 0
        };

        _context.NguoiDungs.Add(nguoiDung);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Token = GenerateJwt(nguoiDung, vaiTro.TenVaiTro),
            Email = nguoiDung.Email,
            HoTen = nguoiDung.HoTen,
            VaiTro = vaiTro.TenVaiTro,
            MaNguoiDung = nguoiDung.MaNguoiDung
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var nguoiDung = await _context.NguoiDungs
            .Include(u => u.VaiTro)
            .FirstOrDefaultAsync(u => u.Email == request.Email || u.HoTen == request.Email)
            ?? throw new ArgumentException("Tài khoản hoặc mật khẩu không đúng.");

        if (!BCrypt.Net.BCrypt.Verify(request.MatKhau, nguoiDung.MatKhau))
            throw new ArgumentException("Tài khoản hoặc mật khẩu không đúng.");

        return new AuthResponse
        {
            Token = GenerateJwt(nguoiDung, nguoiDung.VaiTro.TenVaiTro),
            Email = nguoiDung.Email,
            HoTen = nguoiDung.HoTen,
            VaiTro = nguoiDung.VaiTro.TenVaiTro,
            MaNguoiDung = nguoiDung.MaNguoiDung
        };
    }

    private string GenerateJwt(Core.Entities.NguoiDung user, string vaiTro)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.MaNguoiDung.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.HoTen),
            new Claim(ClaimTypes.Role, vaiTro)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(int.Parse(_config["Jwt:ExpiresInDays"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
