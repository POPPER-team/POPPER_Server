using AutoMapper;
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
    private readonly IMapper _mapper;

    public PostController(IPostService postService, IMapper mapper)
    {
        _postService = postService;
        _mapper = mapper;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> CreatePost([FromBody] NewPostDto dto)
    {
        User user = await Request.GetUserAsync();
        try
        {
            return Ok(_mapper.Map<PostDto>(await _postService.CreatePost(user, dto)));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("[action]/{guid}")]
    public async Task<IActionResult> UploadPostMedia(
        [FromRoute] string guid,
        [FromForm] FileUploadDto file
    )
    {
        User user = await Request.GetUserAsync();
        try
        {
            _postService.UploadMedaToPost(guid, user, file);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

        return Ok();
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetRecommendedPosts()
    {
        return Ok( (await  _postService.GetPosts()));
    }

    [HttpGet("[action]/{guid}")]
    public async Task<IActionResult> GetPostData([FromRoute] string guid)
    {
        try
        {
            return Ok(_mapper.Map<PostDto>(_postService.GetPost(guid)));
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpGet("[action]/{guid}")]
    public async Task<IActionResult> GetPostMedia([FromRoute] string guid)
    {
        try
        {
            return await _postService.GetMedia(guid);
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

