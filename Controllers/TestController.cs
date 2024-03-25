using Microsoft.AspNetCore.Mvc;

namespace POPPER_Server.Controllers;
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    // GET
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hello World");
    }
}