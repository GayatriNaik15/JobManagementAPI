namespace JobManagementAPI.Models
{
    public class AddJob
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int LocationId { get; set; }
        public int DepartmentId { get; set; }
        public DateTime ClosingDate { get; set; }

    }
}
