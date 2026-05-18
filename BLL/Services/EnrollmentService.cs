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
        Mapper mapper;

        public EnrollmentService(EnrollmentRepo enrollmentRepo,
                                 CourseRepo courseRepo,
                                 UserRepo userRepo,
                                 NotificationService notificationService)
        {
            this.enrollmentRepo = enrollmentRepo;
            this.courseRepo = courseRepo;
            this.userRepo = userRepo;
            this.notificationService = notificationService;
            mapper = MapperConfig.GetMapper();
        }

        // ===== Result type for the Enroll method =====
        public enum EnrollResult
        {
            Success,
            CourseNotFound,
            AlreadyEnrolled,
            OwnCourse,
            DatabaseError
        }

        // ===== Enroll a learner in a course =====
        public EnrollResult Enroll(int learnerId, int courseId)
        {
            // Rule 1: Course must exist
            var course = courseRepo.Get(courseId);
            if (course == null) return EnrollResult.CourseNotFound;

            // Rule 2: Not your own course
            if (course.InstructorId == learnerId) return EnrollResult.OwnCourse;

            // Rule 3: Not already enrolled
            if (enrollmentRepo.Exists(learnerId, courseId)) return EnrollResult.AlreadyEnrolled;

            // All rules pass — create the enrollment
            var enrollment = new Enrollment
            {
                LearnerId = learnerId,
                CourseId = courseId,
                EnrolledAt = DateTime.Now
            };

            bool saved = enrollmentRepo.Create(enrollment);
            if (!saved) return EnrollResult.DatabaseError;

            // Notify the instructor that a learner enrolled
            var learner = userRepo.Get(learnerId);
            string learnerName = learner?.Name ?? "Someone";
            notificationService.Notify(
                userId: course.InstructorId,
                message: $"{learnerName} enrolled in your course \"{course.Title}\".",
                link: "/Course/Index");

            return EnrollResult.Success;
        }

        // ===== Check if a learner is already enrolled in a course =====
        public bool IsEnrolled(int learnerId, int courseId)
        {
            return enrollmentRepo.Exists(learnerId, courseId);
        }

        // ===== Get this learner's enrollments (with course details) =====
        public List<EnrollmentDTO> GetByLearner(int learnerId)
        {
            var enrollments = enrollmentRepo.GetByLearner(learnerId);
            FillCourseAndInstructor(enrollments);
            return mapper.Map<List<EnrollmentDTO>>(enrollments);
        }

        public List<EnrollmentDTO> GetByCourse(int courseId)
        {
            var enrollments = enrollmentRepo.GetByCourse(courseId);
            // Fill learner data
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

        // ===== Helper: populate Course and Course.Instructor navigation =====
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
    }
}