using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.ViewComponents
{
    public class NotificationBellViewComponent : ViewComponent
    {
        NotificationService notificationService;

        public NotificationBellViewComponent(NotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        public IViewComponentResult Invoke()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Content("");
            }

            int unreadCount = notificationService.GetUnreadCount(userId.Value);
            return View(unreadCount);
        }
    }
}