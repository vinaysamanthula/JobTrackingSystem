
using JobTrackingSystem.Models;
namespace JobTrackingSystem.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(JobTrackingSystemContext context)
        {
            if (context.JobApplications.Any())
                return; // already seeded

            var userId = context.Users.First().Id;

            var random = new Random();

            var statuses = new[] { "Applied", "Interview", "Offer", "Rejected" };
            var roles = new[]
            {
                "Software Engineer",
                "Backend Developer",
                "Frontend Developer",
                "Full Stack Developer",
                "QA Engineer"
            };

            var companies = context.Companies.ToList();

            var list = new List<JobApplication>();

            for (int i = 0; i < 30; i++)
            {
                list.Add(new JobApplication
                {
                    UserId = userId,
                    CompanyId = companies[random.Next(companies.Count)].Id,
                    Role = roles[random.Next(roles.Length)],
                    Status = statuses[random.Next(statuses.Length)],
                    DateApplied = DateTime.Now.AddDays(-random.Next(30)),
                    Notes = "Test data"
                });
            }

            context.JobApplications.AddRange(list);
            await context.SaveChangesAsync();
        }
    }
}
