namespace JobTrackingSystem.ViewModels
{
    public class DashboardVm
    {
        public int Total { get; set; }
        public int Applied { get; set; }
        public int Interview { get; set; }
        public int Offer { get; set; }
        public int Rejected { get; set; }
        public List<RecentActivityVm> RecentActivities { get; set; }
    = new();
    }
}