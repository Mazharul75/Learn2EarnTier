using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using BLL.DTOs;
using DAL.EF.Tables;
using DAL.Repos;

namespace BLL.Services
{
    public class TaskService
    {
        CourseTaskRepo taskRepo;
        TaskSubmissionRepo submissionRepo;
        UserRepo userRepo;
        CourseRepo courseRepo;
        EnrollmentRepo enrollmentRepo;
        NotificationService notificationService;
        Mapper mapper;

        public TaskService(CourseTaskRepo taskRepo,
                           TaskSubmissionRepo submissionRepo,
                           UserRepo userRepo,
                           CourseRepo courseRepo,
                           EnrollmentRepo enrollmentRepo,
                           NotificationService notificationService)
        {
            this.taskRepo = taskRepo;
            this.submissionRepo = submissionRepo;
            this.userRepo = userRepo;
            this.courseRepo = courseRepo;
            this.enrollmentRepo = enrollmentRepo;
            this.notificationService = notificationService;
            mapper = MapperConfig.GetMapper();
        }

        // ===== Instructor side =====

        public bool CreateTask(CourseTaskDTO dto)
        {
            var task = new CourseTask
            {
                CourseId = dto.CourseId,
                Title = dto.Title,
                Description = dto.Description,
                CreatedAt = DateTime.Now
            };
            bool saved = taskRepo.Create(task);
            if (!saved) return false;

            // Notify all enrolled learners about the new task
            var course = courseRepo.Get(dto.CourseId);
            if (course != null)
            {
                var enrollments = enrollmentRepo.GetByCourse(dto.CourseId);
                foreach (var e in enrollments)
                {
                    notificationService.Notify(
                        userId: e.LearnerId,
                        message: $"New task added: \"{task.Title}\" in {course.Title}",
                        link: "/Enrollment/MyCourses");
                }
            }
            return true;
        }

        public bool DeleteTask(int taskId) => taskRepo.Delete(taskId);

        public List<CourseTaskDTO> GetTasksForCourse(int courseId)
        {
            var tasks = taskRepo.GetByCourse(courseId);
            return mapper.Map<List<CourseTaskDTO>>(tasks);
        }

        public List<TaskSubmissionDTO> GetPendingSubmissions(int courseId)
        {
            var submissions = submissionRepo.GetPendingByCourse(courseId);
            FillNavigations(submissions);
            return mapper.Map<List<TaskSubmissionDTO>>(submissions);
        }

        public bool ReviewSubmission(TaskReviewDTO dto)
        {
            var sub = submissionRepo.Get(dto.SubmissionId);
            if (sub == null) return false;

            sub.Status = dto.Status;
            sub.InstructorFeedback = dto.Feedback;
            bool saved = submissionRepo.Update(sub);

            if (saved)
            {
                // Notify learner of the review result
                var emoji = dto.Status == "Approved" ? "✓" : "✗";
                notificationService.Notify(
                    userId: sub.LearnerId,
                    message: $"{emoji} Your submission was {dto.Status}.",
                    link: "/Enrollment/MyCourses");
            }
            return saved;
        }

        // ===== Learner side =====

        public bool SubmitTask(int learnerId, int taskId, string link)
        {
            var task = taskRepo.Get(taskId);
            if (task == null) return false;

            // Must be enrolled in the course
            if (!enrollmentRepo.Exists(learnerId, task.CourseId)) return false;

            // Check for existing submission (one per learner per task)
            var existing = submissionRepo.GetByLearnerAndTask(learnerId, taskId);
            if (existing != null)
            {
                // Allow re-submission only if rejected
                if (existing.Status == "Rejected")
                {
                    existing.SubmissionLink = link;
                    existing.Status = "Pending";
                    existing.InstructorFeedback = null;
                    existing.SubmittedAt = DateTime.Now;
                    return submissionRepo.Update(existing);
                }
                return false;  // already submitted, can't resubmit
            }

            var sub = new TaskSubmission
            {
                TaskId = taskId,
                LearnerId = learnerId,
                SubmissionLink = link,
                Status = "Pending",
                SubmittedAt = DateTime.Now
            };
            bool saved = submissionRepo.Create(sub);

            if (saved)
            {
                // Notify instructor
                var course = courseRepo.Get(task.CourseId);
                if (course != null)
                {
                    var learner = userRepo.Get(learnerId);
                    notificationService.Notify(
                        userId: course.InstructorId,
                        message: $"{learner?.Name ?? "A learner"} submitted task \"{task.Title}\"",
                        link: $"/Task/Review/{task.CourseId}");
                }
            }
            return saved;
        }

        public List<TaskSubmissionDTO> GetSubmissionsForLearner(int learnerId, int courseId)
        {
            var submissions = submissionRepo.GetByLearnerAndCourse(learnerId, courseId);
            FillNavigations(submissions);
            return mapper.Map<List<TaskSubmissionDTO>>(submissions);
        }

        void FillNavigations(List<TaskSubmission> submissions)
        {
            foreach (var s in submissions)
            {
                if (s.Learner == null)
                    s.Learner = userRepo.Get(s.LearnerId);
                if (s.Task == null)
                    s.Task = taskRepo.Get(s.TaskId);
            }
        }
    }
}