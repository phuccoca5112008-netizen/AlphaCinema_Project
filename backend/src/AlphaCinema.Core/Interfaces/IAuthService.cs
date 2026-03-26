using AlphaCinema.Core.DTOs.Auth;

namespace AlphaCinema.Core.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
