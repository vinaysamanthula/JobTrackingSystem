using JobTrackingSystem.Data;
using JobTrackingSystem.Models;
using Microsoft.EntityFrameworkCore;
namespace JobTrackingSystem.Services
{
    public class JobApplicationService : IJobApplicationService
    {

        private readonly JobTrackingSystemContext _context;

        public JobApplicationService(JobTrackingSystemContext context)
        {
            _context = context;
        }

        public async Task<List<JobApplication>> GetForUserAsync(string userId)
        {
            return await _context.JobApplications
                .Include(x => x.Company)
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<JobApplication?> GetByIdForUserAsync(int id, string userId)
        {
            return await _context.JobApplications
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        }
        public async Task CreateAsync(JobApplication entity)
        {
            _context.JobApplications.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(JobApplication entity)
        {
            _context.JobApplications.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
