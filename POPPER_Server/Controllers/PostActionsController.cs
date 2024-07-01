using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using POPPER_Server.Helpers;

namespace POPPER_Server.Controllers;

[Route("api/[controller]")]
public class PostActionsController : ControllerBase
{
    private readonly IPostActions _postActions;
    private readonly IMapper _mapper;

    public PostActionsController(IPostActions postActions, IMapper mapper)
    {
        _postActions = postActions;
        _mapper = mapper;
    }

    [HttpPost("[action]/{guid}")]
    public async Task<IActionResult> LikePostAsync([FromRoute] string guid)
    {
        try
        {
            return Ok(await _postActions.LikePost(guid));
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpPost("[action]/{guid}")]
    public async Task<IActionResult> CommentOnPostAsync(
        [FromRoute] string guid,
        [FromBody] NewCommentDto comment
    )
    {
        try
        {
            return Ok(await _postActions.PostComment(guid, comment));
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpPost("[action]/{guid}")]
    public async Task<IActionResult> SavePost([FromRoute] string guid)
    {
        try
        {
            var user = await Request.GetUserAsync();
            return Ok(await _postActions.SavePost(guid, user));
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpPost("[action]/{guid}")]
    public async Task<IActionResult> SharePostAsync([FromRoute] string guid)
    {
        try
        {
            return Ok(await _postActions.GetShareLink(guid));
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
}
