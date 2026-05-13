using App.AuthFilters;
using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [InstructorOnly]
    public class CourseController : Controller
    {
        CourseService courseService;

        public CourseController(CourseService courseService)
        {
            this.courseService = courseService;
        }

        // ===== INDEX: list this instructor's courses =====
        public IActionResult Index()
        {
            int instructorId = (int)HttpContext.Session.GetInt32("UserId")!;
            var courses = courseService.GetByInstructor(instructorId);
            return View(courses);
        }

        // ===== CREATE =====
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CourseDTO());
        }

        [HttpPost]
        public IActionResult Create(CourseDTO dto)
        {
            if (ModelState.IsValid)
            {
                int instructorId = (int)HttpContext.Session.GetInt32("UserId")!;
                bool success = courseService.Create(dto, instructorId);
                if (success)
                {
                    TempData["Class"] = "success";
                    TempData["Msg"] = "Course created successfully.";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Failed to create course.");
            }
            return View(dto);
        }

        // ===== EDIT =====
        [HttpGet]
        public IActionResult Edit(int id)
        {
            int instructorId = (int)HttpContext.Session.GetInt32("UserId")!;
            if (!courseService.IsOwnedBy(id, instructorId))
            {
                TempData["Class"] = "danger";
                TempData["Msg"] = "You don't have permission to edit this course.";
                return RedirectToAction("Index");
            }

            var course = courseService.Get(id);
            if (course == null) return RedirectToAction("Index");

            return View(course);
        }

        [HttpPost]
        public IActionResult Edit(CourseDTO dto)
        {
            if (ModelState.IsValid)
            {
                int instructorId = (int)HttpContext.Session.GetInt32("UserId")!;
                bool success = courseService.Update(dto, instructorId);
                if (success)
                {
                    TempData["Class"] = "success";
                    TempData["Msg"] = "Course updated successfully.";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Failed to update course.");
            }
            return View(dto);
        }

        // ===== DELETE =====
        [HttpGet]
        public IActionResult Delete(int id)
        {
            int instructorId = (int)HttpContext.Session.GetInt32("UserId")!;
            if (!courseService.IsOwnedBy(id, instructorId))
            {
                TempData["Class"] = "danger";
                TempData["Msg"] = "You don't have permission to delete this course.";
                return RedirectToAction("Index");
            }

            var course = courseService.Get(id);
            if (course == null) return RedirectToAction("Index");

            return View(course);
        }

        [HttpPost]
        public IActionResult Delete(int id, string Decision)
        {
            if (Decision == "Yes")
            {
                int instructorId = (int)HttpContext.Session.GetInt32("UserId")!;
                bool success = courseService.Delete(id, instructorId);
                if (success)
                {
                    TempData["Class"] = "success";
                    TempData["Msg"] = "Course deleted.";
                }
                else
                {
                    TempData["Class"] = "danger";
                    TempData["Msg"] = "Failed to delete course.";
                }
            }
            return RedirectToAction("Index");
        }
    }
}