using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTOs
{
    public class QuizSubmissionDTO
    {
        public int QuizId { get; set; }

        // Maps question Id → picked option ("A", "B", "C", or "D")
        // Will be populated from form fields like name="Answers[5]" value="A"
        public Dictionary<int, string> Answers { get; set; } = new Dictionary<int, string>();
    }
}