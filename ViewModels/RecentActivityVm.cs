namespace JobTrackingSystem.ViewModels
{
    public class RecentActivityVm
    {
        public string Action { get; set; } = "";
        public string EntityName { get; set; } = "";
        public string? Description { get; set; }
        public DateTime Timestamp { get; set; }


    }
}
