using otomobil.Data;
using otomobil.Models;
using otomobil.DTOs.Course;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Reflection.Metadata.BlobBuilder;
using Microsoft.AspNetCore.Authorization;

namespace otomobil.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CourseController : ControllerBase
    {
        private readonly CourseData _courseData;

        public CourseController(CourseData courseData)
        {
            _courseData = courseData;
        }

        [HttpGet("GetAll")]
        // [Authorize]
        public IActionResult GetAll()
        {
            try
            {
                List<Course> courses = _courseData.GetAll();
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }
    }

}

