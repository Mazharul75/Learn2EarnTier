using App.AuthFilters;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [Logged]
    public class ProfileController : Controller
    {
        AuthService authService;

        public ProfileController(AuthService authService)
        {
            this.authService = authService;
        }

        public IActionResult Index()
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId")!;
            var profile = authService.GetProfile(userId);
            if (profile == null) return RedirectToAction("Login", "Auth");
            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string Name)
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId")!;

            if (string.IsNullOrWhiteSpace(Name) || Name.Length < 2)
            {
                TempData["Class"] = "danger";
                TempData["Msg"] = "Name must be at least 2 characters.";
                return RedirectToAction("Index");
            }

            bool success = authService.UpdateProfile(userId, Name.Trim());
            if (success)
            {
                // Also update the session so navbar updates
                HttpContext.Session.SetString("UserName", Name.Trim());
                TempData["Class"] = "success";
                TempData["Msg"] = "Profile updated.";
            }
            else
            {
                TempData["Class"] = "danger";
                TempData["Msg"] = "Update failed.";
            }
            return RedirectToAction("Index");
        }
    }
}