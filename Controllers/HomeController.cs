using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;


[ApiController]
public class HomeController : ControllerBase
{
    
    [HttpGet("/")]
    public IActionResult HealthCheck()
    {
        return Ok("Health Check");
    }
}