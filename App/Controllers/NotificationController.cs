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

        public IActionResult Index()
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId")!;
            var list = notificationService.GetForUser(userId);
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkAsRead(int id)
        {
            notificationService.MarkAsRead(id);
            return RedirectToAction("Index");
        }

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