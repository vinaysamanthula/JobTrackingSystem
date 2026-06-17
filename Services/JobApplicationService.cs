using JobTrackingSystem.Data;
using JobTrackingSystem.Models;
using JobTrackingSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace JobTrackingSystem.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly JobTrackingSystemContext _context;
        private readonly ILogger<JobApplicationService> _logger;

        public JobApplicationService(
            JobTrackingSystemContext context,
            ILogger<JobApplicationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ================= SEARCH =================
        public async Task<(List<JobApplication>, int)> SearchAsync(string userId, JobApplicationSearchVm vm)
        {
            var query = _context.JobApplications
                .Include(x => x.Company)
                .Where(x => x.UserId == userId && !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(vm.Keyword))
            {
                var keyword = vm.Keyword.Trim().ToLower();

                query = query.Where(x =>
                    x.Role.ToLower().Contains(keyword) ||
                    (x.Notes != null && x.Notes.ToLower().Contains(keyword)) ||
                    x.Company.Name.ToLower().Contains(keyword));
            }

            if (!string.IsNullOrEmpty(vm.Status))
                query = query.Where(x => x.Status == vm.Status);

            if (vm.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == vm.CompanyId);

            if (vm.FromDate.HasValue)
                query = query.Where(x => x.DateApplied >= vm.FromDate);

            if (vm.ToDate.HasValue)
                query = query.Where(x => x.DateApplied <= vm.ToDate);
            switch (vm.SortBy)
            {
                case "company":
                    query = query.OrderBy(x => x.Company.Name);
                    break;

                case "status":
                    query = query.OrderBy(x => x.Status);
                    break;

                case "date":
                default:
                    query = query.OrderByDescending(x => x.DateApplied);
                    break;
            }
            var totalCount = await query.CountAsync();

            var data = await query
                .Skip((vm.Page - 1) * vm.PageSize)
                .Take(vm.PageSize)
                .ToListAsync();

            return (data, totalCount);
        }

        // ================= GET =================
        public async Task<List<JobApplication>> GetForUserAsync(string userId)
        {
            return await _context.JobApplications
                .Include(x => x.Company)
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .ToListAsync();
        }

        public async Task<JobApplication?> GetByIdForUserAsync(int id, string userId)
        {
            return await _context.JobApplications
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId && !x.IsDeleted);
        }

        // ================= CREATE =================
        public async Task CreateAsync(JobApplication entity, string ip, string userAgent)
        {
            _context.JobApplications.Add(entity);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} created job {JobId}", entity.UserId, entity.Id);

            var newValues = JsonSerializer.Serialize(new
            {
                entity.CompanyId,
                entity.Role,
                entity.Status,
                entity.DateApplied,
                entity.Notes
            });

            await LogAudit(entity.UserId, "Create", entity, null, newValues, ip, userAgent);

        }

        // ================= UPDATE =================
        public async Task UpdateAsync(JobApplicationEditVm vm, string userId, string ip, string userAgent)
        {
            var existing = await _context.JobApplications
                .FirstOrDefaultAsync(x => x.Id == vm.Id && x.UserId == userId);

            if (existing == null)
                throw new Exception("Not found or unauthorized");

            var oldCopy = new JobApplication
            {
                CompanyId = existing.CompanyId,
                Role = existing.Role,
                Status = existing.Status,
                DateApplied = existing.DateApplied,
                Notes = existing.Notes
            };

            existing.CompanyId = vm.CompanyId;
            existing.Role = vm.Role;
            existing.Status = vm.Status;
            existing.DateApplied = vm.DateApplied;
            existing.Notes = vm.Notes;

            await _context.SaveChangesAsync();

            var changes = GetChanges(oldCopy, existing);
            var changesJson = JsonSerializer.Serialize(changes);

            _logger.LogInformation("User {UserId} updated job {JobId}", userId, vm.Id);

            await LogAudit(userId, "Update", existing, null, changesJson, ip, userAgent);
        }

        // ================= DELETE =================
        public async Task DeleteAsync(int id, string userId, string ip, string userAgent)
        {
            var entity = await _context.JobApplications
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (entity == null)
                throw new Exception("Not found or unauthorized");

            entity.IsDeleted = true;
            await _context.SaveChangesAsync();

            _logger.LogWarning("User {UserId} deleted job {JobId}", userId, id);

            await LogAudit(userId, "Delete", entity, null, null, ip, userAgent);
        }

        // ================= RESTORE =================
        public async Task RestoreAsync(int id, string userId, string ip, string userAgent)
        {
            var entity = await _context.JobApplications
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (entity == null)
                throw new Exception("Not found");

            entity.IsDeleted = false;
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} restored job {JobId}", userId, id);

            await LogAudit(userId, "Restore", entity, null, null, ip, userAgent);
        }

        // ================= AUDIT =================
        private async Task LogAudit(
     string userId,
     string action,
     JobApplication entity,
     string? oldValues,
     string? newValues,
     string ip,
     string userAgent)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == entity.CompanyId);

            var description =
                $"{action} {company?.Name} - {entity.Role}";

            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                EntityName = "JobApplication",
                EntityId = entity.Id,
                Timestamp = DateTime.UtcNow,
                OldValues = oldValues,
                NewValues = newValues,
                IpAddress = ip,
                UserAgent = userAgent,
                Description = description
            };

            _context.AuditLogs.Add(log);

            await _context.SaveChangesAsync();
        }

        private Dictionary<string, object> GetChanges(JobApplication oldObj, JobApplication newObj)
        {
            var changes = new Dictionary<string, object>();

            if (oldObj.Role != newObj.Role)
                changes["Role"] = new { Old = oldObj.Role, New = newObj.Role };

            if (oldObj.Status != newObj.Status)
                changes["Status"] = new { Old = oldObj.Status, New = newObj.Status };

            if (oldObj.Notes != newObj.Notes)
                changes["Notes"] = new { Old = oldObj.Notes, New = newObj.Notes };

            if (oldObj.CompanyId != newObj.CompanyId)
                changes["CompanyId"] = new { Old = oldObj.CompanyId, New = newObj.CompanyId };

            return changes;
        }
    }
}