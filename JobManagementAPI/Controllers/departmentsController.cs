using JobManagementAPI.Data;
using JobManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobManagementAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class departmentsController : ControllerBase
    {
        public readonly ApplicationDbContext _context;
        public departmentsController(ApplicationDbContext context) => _context = context;

        [HttpPost]
        public IActionResult CreateDepartment([FromBody] Departments department)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Departments.Add(department);
            _context.SaveChanges();

            return Ok(department);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateDepartment(int id, [FromBody] Departments department)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingDepartment = _context.Departments.Find(id);
            if (existingDepartment == null)
            {
                return NotFound("Department not found");
            }

            existingDepartment.Title = department.Title; // Update other properties as needed

            _context.SaveChanges();

            return Ok(existingDepartment);
        }

        [HttpGet]
        public IActionResult GetDepartments()
        {
            var departments = _context.Departments.ToList();
            return Ok(departments);
        }
    }
}
