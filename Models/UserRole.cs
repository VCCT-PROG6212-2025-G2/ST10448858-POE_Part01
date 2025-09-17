using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CMCS.Web.Models
{
    public class UserRole
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        public string RoleName { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
