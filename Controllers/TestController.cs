using otomobil.Data;
using otomobil.Models;
using otomobil.DTOs.Test;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Reflection.Metadata.BlobBuilder;
using Microsoft.AspNetCore.Authorization;

namespace otomobil.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TestController : ControllerBase
    {

        [HttpGet("GetAll")]
        // [Authorize]
        public IActionResult GetAll()
        {
            try
            {
                var res =  new TestDTO { 
                   name = "krisna",
                   date = "10-02-2000",
                };
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }
    }

}

