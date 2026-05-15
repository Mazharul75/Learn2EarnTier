using System;
using System.Collections.Generic;
using System.Text;
using BLL.DTOs;
using DAL.Repos;

namespace BLL.Services
{
    public class ProgressService
    {
        EnrollmentRepo enrollmentRepo;
        QuizAttemptRepo attemptRepo;
        QuizRepo quizRepo;
        CourseRepo courseRepo;

        public ProgressService(EnrollmentRepo enrollmentRepo,
                               QuizAttemptRepo attemptRepo,
                               QuizRepo quizRepo,
                               CourseRepo courseRepo)
        {
            this.enrollmentRepo = enrollmentRepo;
            this.attemptRepo = attemptRepo;
            this.quizRepo = quizRepo;
            this.courseRepo = courseRepo;
        }

        public ProgressDTO GetLearnerStats(int learnerId)
        {
            var dto = new ProgressDTO();

            // ===== Enrollments =====
            var enrollments = enrollmentRepo.GetByLearner(learnerId);
            dto.EnrolledCoursesCount = enrollments.Count;

            // ===== All attempts by this learner =====
            var allAttempts = attemptRepo.GetByLearner(learnerId);

            if (allAttempts.Count == 0)
            {
                // No quiz activity yet — leave stats at default (0s, null)
                return dto;
            }

            // ===== Group attempts by Quiz, keep best score per quiz =====
            var bestPerQuiz = allAttempts
                .GroupBy(a => a.QuizId)
                .Select(g => new
                {
                    QuizId = g.Key,
                    BestScore = g.Max(a => a.Score),
                    EverPassed = g.Any(a => a.Passed)
                })
                .ToList();

            dto.QuizzesAttempted = bestPerQuiz.Count;
            dto.QuizzesPassed = bestPerQuiz.Count(x => x.EverPassed);
            dto.AverageScore = (int)Math.Round(bestPerQuiz.Average(x => x.BestScore));
            dto.LastAttemptedAt = allAttempts.Max(a => a.AttemptedAt);

            // ===== Build per-course chart data =====
            foreach (var best in bestPerQuiz)
            {
                var quiz = quizRepo.Get(best.QuizId);
                if (quiz == null) continue;

                var course = courseRepo.Get(quiz.CourseId);
                if (course == null) continue;

                dto.CourseScores.Add(new CourseScoreDTO
                {
                    CourseTitle = course.Title,
                    BestScore = best.BestScore,
                    Passed = best.EverPassed
                });
            }

            return dto;
        }
    }
}