using TaskManagementApi.DTOs;

namespace TaskManagementApi.Services.Interfaces
{
    public interface IAuthService
    {
        string Register(RegisterDto request);

        TokenResponseDto Login(LoginDto request);

        TokenResponseDto RefreshToken(RefreshTokenDto request);
    }
}