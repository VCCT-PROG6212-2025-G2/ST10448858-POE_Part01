using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using CMCS.Web.Data;
using CMCS.Web.Models;
using CMCS.Web.Models.ViewModels;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace CMCS.Web.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly BlobServiceClient _blobClient;
        private readonly string _containerName;

        public ClaimsController(AppDbContext db, BlobServiceClient blobClient, IConfiguration config)
        {
            _db = db;
            _blobClient = blobClient;
            _containerName = config.GetSection("AzureBlobStorage")["ContainerName"] ?? "supportingdocs";
        }

        // GET: /Claims
        public async Task<IActionResult> Index()
        {
            var claim = await _db.Claims
                .Include(c => c.Lecturer).ThenInclude(l => l.User)
                .OrderByDescending(c => c.SubmittedDate)
                .ToListAsync();
            return View(claim);
        }

        // GET: /Claims/Create
        public IActionResult Create()
        {
            ViewBag.Lecturers = _db.Lecturers.Include(l => l.User).Select(l => new { l.LecturerId, Name = l.User.FullName }).ToList();
            return View(new ClaimCreateViewModel());
        }

        // POST: /Claims/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClaimCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Lecturers = _db.Lecturers.Include(l => l.User).Select(l => new { l.LecturerId, Name = l.User.FullName }).ToList();
                return View(vm);
            }

            // compute amounts for claim items
            foreach (var item in vm.ClaimItems)
            {
                item.ClaimItemId = Guid.NewGuid();
                item.Amount = Math.Round(item.HoursWorked * item.HourlyRate, 2);
            }

            var claim = new Claim
            {
                ClaimId = Guid.NewGuid(),
                LecturerId = vm.LecturerId,
                PeriodMonth = vm.PeriodMonth,
                PeriodYear = vm.PeriodYear,
                TotalAmount = vm.ClaimItems.Sum(i => i.Amount),
                Status = "Submitted",
                SubmittedDate = DateTime.UtcNow
            };

            await using var txn = await _db.Database.BeginTransactionAsync();
            try
            {
                await _db.Claims.AddAsync(claim);
                await _db.SaveChangesAsync();

                // attach claim items
                foreach (var item in vm.ClaimItems)
                {
                    item.ClaimId = claim.ClaimId;
                    await _db.ClaimItems.AddAsync(item);
                }
                await _db.SaveChangesAsync();

                // upload supporting documents to blob storage and save metadata
                if (vm.Files != null && vm.Files.Any())
                {
                    var container = _blobClient.GetBlobContainerClient(_containerName);
                    await container.CreateIfNotExistsAsync(PublicAccessType.None);

                    foreach (var file in vm.Files)
                    {
                        var blobName = $"{claim.ClaimId}/{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                        var blobClient = container.GetBlobClient(blobName);
                        using (var stream = file.OpenReadStream())
                        {
                            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
                        }

                        var doc = new SupportingDocument
                        {
                            DocumentId = Guid.NewGuid(),
                            ClaimId = claim.ClaimId,
                            FileName = file.FileName,
                            FileType = file.ContentType,
                            BlobUrl = blobClient.Uri.ToString(),
                            UploadedDate = DateTime.UtcNow
                        };
                        await _db.SupportingDocuments.AddAsync(doc);
                    }
                    await _db.SaveChangesAsync();
                }

                // audit log
                var log = new AuditLog
                {
                    LogId = Guid.NewGuid(),
                    ClaimId = claim.ClaimId,
                    UserId = null, // replace with current user id when auth in place
                    Action = "Submitted",
                    ActionDate = DateTime.UtcNow
                };
                await _db.AuditLogs.AddAsync(log);
                await _db.SaveChangesAsync();

                await txn.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await txn.RollbackAsync();
                ModelState.AddModelError("", "Error saving claim: " + ex.Message);
                ViewBag.Lecturers = _db.Lecturers.Include(l => l.User).Select(l => new { l.LecturerId, Name = l.User.FullName }).ToList();
                return View(vm);
            }
        }

        // GET: /Claims/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var claim = await _db.Claims
                .Include(c => c.ClaimItems)
                .Include(c => c.SupportingDocuments)
                .Include(c => c.AuditLogs)
                .Include(c => c.Lecturer).ThenInclude(l => l.User)
                .FirstOrDefaultAsync(c => c.ClaimId == id);
            if (claim == null) return NotFound();
            return View(claim);
        }

        // Additional actions (Edit, Delete, Verify, Approve) follow similar patterns
    }
}
