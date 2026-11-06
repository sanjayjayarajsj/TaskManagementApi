using TaskManagementApi.DTOs;
using TaskManagementApi.Models;
using TaskManagementApi.Services.Interfaces;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace TaskManagementApi.Services
{
    public class AuthService:IAuthService
    {
        private static List<AppUser> users = new();
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<AppUser> _passwordHasher = new();
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;

            // Get Admin credentials from configuration
            var adminEmail = _configuration["Admin:Email"];
            var adminPassword = _configuration["Admin:Password"];

            // Create Admin if it doesn't already exist
            if (!string.IsNullOrWhiteSpace(adminEmail) &&
                !string.IsNullOrWhiteSpace(adminPassword) &&
                !users.Any(u => u.Email == adminEmail))
            {
                var admin = new AppUser
                {
                    Id = users.Count + 1,
                    Username = "Admin",
                    Email = adminEmail,
                    Role = "Admin"
                };

                admin.Password = _passwordHasher.HashPassword(
                    admin, adminPassword
                );

                users.Add(admin);
            }
        }
        public string Register(RegisterDto request)
        {
            // Check existing email
            if (users.Any(u => u.Email == request.Email))
            {
                throw new Exception("Email already exists");
            }

            // Create user
            var user = new AppUser
            {
                Id = users.Count + 1,
                Username = request.Username,
                Email = request.Email,
                Role = "User"
            };

            // Hash password
            user.Password = _passwordHasher.HashPassword(
                user, request.Password
            );

            users.Add(user);

            return "Registration successful";
        }

        // LOGIN
        public string Login(LoginDto request)
        {
            var user = users.FirstOrDefault(
                u => u.Email == request.Email
            );

            if (user == null)
            {
                throw new Exception("Invalid email or password");
            }

            // Verify password
            var result = _passwordHasher.VerifyHashedPassword(
                user, user.Password, request.Password
            );

            if (result == PasswordVerificationResult.Failed)
            {
                throw new Exception("Invalid email or password");
            }

            // Create JWT token
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!
                )
            );

            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
