using App.AuthFilters;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [LearnerOnly]
    public class LearnerController : Controller
    {
        CourseService courseService;
        EnrollmentService enrollmentService;
        ProgressService progressService;

        public LearnerController(CourseService courseService,
                                 EnrollmentService enrollmentService,
                                 ProgressService progressService)
        {
            this.courseService = courseService;
            this.enrollmentService = enrollmentService;
            this.progressService = progressService;
        }

        public IActionResult Dashboard()
        {
            int learnerId = (int)HttpContext.Session.GetInt32("UserId")!;
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            var stats = progressService.GetLearnerStats(learnerId);
            return View(stats);
        }

        public IActionResult BrowseCourses(string? title, string? difficulty, string? instructor)
        {
            var courses = courseService.Search(title, difficulty, instructor);

            int learnerId = (int)HttpContext.Session.GetInt32("UserId")!;
            var myEnrollments = enrollmentService.GetByLearner(learnerId);
            var enrolledCourseIds = new HashSet<int>(myEnrollments.Select(e => e.CourseId));
            ViewBag.EnrolledCourseIds = enrolledCourseIds;

            ViewBag.Title = title;
            ViewBag.Difficulty = difficulty;
            ViewBag.Instructor = instructor;

            return View(courses);
        }
    }
}