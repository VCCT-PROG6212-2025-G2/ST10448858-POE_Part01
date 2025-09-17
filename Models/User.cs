using System.ComponentModel.DataAnnotations;
using System;

namespace CMCS.Web.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        // Role FK
        public int RoleId { get; set; }
        public UserRole Role { get; set; }

        // optional: link to Lecturer profile
        public Lecturer LecturerProfile { get; set; }
    }
}
