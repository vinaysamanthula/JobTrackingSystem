using JobTrackingSystem.Data;
using JobTrackingSystem.Models;
using JobTrackingSystem.Services;
using JobTrackingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

    // 🔥 NEW: REQUEST INFO (IP + DEVICE)
    private (string ip, string userAgent) GetRequestInfo()
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var userAgent = Request.Headers["User-Agent"].ToString() ?? "Unknown";
        return (ip, userAgent);
    }

    // ================= INDEX =================
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        var data = await _service.GetForUserAsync(userId);
        return View(data);
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
}