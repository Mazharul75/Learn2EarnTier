using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class TaskSubmissionDTO
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int LearnerId { get; set; }

        [Required(ErrorMessage = "Submission link is required")]
        [Url(ErrorMessage = "Please provide a valid URL")]
        [StringLength(500)]
        public string SubmissionLink { get; set; } = null!;

        public string Status { get; set; } = "Pending";
        public string? InstructorFeedback { get; set; }
        public DateTime SubmittedAt { get; set; }

        // Display fields
        public string? TaskTitle { get; set; }
        public string? LearnerName { get; set; }
    }
}