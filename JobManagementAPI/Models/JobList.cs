namespace JobManagementAPI.Models
{
    public class JobList
    {
        public string? Q { get; set; } // Search string
        public int PageNo { get; set; } // Page number
        public int PageSize { get; set; } // Page size
        public int? LocationId { get; set; } // Optional location id
        public int? DepartmentId { get; set; } // Optional department id
    }

}
