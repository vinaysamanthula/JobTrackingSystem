
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JobTrackingSystem.ViewModels
{
    public class JobApplicationCreateVm
    {
        [Required]
        public int CompanyId { get; set; }
        public IEnumerable<SelectListItem> Companies { get; set; } = [];

        [Required]
        public string Role { get; set; } = "";

        [Required]
        public string Status { get; set; } = "";

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateApplied { get; set; }

        public string? Notes { get; set; }
    }
}
