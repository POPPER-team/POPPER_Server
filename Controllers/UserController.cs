using Microsoft.AspNetCore.Mvc;
using POPPER_Server.Dtos;

namespace POPPER_Server.Controllers;

[Route("api/[controller]")]
public class UserController : ControllerBase
{

    [HttpGet("[action]")]
    public IActionResult Login(string username, string password)
    {
        throw new NotImplementedException();
    }
    [HttpPost("[action]")]
    public IActionResult Register([FromForm] UserDto userDto)
    {
        throw new NotImplementedException();
    }
    [HttpGet("[action]/{userGuid}")]
    public IActionResult GetUser([FromRoute] string userGuid)
    {
        throw new NotImplementedException();
    }
    [HttpGet("[action]")]
    public IActionResult SearchUser(string searchString)
    {
        throw new NotImplementedException();
    }
}