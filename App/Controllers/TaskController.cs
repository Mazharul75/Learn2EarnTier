using App.AuthFilters;
using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    public class TaskController : Controller
    {
        TaskService taskService;
        CourseService courseService;

        public TaskController(TaskService taskService, CourseService courseService)
        {
            this.taskService = taskService;
            this.courseService = courseService;
        }

        //INSTRUCTOR

        [InstructorOnly]
        public IActionResult Manage(int id)
        {
            int instructorId = (int)HttpContext.Session.GetInt32("UserId")!;
            if (!courseService.IsOwnedBy(id, instructorId))
            {
                TempData["Class"] = "danger";
                TempData["Msg"] = "You don't own this course.";
                return RedirectToAction("Index", "Course");
            }

            var course = courseService.Get(id);
            ViewBag.Course = course;
            ViewBag.Tasks = taskService.GetTasksForCourse(id);
            ViewBag.PendingSubmissions = taskService.GetPendingSubmissions(id);
            return View();
        }

        [HttpPost]
        [InstructorOnly]
        [ValidateAntiForgeryToken]
        public IActionResult AddTask(CourseTaskDTO dto)
        {
            int instructorId = (int)HttpContext.Session.GetInt32("UserId")!;
            if (!courseService.IsOwnedBy(dto.CourseId, instructorId))
            {
                TempData["Class"] = "danger";
                TempData["Msg"] = "Forbidden.";
                return RedirectToAction("Index", "Course");
            }

            if (ModelState.IsValid)
            {
                if (taskService.CreateTask(dto))
                {
                    TempData["Class"] = "success";
                    TempData["Msg"] = "Task added.";
                }
                else
                {
                    TempData["Class"] = "danger";
                    TempData["Msg"] = "Failed to add task.";
                }
            }
            else
            {
                TempData["Class"] = "danger";
                TempData["Msg"] = "Invalid task data.";
            }
            return RedirectToAction("Manage", new { id = dto.CourseId });
        }

        [HttpPost]
        [InstructorOnly]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTask(int id, int courseId)
        {
            int instructorId = (int)HttpContext.Session.GetInt32("UserId")!;
            if (!courseService.IsOwnedBy(courseId, instructorId))
                return RedirectToAction("Index", "Course");

            taskService.DeleteTask(id);
            TempData["Class"] = "success";
            TempData["Msg"] = "Task deleted.";
            return RedirectToAction("Manage", new { id = courseId });
        }

        [HttpPost]
        [InstructorOnly]
        [ValidateAntiForgeryToken]
        public IActionResult Review(TaskReviewDTO dto, int courseId)
        {
            int instructorId = (int)HttpContext.Session.GetInt32("UserId")!;
            if (!courseService.IsOwnedBy(courseId, instructorId))
                return RedirectToAction("Index", "Course");

            if (ModelState.IsValid && taskService.ReviewSubmission(dto))
            {
                TempData["Class"] = "success";
                TempData["Msg"] = $"Submission {dto.Status}.";
            }
            else
            {
                TempData["Class"] = "danger";
                TempData["Msg"] = "Review failed.";
            }
            return RedirectToAction("Manage", new { id = courseId });
        }

        //LEARNER

        [LearnerOnly]
        public IActionResult MyTasks(int id)
        {
            int learnerId = (int)HttpContext.Session.GetInt32("UserId")!;
            var course = courseService.Get(id);
            var tasks = taskService.GetTasksForCourse(id);
            var submissions = taskService.GetSubmissionsForLearner(learnerId, id);

            ViewBag.Course = course;
            ViewBag.Tasks = tasks;
            ViewBag.Submissions = submissions;
            return View();
        }

        [HttpPost]
        [LearnerOnly]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(int taskId, int courseId, string SubmissionLink)
        {
            int learnerId = (int)HttpContext.Session.GetInt32("UserId")!;
            if (string.IsNullOrWhiteSpace(SubmissionLink))
            {
                TempData["Class"] = "danger";
                TempData["Msg"] = "Submission link is required.";
                return RedirectToAction("MyTasks", new { id = courseId });
            }

            bool success = taskService.SubmitTask(learnerId, taskId, SubmissionLink.Trim());
            if (success)
            {
                TempData["Class"] = "success";
                TempData["Msg"] = "Task submitted!";
            }
            else
            {
                TempData["Class"] = "danger";
                TempData["Msg"] = "Submission failed (already submitted or not enrolled).";
            }
            return RedirectToAction("MyTasks", new { id = courseId });
        }
    }
}