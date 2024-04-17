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

    public class CarCategoryController : ControllerBase
    {
        private readonly CarCategoryData _carCategoryData;

        public CarCategoryController(CarCategoryData carCategoryData)
        {
            _carCategoryData = carCategoryData;
        }

        [HttpGet("GetAll")]
        // [Authorize]
        public IActionResult GetAll()
        {
            try
            {
                List<CarCategory> carCategories = _carCategoryData.GetAll();
                return Ok(carCategories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }
    }

}

