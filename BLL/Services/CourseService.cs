using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using BLL.DTOs;
using DAL.EF.Tables;
using DAL.Repos;

namespace BLL.Services
{
    public class CourseService
    {
        CourseRepo courseRepo;
        UserRepo userRepo;
        NotificationService notificationService;
        Mapper mapper;

        public CourseService(CourseRepo courseRepo, UserRepo userRepo, NotificationService notificationService)
        {
            this.courseRepo = courseRepo;
            this.userRepo = userRepo;
            this.notificationService = notificationService;
            mapper = MapperConfig.GetMapper();
        }

        // List ALL courses (used by Phase E learner browse)
        public List<CourseDTO> Get()
        {
            var courses = courseRepo.Get();
            FillNavigations(courses);
            return mapper.Map<List<CourseDTO>>(courses);
        }

        // List courses owned by a specific instructor (used by Index)
        public List<CourseDTO> GetByInstructor(int instructorId)
        {
            var courses = courseRepo.GetByInstructor(instructorId);
            FillNavigations(courses);
            return mapper.Map<List<CourseDTO>>(courses);
        }

        public List<CourseDTO> Search(string? title, string? difficulty, string? instructor)
        {
            // Start with every course
            var courses = courseRepo.Get();

            // Fill instructor data so we can filter by instructor name and populate DTO field
            FillNavigations(courses);

            // Apply each filter only if a value was provided
            if (!string.IsNullOrWhiteSpace(title))
            {
                courses = courses
                    .Where(c => c.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(difficulty))
            {
                courses = courses
                    .Where(c => c.Difficulty == difficulty)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(instructor))
            {
                courses = courses
                    .Where(c => c.Instructor != null &&
                                c.Instructor.Name.Contains(instructor, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return mapper.Map<List<CourseDTO>>(courses);
        }

        // Get a single course by id
        public CourseDTO? Get(int id)
        {
            var course = courseRepo.Get(id);
            if (course == null) return null;
            FillNavigations(new List<Course> { course });
            return mapper.Map<CourseDTO>(course);
        }

        // Create a new course owned by the given instructor
        public bool Create(CourseDTO dto, int instructorId)
        {
            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                Difficulty = dto.Difficulty,
                ContentLink = dto.ContentLink,
                MaxCapacity = dto.MaxCapacity == 0 ? null : dto.MaxCapacity,
                PrerequisiteId = dto.PrerequisiteId,
                InstructorId = instructorId,
                CreatedAt = DateTime.Now
            };
            bool saved = courseRepo.Create(course);
            if (!saved) return false;

            //  Notify all Learners about the new course
            notificationService.NotifyByRole(
                userTypeId: 1,  // 1 = Learner
                message: $"New course available: \"{course.Title}\" ({course.Difficulty})",
                link: "/Learner/BrowseCourses");

            return true;
        }

        // Update — but only if the caller owns the course
        public bool Update(CourseDTO dto, int requestingInstructorId)
        {
            var existing = courseRepo.Get(dto.Id);
            if (existing == null) return false;
            if (existing.InstructorId != requestingInstructorId) return false; // ownership check

            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.Difficulty = dto.Difficulty;
            existing.ContentLink = dto.ContentLink;
            existing.MaxCapacity = dto.MaxCapacity == 0 ? null : dto.MaxCapacity;
            existing.PrerequisiteId = dto.PrerequisiteId;

            return courseRepo.Update(existing);
        }

        // Delete — but only if the caller owns the course
        public bool Delete(int id, int requestingInstructorId)
        {
            var existing = courseRepo.Get(id);
            if (existing == null) return false;
            if (existing.InstructorId != requestingInstructorId) return false; // ownership check

            return courseRepo.Delete(id);
        }

        // Check if a course is owned by a given instructor (used by Edit/Delete GET)
        public bool IsOwnedBy(int courseId, int instructorId)
        {
            var c = courseRepo.Get(courseId);
            return c != null && c.InstructorId == instructorId;
        }

        // Helper: populate the Instructor navigation property by hand
        // (since CourseRepo doesn't Include() it)
        void FillNavigations(List<Course> courses)
        {
            foreach (var c in courses)
            {
                if (c.Instructor == null)
                    c.Instructor = userRepo.Get(c.InstructorId);

                // Fill prerequisite if set
                if (c.PrerequisiteId.HasValue && c.Prerequisite == null)
                    c.Prerequisite = courseRepo.Get(c.PrerequisiteId.Value);
            }
        }
    }
}
