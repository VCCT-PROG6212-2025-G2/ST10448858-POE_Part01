using System;
using System.ComponentModel.DataAnnotations;

namespace CMCS.Web.Models
{
    public class SupportingDocument
    {
        [Key]
        public Guid DocumentId { get; set; }

        public Guid ClaimId { get; set; }
        public Claim Claim { get; set; }

        public string FileName { get; set; }
        public string FileType { get; set; }

        // store blob URL (recommended) or file path
        public string BlobUrl { get; set; }

        public DateTime UploadedDate { get; set; }
    }
}
