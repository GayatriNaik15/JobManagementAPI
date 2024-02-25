using JobManagementAPI.Models;

namespace JobManagementAPI
{
    public class JobListResponse
    {
        public int Total { get; set; }
        public List<JobListing>? Data { get; set; }
    }
}
