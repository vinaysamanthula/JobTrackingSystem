using JobTrackingSystem.Areas.Identity.Data;
using JobTrackingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobTrackingSystem.Data;

public class JobTrackingSystemContext : IdentityDbContext<JobTrackingSystemUser>
{
    public JobTrackingSystemContext(DbContextOptions<JobTrackingSystemContext> options)
        : base(options)
    {
    }
    public DbSet<Company> Companies { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // keep this FIRST (important for Identity)

        // 🔥 ADD THIS LINE
        builder.Entity<JobApplication>()
            .HasQueryFilter(x => !x.IsDeleted);


        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
