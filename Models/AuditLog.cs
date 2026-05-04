namespace JobTrackingSystem.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        public string UserId { get; set; } = "";

        public string Action { get; set; } = ""; // Create, Update, Delete, Restore

        public string EntityName { get; set; } = "";

        public int EntityId { get; set; }

        public DateTime Timestamp { get; set; }

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}
