using System.ComponentModel.DataAnnotations;

namespace CMCS.Web.Models
{
    public class AuditLog
    {
        [Key]
        public Guid LogId { get; set; }

        public Guid ClaimId { get; set; }
        public Claim Claim { get; set; }

        public Guid? UserId { get; set; }
        public User User { get; set; }

        public string Action { get; set; } // Submitted, Verified, Approved, Returned
        public DateTime ActionDate { get; set; }
    }
}
