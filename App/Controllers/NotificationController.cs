using App.AuthFilters;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [Logged]
    public class NotificationController : Controller
    {
        NotificationService notificationService;

        public NotificationController(NotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        // GET /Notification
        public IActionResult Index()
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId")!;
            var list = notificationService.GetForUser(userId);
            return View(list);
        }

        // POST /Notification/MarkAsRead/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkAsRead(int id)
        {
            notificationService.MarkAsRead(id);
            return RedirectToAction("Index");
        }

        // POST /Notification/MarkAllAsRead
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkAllAsRead()
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId")!;
            notificationService.MarkAllAsRead(userId);
            return RedirectToAction("Index");
        }
    }
}