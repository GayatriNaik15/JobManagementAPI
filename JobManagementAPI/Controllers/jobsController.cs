using JobManagementAPI.Data;
using JobManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobManagementAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class jobsController : ControllerBase
    {
        public readonly ApplicationDbContext _context;
        public jobsController(ApplicationDbContext context) => _context = context;

        [HttpPost]
        public IActionResult AddJob([FromBody] AddJob jobData)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var location = _context.Locations.Find(jobData.LocationId);
                if (location == null)
                {
                    return BadRequest("Invalid locationId");
                }

                var department = _context.Departments.Find(jobData.DepartmentId);
                if (department == null)
                {
                    return BadRequest("Invalid departmentId");
                }

                var job = new Jobs
                {
                    Title = jobData.Title,
                    Description = jobData.Description,
                    Location = location,
                    Department = department,
                    ClosingDate = jobData.ClosingDate
                };

                string jobCode = GenerateUniqueJobCode();

                job.Code = jobCode;
                if(job.PostedDate == DateTime.MinValue)
                    job.PostedDate = DateTime.Now;

                _context.Jobs.Add(job);
                _context.SaveChanges();

                return CreatedAtAction(nameof(GetJobById), new { id = job.Id }, job);
            }
            catch(Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request. "+ex.Message);
            }

        }

        private string GenerateUniqueJobCode()
        {
            // Get latest job code from the database
            var latestJob = _context.Jobs.OrderByDescending(j => j.Id).FirstOrDefault();
            int latestJobNumber = 0;

            if (latestJob != null && latestJob.Code != null && latestJob.Code.StartsWith("JOB-"))
            {
                // Extract the numeric part of the latest job code and parse it
                string numericPart = latestJob.Code.Substring(4);
                if (int.TryParse(numericPart, out latestJobNumber))
                {
                    // Increment the numeric part by 1
                    latestJobNumber++;
                }
            }

            // If no previous data is available or the latest job number is 0, start from 1
            if (latestJobNumber == 0)
            {
                latestJobNumber = 1;
            }

            // Format the incremented number back to the desired format "JOB-XX"
            string newJobCode = "JOB-" + latestJobNumber.ToString("D2"); // D2 ensures leading zeros if needed

            return newJobCode;
        }

        [HttpPut("{id}")]
        public IActionResult UpdateJob(int id, [FromBody] AddJob jobData)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var job = _context.Jobs.Find(id);
                if (job == null)
                {
                    return NotFound("Job not found");
                }

                var location = _context.Locations.Find(jobData.LocationId);
                if (location == null)
                {
                    return BadRequest("Invalid locationId");
                }

                var department = _context.Departments.Find(jobData.DepartmentId);
                if (department == null)
                {
                    return BadRequest("Invalid departmentId");
                }

                // Update the job properties
                job.Title = jobData.Title == null ? job.Title : jobData.Title;
                job.Description = jobData.Description == null ? job.Description : jobData.Description;
                job.Location = location;
                job.Department = department;
                job.ClosingDate = jobData.ClosingDate == DateTime.MinValue ? job.ClosingDate : jobData.ClosingDate;

                _context.SaveChanges();

                return Ok(job);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, "An error occurred while processing the request: " + ex.Message);
            }
        }

        [HttpPost("list")]
        public IActionResult GetJobsList([FromBody] JobList parameters)
        {
            try
            {              
                // Query jobs from the database
                var query = _context.Jobs.Include(j => j.Location).Include(j => j.Department).AsQueryable();

                // Apply search string filter if provided
                if (!string.IsNullOrEmpty(parameters.Q))
                {
                    query = query.Where(j => j.Title != null && j.Title.Contains(parameters.Q) || j.Description != null && j.Description.Contains(parameters.Q));
                }

                // Apply location filter if provided
                if (parameters.LocationId != null)
                {
                    query = query.Where(j => j.Location != null && j.Location.Id == parameters.LocationId);
                }

                // Apply department filter if provided
                if (parameters.DepartmentId != null)
                {
                    query = query.Where(j => j.Department != null && j.Department.Id == parameters.DepartmentId);
                }

                // Project only the title properties of Location and Department
                var projectedQuery = query.Select(j => new JobListing
                {
                    Id = j.Id,
                    Code = j.Code,
                    Title = j.Title,
                    Location = j.Location != null ? j.Location.Title : null,
                    Department = j.Department != null ? j.Department.Title : null,
                    PostedDate = j.PostedDate,
                    ClosingDate = j.ClosingDate
                });

                // Calculate total count before pagination
                int totalCount = projectedQuery.Count();

                // Paginate the results
                var paginatedListings = projectedQuery
                    .Skip((parameters.PageNo - 1) * parameters.PageSize)
                    .Take(parameters.PageSize)
                    .ToList();

                var response = new JobListResponse
                {
                    Total = totalCount,
                    Data = paginatedListings
                };

                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request: " + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetJobById(int id)
        {
            try
            {
                var job = _context.Jobs
                    .Include(j => j.Location)
                    .Include(j => j.Department)
                    .FirstOrDefault(j => j.Id == id);

                if (job == null)
                {
                    return NotFound("Job not found");
                }

                return Ok(job);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request: " + ex.Message);
            }
        }

    }
}
