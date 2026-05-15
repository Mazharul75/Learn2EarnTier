using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class QuizDTO
    {
        public int Id { get; set; }
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Pass mark is required")]
        [Range(1, 100, ErrorMessage = "Pass mark must be between 1 and 100")]
        public int PassMark { get; set; } = 50;

        // Display fields populated by the service
        public string? CourseTitle { get; set; }
        public List<QuestionDTO> Questions { get; set; } = new List<QuestionDTO>();
    }
}
