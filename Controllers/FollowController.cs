using Microsoft.AspNetCore.Mvc;
using POPPER_Server.Services;

namespace POPPER_Server.Controllers;


[Route("api/[controller]")]


public class FollowController : ControllerBase
{
    private readonly IFollowService _followService;
    
    public FollowController(IFollowService followService)
    {
        _followService = followService;
    }
    
    [HttpPost("[action]/{followingGuid}")]
    public async Task<IActionResult> FollowUser([FromRoute] string userGuid, [FromRoute] string followingGuid)
    {
        await _followService.FollowUserAsync(userGuid, followingGuid);
        return Ok();
    }
    
    [HttpDelete("[action]/{followingGuid}")]
    public async Task<IActionResult> UnFollowUser([FromRoute] string userGuid, [FromRoute] string followingGuid)
    {
        await _followService.UnFollowUserAsync(userGuid, followingGuid);
        return Ok();
    }
    
    [HttpGet("followers")]
    public async Task<IActionResult> GetFollowers([FromRoute] string userGuid)
    {
        return Ok(await _followService.GetFollowersAsync(userGuid));
    }
    
    [HttpGet("following")]
    public async Task<IActionResult> GetFollowing([FromRoute] string userGuid)
    {
        return Ok(await _followService.GetFollowingAsync(userGuid));
    }
    
    
}