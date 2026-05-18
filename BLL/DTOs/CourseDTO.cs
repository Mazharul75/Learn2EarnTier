using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class CourseDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be 3–200 characters")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be at least 10 characters")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Difficulty is required")]
        public string Difficulty { get; set; } = null!;

        public int InstructorId { get; set; }

        public string? InstructorName { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(500)]
        public string? ContentLink { get; set; }

        [Range(0, 1000, ErrorMessage = "Capacity must be between 0 and 1000. Use 0 for unlimited.")]
        public int MaxCapacity { get; set; } = 0;  

        public int? PrerequisiteId { get; set; }
        public string? PrerequisiteTitle { get; set; } 
        public int CurrentEnrollmentCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
