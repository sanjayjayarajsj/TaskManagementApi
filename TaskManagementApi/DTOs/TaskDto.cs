
using System.ComponentModel.DataAnnotations;

namespace TaskManagementApi.DTOs
{
    public class TaskDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";
    }
}
