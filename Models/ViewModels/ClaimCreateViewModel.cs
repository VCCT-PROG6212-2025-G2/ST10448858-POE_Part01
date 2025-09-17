using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace CMCS.Web.Models.ViewModels
{
    public class ClaimCreateViewModel
    {
        public Guid LecturerId { get; set; }
        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }

        public List<ClaimItem> ClaimItems { get; set; } = new List<ClaimItem>();

        // file upload
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
    }
}
