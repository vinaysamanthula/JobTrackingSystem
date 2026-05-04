using JobTrackingSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobTrackingSystem.Controllers
{
    [Authorize] // later you restrict to admin
    public class AuditController : Controller
    {
        private readonly JobTrackingSystemContext _context;

        public AuditController(JobTrackingSystemContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 10;

            var query = _context.AuditLogs
                .OrderByDescending(x => x.Timestamp);

            var total = await query.CountAsync();

            var logs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);

            return View(logs);
        }
    }
}