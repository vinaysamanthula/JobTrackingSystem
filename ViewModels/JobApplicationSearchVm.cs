using JobTrackingSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace JobTrackingSystem.ViewModels
{
    public class JobApplicationSearchVm
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;

        public int TotalCount { get; set; }
        public int TotalPages =>
           Math.Max(1, (int)Math.Ceiling((double)TotalCount / PageSize));
        public string? Keyword { get; set; }
        public string? Status { get; set; }
        public int? CompanyId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public IEnumerable<SelectListItem> Companies { get; set; } = new List<SelectListItem>();

        public List<JobApplication> Results { get; set; } = new();
        public string? SortBy { get; set; } = "date";
        public int TotalApplications { get; set; }
        public int AppliedCount { get; set; }
        public int InterviewCount { get; set; }
        public int OfferCount { get; set; }
        public int RejectedCount { get; set; }
    }
}