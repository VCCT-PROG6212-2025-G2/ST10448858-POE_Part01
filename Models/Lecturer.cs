using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System;
using System.Collections.Generic;

namespace CMCS.Web.Models
{
    public class Lecturer
    {
        [Key]
        public Guid LecturerId { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal HourlyRate { get; set; }

        public ICollection<Claim> Claims { get; set; }
    }
}
