using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class QuestionDTO
    {
        public int Id { get; set; }
        public int QuizId { get; set; }

        [Required(ErrorMessage = "Question text is required")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Question must be 5–500 characters")]
        public string QuestionText { get; set; } = null!;

        [Required(ErrorMessage = "Option A is required")]
        public string OptionA { get; set; } = null!;

        [Required(ErrorMessage = "Option B is required")]
        public string OptionB { get; set; } = null!;

        [Required(ErrorMessage = "Option C is required")]
        public string OptionC { get; set; } = null!;

        [Required(ErrorMessage = "Option D is required")]
        public string OptionD { get; set; } = null!;

        [Required(ErrorMessage = "Please specify the correct option")]
        [RegularExpression("^[ABCD]$", ErrorMessage = "Correct option must be A, B, C, or D")]
        public string CorrectOption { get; set; } = null!;
    }
}
