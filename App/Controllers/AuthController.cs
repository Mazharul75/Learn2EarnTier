using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    public class AuthController : Controller
    {
        AuthService authService;

        public AuthController(AuthService authService)
        {
            this.authService = authService;
        }

        //Registration

        [HttpGet]
        public IActionResult Registration()
        {
            ViewBag.UserTypes = authService.GetUserTypes();
            return View(new RegistrationDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registration(RegistrationDTO obj)
        {
            if (ModelState.IsValid)
            {
                bool success = authService.Register(obj);
                if (success)
                {
                    TempData["Class"] = "success";
                    TempData["Msg"] = "Registration successful. Please log in.";
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError("Email", "Registration failed — email may already exist.");
            }
            ViewBag.UserTypes = authService.GetUserTypes();
            return View(obj);
        }

        //Login

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginDTO obj)
        {
            if (ModelState.IsValid)
            {
                var user = authService.Login(obj);
                if (user != null)
                {
                    // Store identity in session
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    HttpContext.Session.SetInt32("UserTypeId", user.UserTypeId);
                    HttpContext.Session.SetString("UserName", user.Name);

                    // Role-based redirectio n
                    if (user.UserTypeId == 1)
                    {
                        return RedirectToAction("Dashboard", "Learner");
                    }
                    else if (user.UserTypeId == 2)
                    {
                        return RedirectToAction("Dashboard", "Instructor");
                    }
                    return RedirectToAction("Index", "Home");
                }
                TempData["Class"] = "danger";
                TempData["Msg"] = "Invalid email or password.";
            }
            return View(obj);
        }

        //Logout

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Class"] = "info";
            TempData["Msg"] = "You have been logged out.";
            return RedirectToAction("Login");
        }
    }
}