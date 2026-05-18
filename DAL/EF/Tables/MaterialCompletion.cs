using System;
using System.Collections.Generic;

namespace DAL.EF.Tables;

public partial class MaterialCompletion
{
    public int Id { get; set; }

    public int LearnerId { get; set; }

    public int CourseId { get; set; }

    public DateTime CompletedAt { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual User Learner { get; set; } = null!;
}
