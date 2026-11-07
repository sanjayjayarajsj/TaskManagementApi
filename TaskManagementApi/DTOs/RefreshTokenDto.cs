using System.ComponentModel.DataAnnotations;

namespace TaskManagementApi.DTOs
{
    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}