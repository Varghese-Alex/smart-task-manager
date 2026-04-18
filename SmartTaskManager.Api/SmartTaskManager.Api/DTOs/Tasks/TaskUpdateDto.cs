using SmartTaskManager.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace SmartTaskManager.Api.DTOs.Tasks
{
    public class TaskUpdateDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskItemStatus Status { get; set; }
    }
}
