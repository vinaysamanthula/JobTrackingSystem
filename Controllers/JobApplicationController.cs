    using JobTrackingSystem.Data;
    using JobTrackingSystem.Models;
    using JobTrackingSystem.Services;
    using JobTrackingSystem.ViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using System.Security.Claims;
    using ClosedXML.Excel;
    using System.IO;

[Authorize]
    public class JobApplicationController : Controller
    {
        private readonly IJobApplicationService _service;
        private readonly JobTrackingSystemContext _context;
        private readonly ILogger<JobApplicationController> _logger;

        public JobApplicationController(
            IJobApplicationService service,
            JobTrackingSystemContext context,
            ILogger<JobApplicationController> logger)
        {
            _service = service;
            _context = context;
            _logger = logger;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    //// 🔥 NEW: REQUEST INFO (IP + DEVICE)
    //private (string ip, string userAgent) GetRequestInfo()
    //{
    //    var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    //    var userAgent = Request.Headers["User-Agent"].ToString() ?? "Unknown";
    //    return (ip, userAgent);
    //}

    // ================= INDEX =================
    //public async Task<IActionResult> Index()
    //{
    //    var userId = GetUserId();
    //    var data = await _service.GetForUserAsync(userId);
    //    return View(data);
    public async Task<IActionResult> Index(JobApplicationSearchVm vm)
    {
        var userId = GetUserId();

        vm.Page = vm.Page <= 0 ? 1 : vm.Page;

        vm.Companies = await _context.Companies
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .ToListAsync();

        var result = await _service.SearchAsync(userId, vm);

        vm.Results = result.Item1;
        vm.TotalCount = result.Item2;

        vm.TotalApplications = await _context.JobApplications
            .CountAsync(x => x.UserId == userId && !x.IsDeleted);

        vm.AppliedCount = await _context.JobApplications
            .CountAsync(x => x.UserId == userId &&
                             x.Status == "Applied" &&
                             !x.IsDeleted);

        vm.InterviewCount = await _context.JobApplications
            .CountAsync(x => x.UserId == userId &&
                             x.Status == "Interview" &&
                             !x.IsDeleted);

        vm.OfferCount = await _context.JobApplications
            .CountAsync(x => x.UserId == userId &&
                             x.Status == "Offer" &&
                             !x.IsDeleted);

        vm.RejectedCount = await _context.JobApplications
            .CountAsync(x => x.UserId == userId &&
                             x.Status == "Rejected" &&
                             !x.IsDeleted);

        return View(vm);
    }
    // ================= CREATE (GET) =================
    public IActionResult Create()
        {
            var vm = new JobApplicationCreateVm
            {
                Companies = _context.Companies
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
            };

            return View(vm);
        }

        // ================= CREATE (POST) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobApplicationCreateVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Companies = _context.Companies.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
                return View(vm);
            }

            var entity = new JobApplication
            {
                UserId = GetUserId(),
                CompanyId = vm.CompanyId,
                Role = vm.Role,
                Status = vm.Status,
                DateApplied = vm.DateApplied,
                Notes = vm.Notes
            };

            var (ip, userAgent) = GetRequestInfo();
            await _service.CreateAsync(entity, ip, userAgent);

            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT (GET) =================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetUserId();

            var entity = await _service.GetByIdForUserAsync(id, userId);
            if (entity == null) return NotFound();

            var vm = new JobApplicationEditVm
            {
                Id = entity.Id,
                CompanyId = entity.CompanyId,
                Role = entity.Role,
                Status = entity.Status,
                DateApplied = entity.DateApplied,
                Notes = entity.Notes,

                Companies = await _context.Companies
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToListAsync()
            };

            return View(vm);
        }

        // ================= EDIT (POST) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(JobApplicationEditVm model)
        {
            var userId = GetUserId();

            if (!ModelState.IsValid)
            {
                model.Companies = await _context.Companies
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToListAsync();

                return View(model);
            }

            var (ip, userAgent) = GetRequestInfo();
            await _service.UpdateAsync(model, userId, ip, userAgent);

            return RedirectToAction(nameof(Index));
        }

        // ================= SEARCH =================
        public async Task<IActionResult> Search(JobApplicationSearchVm vm)
        {
            var userId = GetUserId();

            vm.Companies = _context.Companies
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

            var result = await _service.SearchAsync(userId, vm);

            vm.Results = result.Item1;
            vm.TotalCount = result.Item2;

            return View(vm);
        }

        // ================= DELETE (GET) =================
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();

            var job = await _service.GetByIdForUserAsync(id, userId);
            if (job == null) return NotFound();

            return View(job);
        }

        // ================= DELETE (POST) =================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetUserId();
            var (ip, userAgent) = GetRequestInfo();

            await _service.DeleteAsync(id, userId, ip, userAgent);

            return RedirectToAction(nameof(Index));
        }

        // ================= RESTORE =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var userId = GetUserId();
            var (ip, userAgent) = GetRequestInfo();

            await _service.RestoreAsync(id, userId, ip, userAgent);

            return RedirectToAction(nameof(Index));
        }

        // ================= DELETED LIST =================
        public async Task<IActionResult> Deleted()
        {
            var userId = GetUserId();

            var data = await _context.JobApplications
                .IgnoreQueryFilters()
                .Where(x => x.UserId == userId && x.IsDeleted)
                .Include(x => x.Company)
                .ToListAsync();

            return View(data);
        }
        private (string ip, string userAgent) GetRequestInfo()
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            // Fix localhost IPv6
            if (ip == "::1")
                ip = "127.0.0.1";

            var userAgent = Request.Headers["User-Agent"].ToString();

            return (ip ?? "Unknown", userAgent ?? "Unknown");
        }
    public async Task<IActionResult> ExportExcel()
    {
        var userId = GetUserId();

        var jobs = await _context.JobApplications
            .Include(x => x.Company)
            .Where(x => x.UserId == userId && !x.IsDeleted)
            .ToListAsync();

        using var workbook = new XLWorkbook();

        var worksheet = workbook.Worksheets.Add("Applications");

        worksheet.Cell(1, 1).Value = "Company";
        worksheet.Cell(1, 2).Value = "Role";
        worksheet.Cell(1, 3).Value = "Status";
        worksheet.Cell(1, 4).Value = "Date Applied";
        worksheet.Cell(1, 5).Value = "Notes";

        int row = 2;

        foreach (var job in jobs)
        {
            worksheet.Cell(row, 1).Value = job.Company?.Name;
            worksheet.Cell(row, 2).Value = job.Role;
            worksheet.Cell(row, 3).Value = job.Status;
            worksheet.Cell(row, 4).Value = job.DateApplied.ToString("dd MMM yyyy");
            worksheet.Cell(row, 5).Value = job.Notes;

            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        var content = stream.ToArray();

        return File(
            content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "JobApplications.xlsx");
    }

}

