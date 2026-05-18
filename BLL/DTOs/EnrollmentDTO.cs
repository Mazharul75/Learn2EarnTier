using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTOs
{
    public class EnrollmentDTO
    {
        public int Id { get; set; }
        public int LearnerId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrolledAt { get; set; }
        public string? CourseContentLink { get; set; }

        public string? LearnerName { get; set; }
        public string? LearnerEmail { get; set; }
        // Convenience fields for display (filled by service)
        public string? CourseTitle { get; set; }
        public string? CourseDescription { get; set; }
        public string? CourseDifficulty { get; set; }
        public string? InstructorName { get; set; }
    }
}