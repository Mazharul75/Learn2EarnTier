using App.AuthFilters;
using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    // Note: no class-level filter — we mix Instructor & Learner actions in this controller
    public class QuizController : Controller
    {
        QuizService quizService;
        CourseService courseService;

        public QuizController(QuizService quizService, CourseService courseService)
        {
            this.quizService = quizService;
            this.courseService = courseService;
        }

        // ============================================================
        // ===== INSTRUCTOR SIDE =====
        // ============================================================

        // GET /Quiz/Manage/5  (5 = courseId)
        [InstructorOnly]
        public IActionResult Manage(int id)
        {
            int instructorId = (int)HttpContext.Session.GetInt32("UserId")!;

            // Must own the course
            if (!courseService.IsOwnedBy(id, instructorId))
            {
                TempData["Class"] = "danger";
                TempData["Msg"] = "You don't own this course.";
                return RedirectToAction("Index", "Course");
            }

            var quiz = quizService.GetOrCreateQuizForCourse(id);
            return View(quiz);
        }

        // POST: add a question to a quiz
        [HttpPost]
        [InstructorOnly]
        [ValidateAntiForgeryToken]
        public IActionResult AddQuestion(QuestionDTO dto, int courseId)
        {
            int instructorId = (int)HttpContext.Session.GetInt32("UserId")!;

            if (!courseService.IsOwnedBy(courseId, instructorId))
            {
                TempData["Class"] = "danger";
                TempData["Msg"] = "You don't own this course.";
                return RedirectToAction("Index", "Course");
            }

            if (ModelState.IsValid)
            {
                bool success = quizService.AddQuestion(dto);
                if (success)
                {
                    TempData["Class"] = "success";
                    TempData["Msg"] = "Question added.";
                }
                else
                {
                    TempData["Class"] = "danger";
                    TempData["Msg"] = "Failed to add question.";
                }
            }
            else
            {
                TempData["Class"] = "danger";
                TempData["Msg"] = "Question data was invalid.";
            }

            return RedirectToAction("Manage", new { id = courseId });
        }

        // GET: edit a question
        [InstructorOnly]
        public IActionResult EditQuestion(int id, int courseId)
        {
            int instructorId = (int)HttpContext.Session.GetInt32("UserId")!;
            if (!courseService.IsOwnedBy(courseId, instructorId))
            {
                return RedirectToAction("Index", "Course");
            }

            var question = quizService.GetQuestion(id);
            if (question == null) return RedirectToAction("Manage", new { id = courseId });

            ViewBag.CourseId = courseId;
            return View(question);
        }

        // POST: save edited question
        [HttpPost]
        [InstructorOnly]
        [ValidateAntiForgeryToken]
        public IActionResult EditQuestion(QuestionDTO dto, int courseId)
        {
            int instructorId = (int)HttpContext.Session.GetInt32("UserId")!;
            if (!courseService.IsOwnedBy(courseId, instructorId))
            {
                return RedirectToAction("Index", "Course");
            }

            if (ModelState.IsValid)
            {
                bool success = quizService.UpdateQuestion(dto);
                if (success)
                {
                    TempData["Class"] = "success";
                    TempData["Msg"] = "Question updated.";
                    return RedirectToAction("Manage", new { id = courseId });
                }
                ModelState.AddModelError("", "Failed to update.");
            }

            ViewBag.CourseId = courseId;
            return View(dto);
        }

        // POST: delete a question
        [HttpPost]
        [InstructorOnly]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteQuestion(int id, int courseId)
        {
            int instructorId = (int)HttpContext.Session.GetInt32("UserId")!;
            if (!courseService.IsOwnedBy(courseId, instructorId))
            {
                return RedirectToAction("Index", "Course");
            }

            quizService.DeleteQuestion(id);
            TempData["Class"] = "success";
            TempData["Msg"] = "Question deleted.";
            return RedirectToAction("Manage", new { id = courseId });
        }

        // ============================================================
        // ===== LEARNER SIDE =====
        // ============================================================

        // GET /Quiz/Take/5  (5 = courseId)
        [LearnerOnly]
        public IActionResult Take(int id)
        {
            var questions = quizService.GetForLearner(id);
            if (questions == null || questions.Count == 0)
            {
                TempData["Class"] = "warning";
                TempData["Msg"] = "This course doesn't have a quiz yet.";
                return RedirectToAction("MyCourses", "Enrollment");
            }

            var quiz = quizService.GetQuizByCourse(id);
            ViewBag.QuizId = quiz!.Id;
            ViewBag.CourseId = id;
            ViewBag.PassMark = quiz.PassMark;
            return View(questions);
        }

        // POST /Quiz/Submit
        [HttpPost]
        [LearnerOnly]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(int quizId, Dictionary<int, string> Answers)
        {
            int learnerId = (int)HttpContext.Session.GetInt32("UserId")!;

            var submission = new QuizSubmissionDTO
            {
                QuizId = quizId,
                Answers = Answers ?? new Dictionary<int, string>()
            };

            var (status, result) = quizService.Submit(submission, learnerId);

            switch (status)
            {
                case QuizService.SubmitResult.Success:
                    return View("Result", result);

                case QuizService.SubmitResult.NotEnrolled:
                    TempData["Class"] = "danger";
                    TempData["Msg"] = "You need to enroll in the course first.";
                    return RedirectToAction("BrowseCourses", "Learner");

                case QuizService.SubmitResult.AlreadyAttempted: 
                    TempData["Class"] = "warning";
                    TempData["Msg"] = "You have already attempted this quiz. Only one attempt is allowed.";
                    return RedirectToAction("MyCourses", "Enrollment");

                case QuizService.SubmitResult.NoQuestions:
                    TempData["Class"] = "warning";
                    TempData["Msg"] = "This quiz has no questions yet.";
                    return RedirectToAction("MyCourses", "Enrollment");

                default:
                    TempData["Class"] = "danger";
                    TempData["Msg"] = "Something went wrong.";
                    return RedirectToAction("MyCourses", "Enrollment");
            }
        }
    }
}