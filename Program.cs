using JobTrackingSystem.Areas.Identity.Data;
using JobTrackingSystem.Data;
using JobTrackingSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Connection string
var connectionString = builder.Configuration.GetConnectionString("JobTrackingSystemContextConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

// 2. DbContext
builder.Services.AddDbContext<JobTrackingSystemContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Identity
builder.Services.AddDefaultIdentity<JobTrackingSystemUser>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<JobTrackingSystemContext>();

// 4. MVC + Razor Pages (IMPORTANT)
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();   // 👈 YOU ARE PROBABLY MISSING THIS
builder.Services.AddScoped<IJobApplicationService, JobApplicationService>();
var app = builder.Build();


// ---------------- PIPELINE ----------------

// 5. Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔴 CRITICAL ORDER (don’t mess this up)
app.UseAuthentication();
app.UseAuthorization();

// 6. Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 👇 THIS ENABLES /Identity/Account/Register
app.MapRazorPages();

app.Run();