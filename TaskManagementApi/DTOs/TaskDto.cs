
using System.ComponentModel.DataAnnotations;

namespace TaskManagementApi.DTOs
{
    public class TaskDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [RegularExpression("^(Pending|InProgress|Completed)$",
     ErrorMessage = "Status must be Pending, InProgress, or Completed")]
        public string Status { get; set; } = "Pending";
    }
}
