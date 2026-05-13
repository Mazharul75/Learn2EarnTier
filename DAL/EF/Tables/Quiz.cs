using System;
using System.Collections.Generic;

namespace DAL.EF.Tables;

public partial class Quiz
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public int PassMark { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
}
