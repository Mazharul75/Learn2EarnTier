using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace App.AuthFilters
{
    public class InstructorOnly : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("UserId");
            var userTypeId = context.HttpContext.Session.GetInt32("UserTypeId");
            if (userId == null || userTypeId != 2) // 2 = Instruc
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }
        }
    }
}