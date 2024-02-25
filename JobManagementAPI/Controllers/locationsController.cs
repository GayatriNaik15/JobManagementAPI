using JobManagementAPI.Data;
using JobManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobManagementAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class locationsController : ControllerBase
    {
        public readonly ApplicationDbContext _context;
        public locationsController(ApplicationDbContext context) => _context = context;

        [HttpPost]
        public IActionResult CreateLocation([FromBody] Locations location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Locations.Add(location);
            _context.SaveChanges();

            return Ok(location);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateLocation(int id, [FromBody] Locations location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingLocation = _context.Locations.Find(id);
            if (existingLocation == null)
            {
                return NotFound("Location not found");
            }

            existingLocation.Title = location.Title; // Update other properties as needed

            _context.SaveChanges();

            return Ok(existingLocation);
        }

        [HttpGet]
        public IActionResult GetLocations()
        {
            var locations = _context.Locations.ToList();
            return Ok(locations);
        }
    }
}
