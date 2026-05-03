using JobTrackingSystem.Models;

namespace JobTrackingSystem.Services
{
    public interface IJobApplicationService
    {
        Task<List<JobApplication>> GetForUserAsync(string userId);

        Task<JobApplication?> GetByIdForUserAsync(int id, string userId);

        Task CreateAsync(JobApplication entity);

        Task UpdateAsync(JobApplication entity);
    }
}