using JobTrackingSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobTrackingSystem.ViewModels;
namespace JobTrackingSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly JobTrackingSystemContext _context;

        public HomeController(JobTrackingSystemContext context)
        {
            _context = context;
        }

        // 🔥 THIS IS YOUR DASHBOARD
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var total = await _context.JobApplications
                .CountAsync(x => x.UserId == userId && !x.IsDeleted);

            var applied = await _context.JobApplications
                .CountAsync(x => x.UserId == userId && x.Status == "Applied");

            var interview = await _context.JobApplications
                .CountAsync(x => x.UserId == userId && x.Status == "Interview");

            var offer = await _context.JobApplications
                .CountAsync(x => x.UserId == userId && x.Status == "Offer");

            var rejected = await _context.JobApplications
                .CountAsync(x => x.UserId == userId && x.Status == "Rejected");

            var model = new DashboardVm
            {
                Total = total,
                Applied = applied,
                Interview = interview,
                Offer = offer,
                Rejected = rejected
            };
            model.RecentActivities = await _context.AuditLogs
         .Where(x => x.UserId == userId &&
                     !string.IsNullOrEmpty(x.Description))
         .OrderByDescending(x => x.Timestamp)
         .Take(5)
         .Select(x => new RecentActivityVm
         {
             Description = x.Description,
             Timestamp = x.Timestamp
         })
         .ToListAsync();    

            return View(model);
        }
    }
}