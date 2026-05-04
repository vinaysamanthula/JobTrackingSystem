
using JobTrackingSystem.Areas.Identity.Data; // make sure this is addednamespace JobTrackingSystem.Models;
namespace JobTrackingSystem.Models;

public class JobApplication
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public JobTrackingSystemUser? User { get; set; }

    public int CompanyId { get; set; }
    public Company? Company { get; set; }

    public string Role { get; set; }
    public string Status { get; set; }
    public DateTime DateApplied { get; set; }
    public string Notes { get; set; }
    public bool IsDeleted { get; set; } = false;
}
