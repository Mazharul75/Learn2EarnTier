using BLL.Services;
using System.ComponentModel.DataAnnotations;

namespace BLL.Validations
{
    public class UniqueEmail : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success; // [Required] handles null; we don't double up

            var email = value.ToString();

            // Pull AuthService from DI
            var authService = validationContext.GetService(typeof(AuthService)) as AuthService;

            if (authService == null)
                return ValidationResult.Success; // service not available; skip

            if (authService.EmailExists(email!))
                return new ValidationResult("This email is already registered.");

            return ValidationResult.Success;
        }
    }
}