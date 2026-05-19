using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using BLL.DTOs;
using DAL.EF.Tables;
using DAL.Repos;

namespace BLL.Services
{
    public class QuizService
    {
        QuizRepo quizRepo;
        QuestionRepo questionRepo;
        QuizAttemptRepo attemptRepo;
        CourseRepo courseRepo;
        EnrollmentRepo enrollmentRepo;
        NotificationService notificationService;
        Mapper mapper;

        public QuizService(QuizRepo quizRepo,
                           QuestionRepo questionRepo,
                           QuizAttemptRepo attemptRepo,
                           CourseRepo courseRepo,
                           EnrollmentRepo enrollmentRepo,
                           NotificationService notificationService)
        {
            this.quizRepo = quizRepo;
            this.questionRepo = questionRepo;
            this.attemptRepo = attemptRepo;
            this.courseRepo = courseRepo;
            this.enrollmentRepo = enrollmentRepo;
            this.notificationService = notificationService;
            mapper = MapperConfig.GetMapper();
        }
        public enum SubmitResult
        {
            Success,
            QuizNotFound,
            NotEnrolled,
            NoQuestions,
            AlreadyAttempted,
            DatabaseError
        }

        public QuizDTO GetOrCreateQuizForCourse(int courseId, int passMark = 50)
        {
            var quiz = quizRepo.GetByCourse(courseId);
            if (quiz == null)
            {
                quiz = new Quiz { CourseId = courseId, PassMark = passMark };
                quizRepo.Create(quiz);
            }

            return MapQuizWithQuestions(quiz);
        }

        public QuizDTO? GetForInstructor(int courseId)
        {
            var quiz = quizRepo.GetByCourse(courseId);
            if (quiz == null) return null;
            return MapQuizWithQuestions(quiz);
        }

        public List<QuestionForLearnerDTO>? GetForLearner(int courseId)
        {
            var quiz = quizRepo.GetByCourse(courseId);
            if (quiz == null) return null;

            var questions = questionRepo.GetByQuiz(quiz.Id);
            return mapper.Map<List<QuestionForLearnerDTO>>(questions);
        }

        public Quiz? GetQuizByCourse(int courseId)
        {
            return quizRepo.GetByCourse(courseId);
        }

        public bool AddQuestion(QuestionDTO dto)
        {
            var question = new Question
            {
                QuizId = dto.QuizId,
                QuestionText = dto.QuestionText,
                OptionA = dto.OptionA,
                OptionB = dto.OptionB,
                OptionC = dto.OptionC,
                OptionD = dto.OptionD,
                CorrectOption = dto.CorrectOption
            };
            return questionRepo.Create(question);
        }
        public bool UpdateQuestion(QuestionDTO dto)
        {
            var existing = questionRepo.Get(dto.Id);
            if (existing == null) return false;

            existing.QuestionText = dto.QuestionText;
            existing.OptionA = dto.OptionA;
            existing.OptionB = dto.OptionB;
            existing.OptionC = dto.OptionC;
            existing.OptionD = dto.OptionD;
            existing.CorrectOption = dto.CorrectOption;

            return questionRepo.Update(existing);
        }

        public bool DeleteQuestion(int questionId)
        {
            return questionRepo.Delete(questionId);
        }

        public QuestionDTO? GetQuestion(int questionId)
        {
            var q = questionRepo.Get(questionId);
            if (q == null) return null;
            return mapper.Map<QuestionDTO>(q);
        }

        //Auto_grading
        public (SubmitResult result, QuizResultDTO? data) Submit(QuizSubmissionDTO submission, int learnerId)
        {
            var quiz = quizRepo.Get(submission.QuizId);
            if (quiz == null) return (SubmitResult.QuizNotFound, null);

            var course = courseRepo.Get(quiz.CourseId);
            if (course == null) return (SubmitResult.QuizNotFound, null);

            if (!enrollmentRepo.Exists(learnerId, course.Id))
                return (SubmitResult.NotEnrolled, null);

            var priorAttempts = attemptRepo.GetByLearnerAndQuiz(learnerId, quiz.Id);
            if (priorAttempts.Any()) return (SubmitResult.AlreadyAttempted, null);

            var questions = questionRepo.GetByQuiz(quiz.Id);
            if (questions.Count == 0) return (SubmitResult.NoQuestions, null);

            int correctCount = 0;
            var breakdown = new List<QuestionResultDTO>();

            foreach (var q in questions)
            {
                submission.Answers.TryGetValue(q.Id, out string? picked);

                bool isCorrect = picked != null
                              && string.Equals(picked, q.CorrectOption, StringComparison.OrdinalIgnoreCase);

                if (isCorrect) correctCount++;

                breakdown.Add(new QuestionResultDTO
                {
                    QuestionId = q.Id,
                    QuestionText = q.QuestionText,
                    OptionA = q.OptionA,
                    OptionB = q.OptionB,
                    OptionC = q.OptionC,
                    OptionD = q.OptionD,
                    CorrectOption = q.CorrectOption,
                    PickedOption = picked,
                    IsCorrect = isCorrect
                });
            }

            int score = (int)Math.Round(100.0 * correctCount / questions.Count);
            bool passed = score >= quiz.PassMark;

            var attempt = new QuizAttempt
            {
                LearnerId = learnerId,
                QuizId = quiz.Id,
                Score = score,
                Passed = passed,
                AttemptedAt = DateTime.Now
            };

            bool saved = attemptRepo.Create(attempt);
            if (!saved) return (SubmitResult.DatabaseError, null);

            if (passed)
            {
                notificationService.Notify(
                    userId: learnerId,
                    message: $"You passed the quiz for \"{course.Title}\" with {score}%!",
                    link: "/Enrollment/MyCourses");
            }

            var result = new QuizResultDTO
            {
                AttemptId = attempt.Id,
                QuizId = quiz.Id,
                CourseId = course.Id,
                CourseTitle = course.Title,
                Score = score,
                PassMark = quiz.PassMark,
                Passed = passed,
                TotalQuestions = questions.Count,
                CorrectCount = correctCount,
                AttemptedAt = attempt.AttemptedAt,
                Breakdown = breakdown
            };

            return (SubmitResult.Success, result);
        }

        QuizDTO MapQuizWithQuestions(Quiz quiz)
        {
            if (quiz.Course == null)
                quiz.Course = courseRepo.Get(quiz.CourseId);

            if (quiz.Questions == null || quiz.Questions.Count == 0)
            {
                var questions = questionRepo.GetByQuiz(quiz.Id);
                quiz.Questions = questions;
            }

            return mapper.Map<QuizDTO>(quiz);
        }
    }
}