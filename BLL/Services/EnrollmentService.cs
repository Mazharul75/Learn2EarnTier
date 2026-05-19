using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using BLL.DTOs;
using DAL.EF.Tables;
using DAL.Repos;

namespace BLL.Services
{
    public class EnrollmentService
    {
        EnrollmentRepo enrollmentRepo;
        CourseRepo courseRepo;
        UserRepo userRepo;
        NotificationService notificationService;
        QuizAttemptRepo attemptRepo;  
        QuizRepo quizRepo;
        MaterialCompletionRepo materialCompletionRepo;
        Mapper mapper;

        public EnrollmentService(EnrollmentRepo enrollmentRepo,
                         CourseRepo courseRepo,
                         UserRepo userRepo,
                         NotificationService notificationService,
                         QuizAttemptRepo attemptRepo,   
                         QuizRepo quizRepo,
                         MaterialCompletionRepo materialCompletionRepo)
        {
            this.enrollmentRepo = enrollmentRepo;
            this.courseRepo = courseRepo;
            this.userRepo = userRepo;
            this.notificationService = notificationService;
            this.attemptRepo = attemptRepo;
            this.quizRepo = quizRepo;          
            this.materialCompletionRepo = materialCompletionRepo;
            mapper = MapperConfig.GetMapper();
        }

        public enum EnrollResult
        {
            Success,
            CourseNotFound,
            AlreadyEnrolled,
            OwnCourse,
            PrerequisiteNotMet,   
            CourseFull,
            DatabaseError
        }

        public EnrollResult Enroll(int learnerId, int courseId)
        {
            var course = courseRepo.Get(courseId);
            if (course == null) return EnrollResult.CourseNotFound;

            if (course.InstructorId == learnerId) return EnrollResult.OwnCourse;

            if (course.PrerequisiteId.HasValue)
            {
                if (!HasCompletedCourse(learnerId, course.PrerequisiteId.Value))
                    return EnrollResult.PrerequisiteNotMet;
            }

            if (enrollmentRepo.Exists(learnerId, courseId)) return EnrollResult.AlreadyEnrolled;

            if (course.MaxCapacity.HasValue && course.MaxCapacity.Value > 0)
            {
                int currentCount = enrollmentRepo.CountByCourse(courseId);
                if (currentCount >= course.MaxCapacity.Value)
                    return EnrollResult.CourseFull;
            }

            var enrollment = new Enrollment
            {
                LearnerId = learnerId,
                CourseId = courseId,
                EnrolledAt = DateTime.Now
            };

            bool saved = enrollmentRepo.Create(enrollment);
            if (!saved) return EnrollResult.DatabaseError;

            var learner = userRepo.Get(learnerId);
            string learnerName = learner?.Name ?? "Someone";
            notificationService.Notify(
                userId: course.InstructorId,
                message: $"{learnerName} enrolled in your course \"{course.Title}\".",
                link: "/Course/Index");

            return EnrollResult.Success;
        }

        public bool IsEnrolled(int learnerId, int courseId)
        {
            return enrollmentRepo.Exists(learnerId, courseId);
        }

        public List<EnrollmentDTO> GetByLearner(int learnerId)
        {
            var enrollments = enrollmentRepo.GetByLearner(learnerId);
            FillCourseAndInstructor(enrollments);
            return mapper.Map<List<EnrollmentDTO>>(enrollments);
        }

        public List<EnrollmentDTO> GetByCourse(int courseId)
        {
            var enrollments = enrollmentRepo.GetByCourse(courseId);
            foreach (var e in enrollments)
            {
                if (e.Learner == null)
                    e.Learner = userRepo.Get(e.LearnerId);
            }
            return enrollments.Select(e => new EnrollmentDTO
            {
                Id = e.Id,
                LearnerId = e.LearnerId,
                CourseId = e.CourseId,
                EnrolledAt = e.EnrolledAt,
                LearnerName = e.Learner?.Name,
                LearnerEmail = e.Learner?.Email
            }).ToList();
        }

        public bool MarkMaterialComplete(int learnerId, int courseId)
        {
            if (materialCompletionRepo.IsCompleted(learnerId, courseId))
                return true;

            var mc = new MaterialCompletion
            {
                LearnerId = learnerId,
                CourseId = courseId,
                CompletedAt = DateTime.Now
            };
            return materialCompletionRepo.Create(mc);
        }

        public bool IsMaterialCompleted(int learnerId, int courseId)
        {
            return materialCompletionRepo.IsCompleted(learnerId, courseId);
        }

        void FillCourseAndInstructor(List<Enrollment> enrollments)
        {
            foreach (var e in enrollments)
            {
                if (e.Course == null)
                {
                    e.Course = courseRepo.Get(e.CourseId);
                }
                if (e.Course != null && e.Course.Instructor == null)
                {
                    e.Course.Instructor = userRepo.Get(e.Course.InstructorId);
                }
            }
        }

        public bool HasCompletedCourse(int learnerId, int courseId)
        {
            var quiz = quizRepo.GetByCourse(courseId);
            if (quiz == null) return false;

            var attempts = attemptRepo.GetByLearnerAndQuiz(learnerId, quiz.Id);
            return attempts.Any(a => a.Passed);
        }
    }
}