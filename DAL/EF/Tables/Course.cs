using System;
using System.Collections.Generic;

namespace DAL.EF.Tables;

public partial class Course
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Difficulty { get; set; } = null!;

    public int InstructorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? ContentLink { get; set; }

    public int? PrerequisiteId { get; set; }

    public int? MaxCapacity { get; set; }

    public virtual ICollection<CourseTask> CourseTasks { get; set; } = new List<CourseTask>();

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual User Instructor { get; set; } = null!;

    public virtual ICollection<Course> InversePrerequisite { get; set; } = new List<Course>();

    public virtual ICollection<MaterialCompletion> MaterialCompletions { get; set; } = new List<MaterialCompletion>();

    public virtual Course? Prerequisite { get; set; }

    public virtual Quiz? Quiz { get; set; }
}
