using System;
using System.Collections.Generic;

namespace DAL.EF.Tables;

public partial class QuizAttempt
{
    public int Id { get; set; }

    public int LearnerId { get; set; }

    public int QuizId { get; set; }

    public int Score { get; set; }

    public bool Passed { get; set; }

    public DateTime AttemptedAt { get; set; }

    public virtual User Learner { get; set; } = null!;

    public virtual Quiz Quiz { get; set; } = null!;
}
