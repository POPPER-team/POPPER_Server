using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POPPER_Server.Dtos;
using POPPER_Server.Helpers;
using POPPER_Server.Models;
using POPPER_Server.Services;

namespace POPPER_Server.Controllers;

[Route("api/[controller]")]
public class PostActionsController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IMapper _mapper;

    public PostActionsController(IPostService postService, IMapper mapper)
    {
        _postService = postService;
        _mapper = mapper;
    }

    [HttpPost("[action]/{guid}")]
    public IActionResult LikePost([FromRoute] string guid)
    {
        return Ok();
    }

    [HttpPost("[action]/{guid}")]
    public IActionResult CommentOnPost([FromRoute] string guid)
    {
        return Ok();
    }

    [HttpPost("[action]/{guid}")]
    public IActionResult SavePost([FromRoute] string guid)
    {
        return Ok();
    }

    [HttpPost("[action]/{guid}")]
    public IActionResult SharePost([FromRoute] string guid)
    {
        return Ok();
    }
}
