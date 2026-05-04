using JobTrackingSystem.Models;
using JobTrackingSystem.ViewModels;

namespace JobTrackingSystem.Services
{
    public interface IJobApplicationService
    {
        Task<(List<JobApplication>, int)> SearchAsync(string userId, JobApplicationSearchVm vm);

        Task<List<JobApplication>> GetForUserAsync(string userId);

        Task<JobApplication?> GetByIdForUserAsync(int id, string userId);

        // ✅ UPDATED SIGNATURES (THIS FIXES YOUR ERRORS)

        Task CreateAsync(JobApplication entity, string ip, string userAgent);

        Task UpdateAsync(JobApplicationEditVm vm, string userId, string ip, string userAgent);

        Task DeleteAsync(int id, string userId, string ip, string userAgent);

        Task RestoreAsync(int id, string userId, string ip, string userAgent);
    }
}