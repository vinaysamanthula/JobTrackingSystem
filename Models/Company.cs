namespace JobTrackingSystem.Models;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public string Website { get; set; }

    public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();

}
