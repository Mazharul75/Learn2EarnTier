using System;
using System.Collections.Generic;

namespace DAL.EF.Tables;

public partial class TaskSubmission
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public int LearnerId { get; set; }

    public string SubmissionLink { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? InstructorFeedback { get; set; }

    public DateTime SubmittedAt { get; set; }

    public virtual User Learner { get; set; } = null!;

    public virtual CourseTask Task { get; set; } = null!;
}
