using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS.Web.Models
{
    public class ClaimItem
    {
        [Key]
        public Guid ClaimItemId { get; set; }

        public Guid ClaimId { get; set; }
        public Claim Claim { get; set; }

        [Required]
        public string Description { get; set; }

        [Column(TypeName = "decimal(9,2)")]
        public decimal HoursWorked { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal HourlyRate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
    }
}
