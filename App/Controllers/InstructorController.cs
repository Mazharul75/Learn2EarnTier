using App.AuthFilters;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [InstructorOnly]
    public class InstructorController : Controller
    {
        public IActionResult Dashboard()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }
    }
}