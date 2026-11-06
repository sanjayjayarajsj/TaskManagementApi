
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.DTOs;
using TaskManagementApi.Services.Interfaces;

namespace TaskManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // REGISTER
        [HttpPost("register")]
        public IActionResult Register(RegisterDto request)
        {
            try
            {
                var result = _authService.Register(request);

                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // LOGIN
        [HttpPost("login")]
        public IActionResult Login(LoginDto request)
        {
            try
            {
                var token = _authService.Login(request);

                return Ok(new { token = token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
