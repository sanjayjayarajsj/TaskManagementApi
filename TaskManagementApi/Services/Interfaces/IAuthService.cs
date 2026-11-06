using TaskManagementApi.DTOs;


namespace TaskManagementApi.Services.Interfaces
{
    public interface IAuthService
    {
        string Register(RegisterDto request);
        string Login(LoginDto request);
    }
}
