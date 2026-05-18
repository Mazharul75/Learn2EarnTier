using System;
using System.Collections.Generic;
using DAL.EF.Tables;
using Microsoft.EntityFrameworkCore;

namespace DAL.EF;

public partial class Learn2EarnDBContext : DbContext
{
    public Learn2EarnDBContext()
    {
    }

    public Learn2EarnDBContext(DbContextOptions<Learn2EarnDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseTask> CourseTasks { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<MaterialCompletion> MaterialCompletions { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Quiz> Quizzes { get; set; }

    public virtual DbSet<QuizAttempt> QuizAttempts { get; set; }

    public virtual DbSet<TaskSubmission> TaskSubmissions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserType> UserTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DbConn");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Course");

            entity.Property(e => e.ContentLink).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())", "DF_Course_CreatedAt")
                .HasColumnType("datetime");
            entity.Property(e => e.Difficulty).HasMaxLength(20);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Instructor).WithMany(p => p.Courses)
                .HasForeignKey(d => d.InstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Course_User");

            entity.HasOne(d => d.Prerequisite).WithMany(p => p.InversePrerequisite)
                .HasForeignKey(d => d.PrerequisiteId)
                .HasConstraintName("FK_Course_Prerequisite");
        });

        modelBuilder.Entity<CourseTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseTa__3214EC07A7EBCB7D");

            entity.ToTable("CourseTask");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())", "DF_CourseTask_CreatedAt")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Course).WithMany(p => p.CourseTasks)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseTask_Course");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.ToTable("Enrollment");

            entity.HasIndex(e => new { e.LearnerId, e.CourseId }, "IX_Enrollment").IsUnique();

            entity.Property(e => e.EnrolledAt)
                .HasDefaultValueSql("(getdate())", "DF_Enrollment_EnrolledAt")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Course).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrollment_Course");

            entity.HasOne(d => d.Learner).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.LearnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrollment_User");
        });

        modelBuilder.Entity<MaterialCompletion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Material__3214EC073CB62866");

            entity.ToTable("MaterialCompletion");

            entity.HasIndex(e => new { e.LearnerId, e.CourseId }, "UQ_MaterialCompletion").IsUnique();

            entity.Property(e => e.CompletedAt)
                .HasDefaultValueSql("(getdate())", "DF_MaterialCompletion_CompletedAt")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Course).WithMany(p => p.MaterialCompletions)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaterialCompletion_Course");

            entity.HasOne(d => d.Learner).WithMany(p => p.MaterialCompletions)
                .HasForeignKey(d => d.LearnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaterialCompletion_Learner");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notification");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())", "DF_Notification_CreatedAt")
                .HasColumnType("datetime");
            entity.Property(e => e.Link).HasMaxLength(255);
            entity.Property(e => e.Message).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notification_User");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.ToTable("Question");

            entity.Property(e => e.CorrectOption)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.OptionA).HasMaxLength(255);
            entity.Property(e => e.OptionB).HasMaxLength(255);
            entity.Property(e => e.OptionC).HasMaxLength(255);
            entity.Property(e => e.OptionD).HasMaxLength(255);
            entity.Property(e => e.QuestionText).HasMaxLength(500);

            entity.HasOne(d => d.Quiz).WithMany(p => p.Questions)
                .HasForeignKey(d => d.QuizId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Question_Quiz");
        });

        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.ToTable("Quiz");

            entity.HasIndex(e => e.CourseId, "IX_Quiz").IsUnique();

            entity.Property(e => e.PassMark).HasDefaultValue(50, "DF_Quiz_PassMark");

            entity.HasOne(d => d.Course).WithOne(p => p.Quiz)
                .HasForeignKey<Quiz>(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Quiz_Course");
        });

        modelBuilder.Entity<QuizAttempt>(entity =>
        {
            entity.ToTable("QuizAttempt");

            entity.Property(e => e.AttemptedAt)
                .HasDefaultValueSql("(getdate())", "DF_QuizAttempt_AttemptedAt")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Learner).WithMany(p => p.QuizAttempts)
                .HasForeignKey(d => d.LearnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QuizAttempt_User");

            entity.HasOne(d => d.Quiz).WithMany(p => p.QuizAttempts)
                .HasForeignKey(d => d.QuizId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QuizAttempt_Quiz");
        });

        modelBuilder.Entity<TaskSubmission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TaskSubm__3214EC07149CBF92");

            entity.ToTable("TaskSubmission");

            entity.HasIndex(e => new { e.TaskId, e.LearnerId }, "UQ_TaskSubmission").IsUnique();

            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending", "DF_TaskSubmission_Status");
            entity.Property(e => e.SubmissionLink).HasMaxLength(500);
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("(getdate())", "DF_TaskSubmission_SubmittedAt")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Learner).WithMany(p => p.TaskSubmissions)
                .HasForeignKey(d => d.LearnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskSubmission_Learner");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskSubmissions)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskSubmission_Task");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "IX_User").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())", "DF_User_CreatedAt")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);

            entity.HasOne(d => d.UserType).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_UserType");
        });

        modelBuilder.Entity<UserType>(entity =>
        {
            entity.ToTable("UserType");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
