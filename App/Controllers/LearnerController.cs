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

        // Parameter name "STitle" matches the form input name="STitle" — fixes B1
        public IActionResult BrowseCourses(string? STitle, string? difficulty, string? instructor)
        {
            // Service still takes the param as "title" — we just rename in this controller
            var courses = courseService.Search(STitle, difficulty, instructor);

            int learnerId = (int)HttpContext.Session.GetInt32("UserId")!;
            var myEnrollments = enrollmentService.GetByLearner(learnerId);
            var enrolledCourseIds = new HashSet<int>(myEnrollments.Select(e => e.CourseId));
            ViewBag.EnrolledCourseIds = enrolledCourseIds;

            // Pass filter values back via ViewBag so view re-fills the form correctly
            ViewBag.STitle = STitle;
            ViewBag.Difficulty = difficulty;
            ViewBag.Instructor = instructor;

            return View(courses);
        }
    }
}