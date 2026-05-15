using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTOs
{
    public class QuizResultDTO
    {
        public int CourseId { get; set; }
        public int AttemptId { get; set; }
        public int QuizId { get; set; }
        public string CourseTitle { get; set; } = null!;
        public int Score { get; set; }          // 0-100 percentage
        public int PassMark { get; set; }
        public bool Passed { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectCount { get; set; }
        public DateTime AttemptedAt { get; set; }

        // Per-question breakdown so learner can review their mistakes
        public List<QuestionResultDTO> Breakdown { get; set; } = new List<QuestionResultDTO>();
    }

    public class QuestionResultDTO
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = null!;
        public string OptionA { get; set; } = null!;
        public string OptionB { get; set; } = null!;
        public string OptionC { get; set; } = null!;
        public string OptionD { get; set; } = null!;
        public string CorrectOption { get; set; } = null!;
        public string? PickedOption { get; set; }
        public bool IsCorrect { get; set; }
    }
}