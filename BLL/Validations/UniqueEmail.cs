using BLL.Services;
using System.ComponentModel.DataAnnotations;

namespace BLL.Validations
{
    public class UniqueEmail : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var email = value.ToString();

            var authService = validationContext.GetService(typeof(AuthService)) as AuthService;

            if (authService == null)
                return ValidationResult.Success;

            if (authService.EmailExists(email!))
                return new ValidationResult("This email is already registered.");

            return ValidationResult.Success;
        }
    }
}