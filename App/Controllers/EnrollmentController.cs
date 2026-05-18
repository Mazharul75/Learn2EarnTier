using App.AuthFilters;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [LearnerOnly]
    public class EnrollmentController : Controller
    {
        EnrollmentService enrollmentService;

        public EnrollmentController(EnrollmentService enrollmentService)
        {
            this.enrollmentService = enrollmentService;
        }

        // ===== POST /Enrollment/Enroll  with form field CourseId =====
        [HttpPost]
        public IActionResult Enroll(int courseId)
        {
            int learnerId = (int)HttpContext.Session.GetInt32("UserId")!;

            var result = enrollmentService.Enroll(learnerId, courseId);

            switch (result)
            {
                case EnrollmentService.EnrollResult.Success:
                    TempData["Class"] = "success";
                    TempData["Msg"] = "You've enrolled in this course!";
                    return RedirectToAction("MyCourses");

                case EnrollmentService.EnrollResult.AlreadyEnrolled:
                    TempData["Class"] = "warning";
                    TempData["Msg"] = "You're already enrolled in this course.";
                    break;

                case EnrollmentService.EnrollResult.OwnCourse:
                    TempData["Class"] = "danger";
                    TempData["Msg"] = "You can't enroll in your own course.";
                    break;

                case EnrollmentService.EnrollResult.CourseNotFound:
                    TempData["Class"] = "danger";
                    TempData["Msg"] = "That course no longer exists.";
                    break;

                case EnrollmentService.EnrollResult.CourseFull:
                    TempData["Class"] = "danger";
                    TempData["Msg"] = "Sorry, this course is full.";
                    break;

                case EnrollmentService.EnrollResult.PrerequisiteNotMet:
                    TempData["Class"] = "warning";
                    TempData["Msg"] = "You must pass the prerequisite course's quiz before enrolling here.";
                    break;

                default:
                    TempData["Class"] = "danger";
                    TempData["Msg"] = "Something went wrong. Please try again.";
                    break;
            }

            return RedirectToAction("BrowseCourses", "Learner");
        }

        // ===== GET /Enrollment/MyCourses =====
        public IActionResult MyCourses()
        {
            int learnerId = (int)HttpContext.Session.GetInt32("UserId")!;
            var enrollments = enrollmentService.GetByLearner(learnerId);
            return View(enrollments);
        }
    }
}