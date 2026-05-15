using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTOs
{
    public class QuestionForLearnerDTO
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string QuestionText { get; set; } = null!;
        public string OptionA { get; set; } = null!;
        public string OptionB { get; set; } = null!;
        public string OptionC { get; set; } = null!;
        public string OptionD { get; set; } = null!;
    }
}
