
using TaskManagementApi.DTOs;
using TaskManagementApi.Models;
using TaskManagementApi.Services.Interfaces;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace TaskManagementApi.Services
{
    public class AuthService : IAuthService
    {
        private static List<AppUser> users = new();

        private readonly IConfiguration _configuration;

        private readonly PasswordHasher<AppUser> _passwordHasher = new();

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;

            // Create Admin account
            var adminEmail = _configuration["Admin:Email"];
            var adminPassword = _configuration["Admin:Password"];

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

        // REGISTER
        public string Register(RegisterDto request)
        {
            if (users.Any(u => u.Email == request.Email))
            {
                throw new Exception("Email already exists");
            }

            var user = new AppUser
            {
                Id = users.Count + 1,
                Username = request.Username,
                Email = request.Email,
                Role = "User"
            };

            user.Password = _passwordHasher.HashPassword(
                user, request.Password
            );

            users.Add(user);

            return "Registration successful";
        }

        // LOGIN
        public TokenResponseDto Login(LoginDto request)
        {
            var user = users.FirstOrDefault(
                u => u.Email == request.Email
            );

            if (user == null)
                throw new Exception("Invalid email or password");

            var result = _passwordHasher.VerifyHashedPassword(
                user, user.Password, request.Password
            );

            if (result == PasswordVerificationResult.Failed)
                throw new Exception("Invalid email or password");

            var accessToken = GenerateAccessToken(user);

            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;

            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        // GENERATE ACCESS TOKEN
        private string GenerateAccessToken(AppUser user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!
                )
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()
                ),

                new Claim(
                    ClaimTypes.Name,
                    user.Username
                ),

                new Claim(
                    ClaimTypes.Role,
                    user.Role
                )
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // GENERATE REFRESH TOKEN
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];

            using var random = RandomNumberGenerator.Create();

            random.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }

        // REFRESH TOKEN
        public TokenResponseDto RefreshToken(RefreshTokenDto request)
        {
            var user = users.FirstOrDefault(
                u => u.RefreshToken == request.RefreshToken
            );

            if (user == null ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new Exception(
                    "Invalid or expired refresh token"
                );
            }

            var newAccessToken = GenerateAccessToken(user);

            var newRefreshToken = GenerateRefreshToken();

            // Replace old refresh token
            user.RefreshToken = newRefreshToken;

            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            return new TokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
    }
}
