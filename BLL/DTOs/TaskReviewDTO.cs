using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class TaskReviewDTO
    {
        public int SubmissionId { get; set; }

        [Required]
        [RegularExpression("^(Approved|Rejected)$", ErrorMessage = "Status must be Approved or Rejected")]
        public string Status { get; set; } = null!;

        [StringLength(2000)]
        public string? Feedback { get; set; }
    }
}