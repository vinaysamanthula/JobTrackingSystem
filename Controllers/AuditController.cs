using JobTrackingSystem.Data;
using JobTrackingSystem.ViewModels;
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
                .Join(_context.Users,
                    log => log.UserId,
                    user => user.Id,
                    (log, user) => new AuditLogVm
                    {
                        Id = log.Id,
                        UserEmail = user.Email,
                        Action = log.Action,
                        EntityName = log.EntityName,
                        EntityId = log.EntityId,
                        Timestamp = log.Timestamp,
                        OldValues = log.OldValues,
                        NewValues = log.NewValues,
                        IpAddress = log.IpAddress,
                        UserAgent = log.UserAgent
                    })
                .OrderByDescending(x => x.Timestamp);

            var total = await query.CountAsync();

            var logs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 🔥 Company mapping (clean version)
            foreach (var log in logs)
            {
                if (string.IsNullOrEmpty(log.NewValues))
                    continue;

                try
                {
                    var data = System.Text.Json.JsonSerializer
                        .Deserialize<Dictionary<string, object>>(log.NewValues);

                    if (data != null && data.TryGetValue("CompanyId", out var value))
                    {
                        if (int.TryParse(value?.ToString(), out int companyId))
                        {
                            var company = await _context.Companies
                                .FirstOrDefaultAsync(c => c.Id == companyId);

                            if (company != null)
                            {
                                data["Company"] = company.Name;
                                data.Remove("CompanyId");

                                log.NewValues = System.Text.Json.JsonSerializer.Serialize(data);
                            }
                        }
                    }
                }
                catch
                {
                    // ignore bad json
                }
            }

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);

            return View(logs);
        }
    }
}
    