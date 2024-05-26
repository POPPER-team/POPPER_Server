using Microsoft.AspNetCore.Mvc;
using POPPER_Server.Dtos;

namespace POPPER_Server.Controllers;
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    public PostController()
    {

    }
    [HttpPost("[action]")]
    public IActionResult CreatePost([FromBody] NewPostDto dto)
    {
        throw new NotImplementedException();
    }

    [HttpGet("[action]")]
    public IActionResult GetRecomendedPosts()
    {
        throw new NotImplementedException();
    }

    [HttpGet("[action]/{guid}")]
    public IActionResult GetPost([FromRoute] string guid)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("[action]{guid}")]
    public IActionResult DeletePost([FromRoute] string guid)
    {
        throw new NotImplementedException();
    }
}
