using System;
using System.Collections.Generic;
using System.Text;
using BLL.DTOs;
using System.ComponentModel.DataAnnotations;

namespace BLL.Validations
{
    public class PasswordMatch : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var obj = validationContext.ObjectInstance as RegistrationDTO;
            if (obj != null && obj.Password != null && obj.Password.Equals(value?.ToString()))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("Passwords don't match");
        }
    }
}
