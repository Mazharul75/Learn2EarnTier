using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTOs
{
    public class ProgressDTO
    {
        // Headline numbers
        public int EnrolledCoursesCount { get; set; }
        public int QuizzesAttempted { get; set; }
        public int QuizzesPassed { get; set; }
        public int AverageScore { get; set; }
        public DateTime? LastAttemptedAt { get; set; }

        // Per-course score data for the chart
        public List<CourseScoreDTO> CourseScores { get; set; } = new List<CourseScoreDTO>();
    }

    public class CourseScoreDTO
    {
        public string CourseTitle { get; set; } = null!;
        public int BestScore { get; set; }
        public bool Passed { get; set; }
    }
}