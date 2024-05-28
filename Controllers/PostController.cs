using Microsoft.AspNetCore.Mvc;
using POPPER_Server.Dtos;
using POPPER_Server.Helpers;
using POPPER_Server.Models;
using POPPER_Server.Services;

namespace POPPER_Server.Controllers;
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;
    public PostController(IPostService postService)
    {
        _postService = postService;
    }
    [HttpPost("[action]")]
    public async Task<IActionResult> CreatePost([FromBody] NewPostDto dto)
    {
        User user = await Request.GetUserAsync();
        try
        {
            await _postService.CreatePost(user, dto);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        return Ok();
    }

    [HttpGet("[action]")]
    public IActionResult GetRecomendedPosts()
    {
        throw new NotImplementedException();
    }

    [HttpGet("[action]/{guid}")]
    public async Task<IActionResult> GetPost([FromRoute] string guid)
    {
        try
        {
            return Ok(await _postService.GetPost(guid));
        }
        catch (Exception e)
        {
            return NotFound();
        }
    }

    [HttpDelete("[action]{guid}")]
    public IActionResult DeletePost([FromRoute] string guid)
    {
        throw new NotImplementedException();
    }
}
