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

        public LearnerController(CourseService courseService,
                                 EnrollmentService enrollmentService)
        {
            this.courseService = courseService;
            this.enrollmentService = enrollmentService;
        }

        public IActionResult Dashboard()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }

        public IActionResult BrowseCourses(string? title, string? difficulty, string? instructor)
        {
            var courses = courseService.Search(title, difficulty, instructor);

            // Compute set of course IDs this learner has already enrolled in
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