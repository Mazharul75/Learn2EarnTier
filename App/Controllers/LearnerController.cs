using App.AuthFilters;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [LearnerOnly]
    public class LearnerController : Controller
    {
        public IActionResult Dashboard()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }
    }
}