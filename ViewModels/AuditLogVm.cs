namespace JobTrackingSystem.ViewModels
{
    public class AuditLogVm
    {
        public int Id { get; set; }
        public string UserEmail { get; set; } = "";
        public string Action { get; set; } = "";
        public string EntityName { get; set; } = "";
        public int EntityId { get; set; }
        public DateTime Timestamp { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? CompanyName { get; set; }    
    }
}
