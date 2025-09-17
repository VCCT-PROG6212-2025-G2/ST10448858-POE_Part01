using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS.Web.Models
{
    public class Claim
    {
        [Key]
        public Guid ClaimId { get; set; }

        public Guid LecturerId { get; set; }
        public Lecturer Lecturer { get; set; }

        // monthly claim
        public int PeriodMonth { get; set; } // 1..12
        public int PeriodYear { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public string Status { get; set; } // Draft, Submitted, Verified, Returned, Approved

        public DateTime SubmittedDate { get; set; }

        public ICollection<ClaimItem> ClaimItems { get; set; }
        public ICollection<SupportingDocument> SupportingDocuments { get; set; }
        public ICollection<AuditLog> AuditLogs { get; set; }
    }
}
