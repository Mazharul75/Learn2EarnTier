using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class ProfileDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2–100 characters")]
        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;
        public string UserTypeName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}