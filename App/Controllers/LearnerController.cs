using App.AuthFilters;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [LearnerOnly]
    public class LearnerController : Controller
    {
        CourseService courseService;

        public LearnerController(CourseService courseService)
        {
            this.courseService = courseService;
        }

        public IActionResult Dashboard()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }

        public IActionResult BrowseCourses(string? title, string? difficulty, string? instructor)
        {
            var courses = courseService.Search(title, difficulty, instructor);

            // Pass filter values back to the view so the form stays filled in
            ViewBag.Title = title;
            ViewBag.Difficulty = difficulty;
            ViewBag.Instructor = instructor;

            return View(courses);
        }
    }
}