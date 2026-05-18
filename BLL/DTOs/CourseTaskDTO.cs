using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class CourseTaskDTO
    {
        public int Id { get; set; }
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Task title is required")]
        [StringLength(200, MinimumLength = 3)]
        public string Title { get; set; } = null!;

        [StringLength(2000)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}